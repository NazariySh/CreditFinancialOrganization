using CreditFinancialOrganization.Domain.Entities.Loans;
using CreditFinancialOrganization.Domain.Repositories;
using CreditFinancialOrganization.Infrastructure.Data;

namespace CreditFinancialOrganization.Infrastructure.Repositories;

public class LoanTypeRepository : BaseRepository<LoanType>, ILoanTypeRepository
{
    public LoanTypeRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }
}