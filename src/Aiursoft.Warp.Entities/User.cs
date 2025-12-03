using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Aiursoft.Warp.Entities;

public class User : IdentityUser
{
    public const string DefaultAvatarPath = "Workspace/avatar/default-avatar.jpg";

    [MaxLength(30)]
    [MinLength(2)]
    public required string DisplayName { get; set; }

    [MaxLength(150)] [MinLength(2)] public string AvatarRelativePath { get; set; } = DefaultAvatarPath;

    public DateTime CreationTime { get; init; } = DateTime.UtcNow;
}


public class ShorterLink
{
    [Key]
    public Guid Id { get; init; }

    [MaxLength(100)]
    public string? Title { get; set; }

    [MaxLength(65535)]
    public required string TargetUrl { get; set; }

    [MaxLength(32)]
    public required string RedirectTo { get; init; }

    public DateTime? ExpireAt { get; init; }

    public bool IsCustom { get; init; }

    public bool IsPrivate { get; init; }

    [MaxLength(100)]
    public string? Password { get; set; }

    public long? MaxClicks { get; init; }

    public long Clicks { get; set; }

    public DateTime CreationTime { get; init; } = DateTime.UtcNow;

    [StringLength(64)]
    public required string UserId { get; init; }

    [ForeignKey(nameof(UserId))]
    [NotNull]
    public User? User { get; init; }
}
