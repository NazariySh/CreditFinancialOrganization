using CreditFinancialOrganization.Application.DTOs.Auth;
using CreditFinancialOrganization.Application.Interfaces;
using CreditFinancialOrganization.Domain.Entities.Users;
using CreditFinancialOrganization.Domain.Exceptions;
using CreditFinancialOrganization.Domain.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CreditFinancialOrganization.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenProvider _tokenProvider;
    private readonly IValidator<LoginDto> _validator;

    public AuthService(
        UserManager<User> userManager,
        IUnitOfWork unitOfWork,
        ITokenProvider tokenProvider,
        IValidator<LoginDto> validator)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<TokenDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(loginDto);

        await _validator.ValidateAndThrowAsync(loginDto, cancellationToken);

        var user = await _unitOfWork.Users.GetByEmailAsync(
            loginDto.Email,
            cancellationToken: cancellationToken);

        if (user is null || !await IsPasswordValidAsync(user, loginDto.Password))
        {
            throw new ArgumentException("Invalid username/password");
        }

        return await CreateTokenAsync(user, populateExp: true);
    }

    public async Task LogoutAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        ArgumentEmptyException.ThrowIfEmpty(userId);

        await _unitOfWork.RefreshTokens.UpdateTokenAsync(
            userId,
            null,
            null,
            cancellationToken: cancellationToken);
    }

    public async Task<TokenDto> RefreshTokenAsync(TokenDto token, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(token);

        var principal = await _tokenProvider.GetPrincipalAsync(token.AccessToken);
        var email = principal.Identity?.Name ?? string.Empty;

        var user = await _unitOfWork.Users.GetByEmailAsync(
            email,
            x => x.Include(u => u.RefreshToken),
            cancellationToken: cancellationToken)
            ?? throw new NotFoundException($"User with email {email} not found");

        if (!IsRefreshTokenValid(user, token))
        {
            throw new ArgumentException("Invalid refresh token");
        }

        return await CreateTokenAsync(user, populateExp: false);
    }

    private async Task<TokenDto> CreateTokenAsync(User user, bool populateExp)
    {
        var userRoles = (await _userManager.GetRolesAsync(user)).AsReadOnly();
        var accessToken = _tokenProvider.GenerateAccessToken(user, userRoles);
        var refreshToken = _tokenProvider.GenerateRefreshToken();

        await _unitOfWork.RefreshTokens.UpdateTokenAsync(
            user.Id,
            refreshToken.Token,
            refreshToken.ExpiryTime,
            populateExp);

        return new TokenDto(accessToken, refreshToken);
    }

    private Task<bool> IsPasswordValidAsync(User user, string password)
    {
        return _userManager.CheckPasswordAsync(user, password);
    }

    private static bool IsRefreshTokenValid(User user, TokenDto token)
    {
        return user.RefreshToken.Token != null &&
               user.RefreshToken.Token.Equals(token.RefreshToken.Token) &&
               user.RefreshToken.ExpiryTime > DateTime.UtcNow;
    }
}