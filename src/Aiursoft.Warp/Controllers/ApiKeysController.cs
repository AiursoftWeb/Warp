using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Aiursoft.Warp.Services;
using Aiursoft.Warp.Models.ApiModels;
using System.Security.Claims;

namespace Aiursoft.Warp.Controllers;

/// <summary>
/// API Key 管理控制器（需要用户登录）
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ApiKeysController : Controller
{
    private readonly ApiKeyService _apiKeyService;

    public ApiKeysController(ApiKeyService apiKeyService)
    {
        _apiKeyService = apiKeyService;
    }

    /// <summary>
    /// 创建新的 API Key
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ApiKeyResponse>>> Create([FromBody] CreateApiKeyRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<ApiKeyResponse>.Fail("Invalid request data"));
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(ApiResponse<ApiKeyResponse>.Fail("User not authenticated"));
        }

        var (apiKey, plainKey) = await _apiKeyService.CreateApiKeyAsync(userId, request.Name, request.ExpireAt);

        var response = new ApiKeyResponse
        {
            Id = apiKey.Id,
            Name = apiKey.Name,
            KeyPrefix = apiKey.KeyPrefix,
            CreationTime = apiKey.CreationTime,
            LastUsedAt = apiKey.LastUsedAt,
            ExpireAt = apiKey.ExpireAt,
            IsActive = apiKey.IsActive,
            UsageCount = apiKey.UsageCount,
            PlainKey = plainKey // 仅在创建时返回完整的 key
        };

        return Ok(ApiResponse<ApiKeyResponse>.Ok(response));
    }

    /// <summary>
    /// 获取所有 API Keys
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ApiKeyResponse>>>> List()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(ApiResponse<List<ApiKeyResponse>>.Fail("User not authenticated"));
        }

        var apiKeys = await _apiKeyService.GetUserApiKeysAsync(userId);
        var response = apiKeys.Select(k => new ApiKeyResponse
        {
            Id = k.Id,
            Name = k.Name,
            KeyPrefix = k.KeyPrefix,
            CreationTime = k.CreationTime,
            LastUsedAt = k.LastUsedAt,
            ExpireAt = k.ExpireAt,
            IsActive = k.IsActive,
            UsageCount = k.UsageCount
            // PlainKey 不返回
        }).ToList();

        return Ok(ApiResponse<List<ApiKeyResponse>>.Ok(response));
    }

    /// <summary>
    /// 更新 API Key
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(Guid id, [FromBody] UpdateApiKeyRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(ApiResponse<bool>.Fail("User not authenticated"));
        }

        var success = await _apiKeyService.UpdateApiKeyAsync(
            id, 
            userId, 
            request.Name, 
            request.IsActive, 
            request.ExpireAt
        );

        if (!success)
        {
            return NotFound(ApiResponse<bool>.Fail("API Key not found"));
        }

        return Ok(ApiResponse<bool>.Ok(true));
    }

    /// <summary>
    /// 删除 API Key
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(ApiResponse<bool>.Fail("User not authenticated"));
        }

        var success = await _apiKeyService.DeleteApiKeyAsync(id, userId);

        if (!success)
        {
            return NotFound(ApiResponse<bool>.Fail("API Key not found"));
        }

        return Ok(ApiResponse<bool>.Ok(true));
    }
}