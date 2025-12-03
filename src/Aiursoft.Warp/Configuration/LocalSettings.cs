namespace Aiursoft.Warp.Configuration;

public abstract class LocalSettings
{
    public required bool AllowRegister { get; init; } = true;

    public required bool AllowWeakPassword { get; init; } = true;
}
