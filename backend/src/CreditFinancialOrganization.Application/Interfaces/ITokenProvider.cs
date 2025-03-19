using CreditFinancialOrganization.Application.DTOs.Auth;
using CreditFinancialOrganization.Domain.Entities.Users;
using System.Security.Claims;

namespace CreditFinancialOrganization.Application.Interfaces;

public interface ITokenProvider
{
    string GenerateAccessToken(User user, IEnumerable<string> roles);
    RefreshTokenDto GenerateRefreshToken();
    Task<ClaimsPrincipal> GetPrincipalAsync(string accessToken);
}