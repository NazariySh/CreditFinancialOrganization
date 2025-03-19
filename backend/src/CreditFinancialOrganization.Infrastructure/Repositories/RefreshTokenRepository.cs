using CreditFinancialOrganization.Domain.Entities.Users;
using CreditFinancialOrganization.Domain.Repositories;
using CreditFinancialOrganization.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditFinancialOrganization.Infrastructure.Repositories;

public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }
    public async Task UpdateTokenAsync(
        Guid userId,
        string? refreshToken,
        DateTime? refreshTokenExpiryTime,
        bool populateExp = true,
        CancellationToken cancellationToken = default)
    {
        await DbContext.RefreshTokens
            .Where(x => x.UserId == userId)
            .ExecuteUpdateAsync(x =>
                populateExp
                    ? x.SetProperty(u => u.Token, refreshToken)
                        .SetProperty(u => u.ExpiryTime, refreshTokenExpiryTime)
                    : x.SetProperty(u => u.Token, refreshToken),
                cancellationToken);
    }
}