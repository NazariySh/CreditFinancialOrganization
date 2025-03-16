using CreditFinancialOrganization.Domain.Entities.Users;

namespace CreditFinancialOrganization.Domain.Repositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task UpdateTokenAsync(
        Guid userId,
        string? refreshToken,
        DateTime? refreshTokenExpiryTime,
        bool populateExp = true,
        CancellationToken cancellationToken = default);
}