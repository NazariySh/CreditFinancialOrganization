using CreditFinancialOrganization.Application.DTOs.Payments;
using CreditFinancialOrganization.Domain.Models;

namespace CreditFinancialOrganization.Application.Interfaces;

public interface IPaymentService
{
    Task CreateAsync(PaymentCreateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaymentDto?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<PagedList<PaymentDto>> GetAllAsync(
        Guid loanId,
        Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}