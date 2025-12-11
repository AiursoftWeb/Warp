using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Aiursoft.Warp.Entities;

public class ApiKey
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// API Key 的值（需要加密存储）
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string KeyHash { get; set; } = string.Empty;
    
    /// <summary>
    /// API Key 的前缀，用于快速查找（明文存储）
    /// 格式: ak_xxxxxx (前8个字符)
    /// </summary>
    [Required]
    [MaxLength(16)]
    public string KeyPrefix { get; set; } = string.Empty;
    
    /// <summary>
    /// 所属用户ID
    /// </summary>
    [Required]
    [StringLength(64)]
    public required string UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    [NotNull]
    public User? User { get; set; }

    
    /// <summary>
    /// API Key 名称/描述
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreationTime { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// 最后使用时间
    /// </summary>
    public DateTime? LastUsedAt { get; set; }
    
    /// <summary>
    /// 过期时间
    /// </summary>
    public DateTime? ExpireAt { get; set; }
    
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// 使用次数
    /// </summary>
    public int UsageCount { get; set; } = 0;
}