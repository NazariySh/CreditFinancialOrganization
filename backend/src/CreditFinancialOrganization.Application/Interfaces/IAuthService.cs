using CreditFinancialOrganization.Application.DTOs.Auth;

namespace CreditFinancialOrganization.Application.Interfaces;

public interface IAuthService
{
    Task<TokenDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default);
    Task LogoutAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<TokenDto> RefreshTokenAsync(TokenDto token, CancellationToken cancellationToken = default);
}