using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Warp.Models.ApiModels;

public class UpdateLinkApiModel
{
    [MaxLength(100)]
    public string? Title { get; set; }

    [MaxLength(65535)]
    public string? TargetUrl { get; set; }

    [MaxLength(32)]
    public string? CustomCode { get; set; }

    public DateTime? ExpireAt { get; set; }

    public bool? IsPrivate { get; set; }

    [MaxLength(100)]
    public string? Password { get; set; }

    public long? MaxClicks { get; set; }
}
