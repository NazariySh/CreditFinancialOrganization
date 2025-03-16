using CreditFinancialOrganization.Domain.Entities.Loans;
using CreditFinancialOrganization.Domain.Enums;

namespace CreditFinancialOrganization.Domain.Repositories;

public interface ILoanApplicationRepository : IRepository<LoanApplication>
{
    void UpdateStatus(Guid id, ApplicationStatus status);
}