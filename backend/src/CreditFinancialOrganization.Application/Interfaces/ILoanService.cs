using CreditFinancialOrganization.Application.DTOs.Loans;
using CreditFinancialOrganization.Domain.Models;

namespace CreditFinancialOrganization.Application.Interfaces;

public interface ILoanService
{
    Task DeleteAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<LoanDto?> GetByIdAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<PagedList<LoanDto>> GetAllAsync(
        Guid userId,
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}