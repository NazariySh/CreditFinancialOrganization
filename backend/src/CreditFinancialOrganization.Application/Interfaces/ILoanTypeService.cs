using CreditFinancialOrganization.Application.DTOs.Loans;

namespace CreditFinancialOrganization.Application.Interfaces;

public interface ILoanTypeService
{
    Task CreateAsync(LoanTypeCreateDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, LoanTypeUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<LoanTypeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LoanTypeDto>> GetAllAsync(CancellationToken cancellationToken = default);
}