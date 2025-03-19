using CreditFinancialOrganization.Domain.Enums;
using CreditFinancialOrganization.Domain.Repositories;
using CreditFinancialOrganization.Infrastructure.Data;
using CreditFinancialOrganization.Domain.Entities.Loans;

namespace CreditFinancialOrganization.Infrastructure.Repositories;

public class LoanRepository : BaseRepository<Loan>, ILoanRepository
{
    public LoanRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public void UpdateStatus(Guid id, LoanStatus status)
    {
        var loan = new Loan
        {
            Id = id,
            Status = status
        };

        DbContext.Loans.Attach(loan);
        DbContext.Entry(loan).Property(x => x.Status).IsModified = true;
    }
}