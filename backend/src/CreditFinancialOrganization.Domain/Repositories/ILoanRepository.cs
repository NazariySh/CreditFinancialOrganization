using CreditFinancialOrganization.Domain.Entities.Loans;
using CreditFinancialOrganization.Domain.Enums;

namespace CreditFinancialOrganization.Domain.Repositories;

public interface ILoanRepository : IRepository<Loan>
{
    void UpdateStatus(Guid id, LoanStatus status);
}