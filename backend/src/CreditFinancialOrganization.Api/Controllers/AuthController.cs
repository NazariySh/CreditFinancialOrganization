using CreditFinancialOrganization.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CreditFinancialOrganization.Application.Interfaces;
using CreditFinancialOrganization.Application.DTOs.Auth;

namespace CreditFinancialOrganization.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private const string RefreshTokenCookieName = "refreshToken";

    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto, CancellationToken cancellationToken)
    {
        var token = await _authService.LoginAsync(loginDto, cancellationToken);

        SetCookie(
            RefreshTokenCookieName,
            token.RefreshToken.Token,
            token.RefreshToken.ExpiryTime.AddMinutes(30));

        return Ok(token);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(string accessToken, CancellationToken cancellationToken)
    {
        var refreshToken = GetCookie(RefreshTokenCookieName);

        if (string.IsNullOrEmpty(refreshToken))
        {
            return BadRequest("Refresh token is required.");
        }

        var tokenDto = new TokenDto(accessToken, new RefreshTokenDto(refreshToken, default));
        var token = await _authService.RefreshTokenAsync(tokenDto, cancellationToken);

        SetCookie(
            RefreshTokenCookieName,
            token.RefreshToken.Token,
            token.RefreshToken.ExpiryTime.AddDays(1));

        return Ok(token);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var userId = User.GetId();

        await _authService.LogoutAsync(userId, cancellationToken);

        RemoveCookie(RefreshTokenCookieName);

        return NoContent();
    }

    [HttpGet("auth-state")]
    public IActionResult GetAuthState()
    {
        return Ok(new
        {
            isAuthenticated = User.Identity?.IsAuthenticated ?? false
        });
    }

    private string? GetCookie(string key)
    {
        Request.Cookies.TryGetValue(key, out var value);

        return value;
    }

    private void SetCookie(string key, string value, DateTime expiryTime)
    {
        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = expiryTime
        };

        Response.Cookies.Append(key, value, options);
    }

    private void RemoveCookie(string key)
    {
        Response.Cookies.Delete(key);
    }
}