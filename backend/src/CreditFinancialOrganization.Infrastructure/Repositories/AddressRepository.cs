using CreditFinancialOrganization.Domain.Entities.Users;
using CreditFinancialOrganization.Domain.Repositories;
using CreditFinancialOrganization.Infrastructure.Data;

namespace CreditFinancialOrganization.Infrastructure.Repositories;

public class AddressRepository : BaseRepository<Address>, IAddressRepository
{
    public AddressRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }
}