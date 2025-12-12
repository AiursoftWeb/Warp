using System.ComponentModel.DataAnnotations;

namespace Aiursoft.Warp.Models.ApiModels;

// API Key 管理模型
public class CreateApiKeyRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    public DateTime? ExpireAt { get; set; }
}

public class ApiKeyResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string KeyPrefix { get; set; } = string.Empty;
    public DateTime CreationTime { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public DateTime? ExpireAt { get; set; }
    public bool IsActive { get; set; }
    public int UsageCount { get; set; }
    
    /// <summary>
    /// 完整的 API Key（仅在创建时返回一次）
    /// </summary>
    public string? PlainKey { get; set; }
}

public class UpdateApiKeyRequest
{
    [MaxLength(200)]
    public string? Name { get; set; }
    
    public bool? IsActive { get; set; }
    
    public DateTime? ExpireAt { get; set; }
}


public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
    
    public static ApiResponse<T> Ok(T data)
    {
        return new ApiResponse<T> { Success = true, Data = data };
    }
    
    public static ApiResponse<T> Fail(string error)
    {
        return new ApiResponse<T> { Success = false, Error = error };
    }
}