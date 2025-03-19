using CreditFinancialOrganization.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore.Query;

namespace CreditFinancialOrganization.Domain.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(
        string email,
        Func<IQueryable<User>, IIncludableQueryable<User, object>>? include = default,
        CancellationToken cancellationToken = default);

    Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default);
}