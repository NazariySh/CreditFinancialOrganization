using CreditFinancialOrganization.Application.DTOs.Loans;
using CreditFinancialOrganization.Domain.Enums;
using CreditFinancialOrganization.Domain.Models;

namespace CreditFinancialOrganization.Application.Interfaces;

public interface ILoanApplicationService
{
    Task CreateAsync(
        LoanApplicationCreateDto dto,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task UpdateStatusAsync(
        Guid id,
        ApplicationStatus status,
        CancellationToken cancellationToken = default);

    Task<PagedList<LoanApplicationDto>> GetAllAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}