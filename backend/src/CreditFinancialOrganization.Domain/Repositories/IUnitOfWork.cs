namespace CreditFinancialOrganization.Domain.Repositories;

public interface IUnitOfWork
{
    ILoanApplicationRepository LoanApplications { get; }
    ILoanRepository Loans { get; }
    ILoanTypeRepository LoanTypes { get; }
    IPaymentRepository Payments { get; }
    IUserRepository Users { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    IAddressRepository Addresses { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}