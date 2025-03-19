using CreditFinancialOrganization.Domain.Entities.Loans;
using CreditFinancialOrganization.Domain.Enums;
using CreditFinancialOrganization.Domain.Repositories;
using CreditFinancialOrganization.Infrastructure.Data;

namespace CreditFinancialOrganization.Infrastructure.Repositories;

public class LoanApplicationRepository : BaseRepository<LoanApplication>, ILoanApplicationRepository
{
    public LoanApplicationRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public void UpdateStatus(Guid id, ApplicationStatus status)
    {
        var application = new LoanApplication
        {
            Id = id,
            Status = status
        };

        DbContext.LoanApplications.Attach(application);
        DbContext.Entry(application).Property(x => x.Status).IsModified = true;
    }
}