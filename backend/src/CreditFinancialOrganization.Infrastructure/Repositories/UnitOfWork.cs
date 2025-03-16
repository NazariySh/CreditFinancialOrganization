using CreditFinancialOrganization.Domain.Entities.Users;
using CreditFinancialOrganization.Domain.Repositories;
using CreditFinancialOrganization.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace CreditFinancialOrganization.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    private ILoanApplicationRepository? _loanApplicationRepository;
    private ILoanRepository? _loanRepository;
    private ILoanTypeRepository? _loanTypeRepository;
    private IPaymentRepository? _paymentRepository;
    private IUserRepository? _userRepository;
    private IRefreshTokenRepository? _refreshTokenRepository;
    private IAddressRepository? _addressRepository;

    public ILoanApplicationRepository LoanApplications =>
        _loanApplicationRepository ??= new LoanApplicationRepository(_dbContext);

    public ILoanRepository Loans =>
        _loanRepository ??= new LoanRepository(_dbContext);

    public ILoanTypeRepository LoanTypes =>
        _loanTypeRepository ??= new LoanTypeRepository(_dbContext);

    public IPaymentRepository Payments =>
        _paymentRepository ??= new PaymentRepository(_dbContext);

    public IUserRepository Users =>
        _userRepository ??= new UserRepository(_dbContext);

    public IRefreshTokenRepository RefreshTokens =>
        _refreshTokenRepository ??= new RefreshTokenRepository(_dbContext);

    public IAddressRepository Addresses =>
        _addressRepository ??= new AddressRepository(_dbContext);
        

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}