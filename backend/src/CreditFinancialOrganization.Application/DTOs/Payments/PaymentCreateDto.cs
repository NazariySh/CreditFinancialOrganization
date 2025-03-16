using CreditFinancialOrganization.Domain.Enums;

namespace CreditFinancialOrganization.Application.DTOs.Payments;

public record PaymentCreateDto(
    Guid LoanId,
    decimal Amount,
    PaymentMethod PaymentMethod);