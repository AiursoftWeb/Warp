using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.Warp.Entities;

public class WarpHit
{
    [Key]
    public Guid Id { get; set; }

    public Guid LinkId { get; set; }

    [ForeignKey(nameof(LinkId))]
    public ShorterLink? Link { get; set; }

    [MaxLength(100)]
    public string? IP { get; set; }

    [MaxLength(500)]
    public string? Device { get; set; }

    [MaxLength(100)]
    public string? Region { get; set; }

    [MaxLength(1000)]
    public string? Referer { get; set; }

    public DateTime HitTime { get; set; } = DateTime.UtcNow;
}
