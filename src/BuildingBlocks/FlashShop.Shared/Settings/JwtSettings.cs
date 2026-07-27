namespace FlashShop.Shared.Settings;

/// <summary>
/// Defines the JWT values shared by the gateway and every protected service.
/// The issuer, audience, and signing key must match so that a token created by
/// Identity can be validated without making another request to Identity.
/// </summary>
public class JwtSettings
{
    /// <summary>Configuration section bound from appsettings.json.</summary>
    public const string SectionName = "JwtSettings";

    /// <summary>Identifies the service that issued the token.</summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>Identifies the intended recipients of the token.</summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>Symmetric key used to sign and validate tokens.</summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>Token lifetime measured from the time it is issued.</summary>
    public int ExpiryInMinutes { get; set; } = 60;
}
