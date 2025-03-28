using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using CreditFinancialOrganization.Domain.Repositories;
using CreditFinancialOrganization.Infrastructure.Data;
using CreditFinancialOrganization.Domain.Entities.Users;

namespace CreditFinancialOrganization.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<User?> GetByEmailAsync(
        string email,
        Func<IQueryable<User>, IIncludableQueryable<User, object>>? include = null,
        CancellationToken cancellationToken = default)
    {
        return GetSingleAsync(
            x => x.Email != null && x.Email.ToLower() == email.ToLower(),
            include,
            cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default)
    {
        return !await DbContext.Users.AnyAsync(
            x => x.Email != null && x.Email.ToLower() == email.ToLower(),
            cancellationToken);
    }
}