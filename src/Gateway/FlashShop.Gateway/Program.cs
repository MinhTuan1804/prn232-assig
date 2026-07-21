using System.Text;
using FlashShop.Shared.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// JWT
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

// YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddHttpClient();

var app = builder.Build();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/gateway/health", async (IHttpClientFactory clientFactory, IConfiguration config) =>
{
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

app.MapReverseProxy();

app.Run();

