namespace Aiursoft.Warp.Configuration;

public class AppSettings
{
    public required string AuthProvider { get; init; } = "Local";
    public bool LocalEnabled => AuthProvider == "Local";
    public bool OIDCEnabled => AuthProvider == "OIDC";

    public required OidcSettings OIDC { get; init; }
    public required LocalSettings Local { get; init; }

    /// <summary>
    /// Keep the user sign in after the browser is closed.
    /// </summary>
    public bool PersistsSignIn { get; init; }

    /// <summary>
    /// Automatically assign the user to this role when they log in.
    /// </summary>
    public string? DefaultRole { get; init; } = string.Empty;

    /// <summary>
    /// The global API key for the application.
    /// </summary>
    public string? ApiKey { get; init; } = string.Empty;
}
