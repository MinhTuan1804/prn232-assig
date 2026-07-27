using System.Text;
using FlashShop.Shared.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Validate the same issuer, audience, and symmetric signature used by Identity.
// This lets the gateway reject invalid tokens before proxying protected traffic.
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});
builder.Services.AddAuthorization();

// Load routes, clusters, and destination service addresses from ReverseProxy
// configuration. YARP preserves the incoming API path when forwarding requests.
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Development clients can call the gateway from a different local origin.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAll");

// Authentication must run before authorization and before the reverse proxy so
// the downstream request carries an already validated user principal.
app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger-doc/identity/v1/swagger.json", "Identity API");
    c.SwaggerEndpoint("/swagger-doc/catalog/v1/swagger.json", "Catalog API");
    c.SwaggerEndpoint("/swagger-doc/inventory/v1/swagger.json", "Inventory API");
    c.SwaggerEndpoint("/swagger-doc/ordering/v1/swagger.json", "Ordering API");
    c.SwaggerEndpoint("/swagger-doc/notification/v1/swagger.json", "Notification API");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "FlashShop Microservices API Documentation";
});

app.MapGet("/api/gateway/health", async (IHttpClientFactory clientFactory, IConfiguration config) =>
{
    // Read the same destinations used by YARP instead of duplicating service URLs.
    var services = new Dictionary<string, string>
    {
        { "identity", config["ReverseProxy:Clusters:identity-cluster:Destinations:destination1:Address"] ?? "http://identity-api:8080" },
        { "catalog", config["ReverseProxy:Clusters:catalog-cluster:Destinations:destination1:Address"] ?? "http://catalog-api:8080" },
        { "inventory", config["ReverseProxy:Clusters:inventory-cluster:Destinations:destination1:Address"] ?? "http://inventory-api:8080" },
        { "ordering", config["ReverseProxy:Clusters:ordering-cluster:Destinations:destination1:Address"] ?? "http://ordering-api:8080" },
        { "notification", config["ReverseProxy:Clusters:notification-cluster:Destinations:destination1:Address"] ?? "http://notification-api:8080" }
    };

    var client = clientFactory.CreateClient();
    client.Timeout = TimeSpan.FromSeconds(2);

    var results = new Dictionary<string, bool>();

    // A short timeout keeps one unavailable microservice from blocking the
    // aggregate health response for an extended period.
    foreach (var (serviceName, address) in services)
    {
        try
        {
            var response = await client.GetAsync($"{address}/swagger/index.html");
            results[serviceName] = response.IsSuccessStatusCode || 
                                  response.StatusCode == System.Net.HttpStatusCode.NotFound || 
                                  response.StatusCode == System.Net.HttpStatusCode.Unauthorized;
        }
        catch
        {
            results[serviceName] = false;
        }
    }

    return Results.Ok(results);
});

// Register YARP last so gateway-owned endpoints such as health and Swagger are
// matched before the catch-all proxy routes.
app.MapReverseProxy();

app.Run();

