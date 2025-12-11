using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Aiursoft.Warp.Entities;

namespace Aiursoft.Warp.Services;

public class ApiKeyService
{
    private readonly TemplateDbContext _dbContext;
    private readonly PasswordService _passwordService;

    public ApiKeyService(TemplateDbContext dbContext, PasswordService passwordService)
    {
        _dbContext = dbContext;
        _passwordService = passwordService;
    }

    /// <summary>
    /// 生成新的 API Key
    /// 格式: ak_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx (总共40字符)
    /// </summary>
    public string GenerateApiKey()
    {
        const string prefix = "ak_";
        const int keyLength = 32; // 生成32个随机字符
        
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var bytes = new byte[keyLength];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);

        var sb = new StringBuilder(keyLength);
        foreach (var b in bytes)
        {
            sb.Append(chars[b % chars.Length]);
        }

        string keyBody = sb.ToString();        
        return prefix + keyBody;
    }

    /// <summary>
    /// 获取 API Key 的前缀（用于显示和快速查找）
    /// </summary>
    public string GetKeyPrefix(string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey) || apiKey.Length < 11)
        {
            throw new ArgumentException("Invalid API key format");
        }
        return apiKey.Substring(0, 11); // ak_xxxxxxxx
    }

    /// <summary>
    /// 创建新的 API Key
    /// </summary>
    public async Task<(ApiKey ApiKey, string PlainKey)> CreateApiKeyAsync(string userId, string name, DateTime? expireAt = null)
    {
        var plainKey = GenerateApiKey();
        var keyHash = _passwordService.HashPassword(plainKey);
        var keyPrefix = GetKeyPrefix(plainKey);

        var apiKey = new ApiKey
        {
            UserId = userId,
            Name = name,
            KeyHash = keyHash,
            KeyPrefix = keyPrefix,
            ExpireAt = expireAt,
            IsActive = true
        };

        _dbContext.ApiKeys.Add(apiKey);
        await _dbContext.SaveChangesAsync();

        return (apiKey, plainKey);
    }

    /// <summary>
    /// 验证 API Key 并返回用户ID
    /// </summary>
    public async Task<string?> ValidateApiKeyAsync(string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            return null;
        }

        var keyPrefix = GetKeyPrefix(apiKey);
        
        // 前缀匹配
        var apiKeys = await _dbContext.ApiKeys
            .Where(k => k.KeyPrefix == keyPrefix && k.IsActive)
            .ToListAsync();

        foreach (var key in apiKeys)
        {
            // 完整匹配
            if (_passwordService.VerifyPassword(key.KeyHash, apiKey))
            {
                // 检查是否过期
                if (key.ExpireAt.HasValue && key.ExpireAt < DateTime.UtcNow)
                {
                    return null;
                }

                // 更新最后使用时间和使用次数
                key.LastUsedAt = DateTime.UtcNow;
                key.UsageCount++;
                await _dbContext.SaveChangesAsync();

                return key.UserId;
            }
        }

        return null;
    }

    /// <summary>
    /// 获取用户的所有 API Keys
    /// </summary>
    public async Task<List<ApiKey>> GetUserApiKeysAsync(string userId)
    {
        return await _dbContext.ApiKeys
            .Where(k => k.UserId == userId)
            .OrderByDescending(k => k.CreationTime)
            .ToListAsync();
    }

    /// <summary>
    /// 删除 API Key
    /// </summary>
    public async Task<bool> DeleteApiKeyAsync(Guid id, string userId)
    {
        var apiKey = await _dbContext.ApiKeys
            .FirstOrDefaultAsync(k => k.Id == id && k.UserId == userId);

        if (apiKey == null)
        {
            return false;
        }

        _dbContext.ApiKeys.Remove(apiKey);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 更新 API Key
    /// </summary>
    public async Task<bool> UpdateApiKeyAsync(Guid id, string userId, string? name = null, bool? isActive = null, DateTime? expireAt = null)
    {
        var apiKey = await _dbContext.ApiKeys
            .FirstOrDefaultAsync(k => k.Id == id && k.UserId == userId);

        if (apiKey == null)
        {
            return false;
        }

        if (name != null)
        {
            apiKey.Name = name;
        }

        if (isActive.HasValue)
        {
            apiKey.IsActive = isActive.Value;
        }

        if (expireAt.HasValue)
        {
            apiKey.ExpireAt = expireAt;
        }

        await _dbContext.SaveChangesAsync();
        return true;
    }
}