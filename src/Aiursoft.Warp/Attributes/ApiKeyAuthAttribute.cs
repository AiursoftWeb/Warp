using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Aiursoft.Warp.Services;
using System.Security.Claims;

namespace Aiursoft.Warp.Attributes;

/// <summary>
/// API Key 认证特性
/// 从 Authorization header 中读取 "Bearer {api_key}" 或 "X-API-Key" header
/// </summary>
public class ApiKeyAuthAttribute : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var apiKeyService = context.HttpContext.RequestServices.GetService<ApiKeyService>();
        if (apiKeyService == null)
        {
            context.Result = new StatusCodeResult(500);
            return;
        }

        string? apiKey = null;

        // 尝试从 Authorization header 获取
        if (context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var authValue = authHeader.ToString();
            if (authValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                apiKey = authValue.Substring(7);
            }
        }

        // 尝试从 X-API-Key header 获取
        if (string.IsNullOrEmpty(apiKey))
        {
            context.HttpContext.Request.Headers.TryGetValue("X-API-Key", out var apiKeyHeader);
            apiKey = apiKeyHeader.ToString();
        }

        if (string.IsNullOrEmpty(apiKey))
        {
            context.Result = new JsonResult(new { error = "API key is required" })
            {
                StatusCode = 401
            };
            return;
        }

        var userId = await apiKeyService.ValidateApiKeyAsync(apiKey);
        if (string.IsNullOrEmpty(userId))
        {
            context.Result = new JsonResult(new { error = "Invalid or expired API key" })
            {
                StatusCode = 401
            };
            return;
        }

        // 设置用户身份
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim("AuthType", "ApiKey")
        };

        var identity = new ClaimsIdentity(claims, "ApiKey");
        var principal = new ClaimsPrincipal(identity);
        context.HttpContext.User = principal;
    }
}