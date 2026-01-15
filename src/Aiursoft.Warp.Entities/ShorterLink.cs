using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Aiursoft.Warp.Entities;

public class ShorterLink
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(100)]
    public string? Title { get; set; }

    [MaxLength(65535)]
    public required string TargetUrl { get; set; }

    [MaxLength(32)]
    public required string RedirectTo { get; set; }

    public DateTime? ExpireAt { get; set; }

    public bool IsCustom { get; set; }

    public bool IsPrivate { get; set; }

    [MaxLength(100)]
    public string? Password { get; set; }

    public long? MaxClicks { get; set; }

    public long Clicks { get; set; }

    public DateTime CreationTime { get; init; } = DateTime.UtcNow;

    [StringLength(64)]
    public required string UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    [NotNull]
    public User? User { get; set; }
}