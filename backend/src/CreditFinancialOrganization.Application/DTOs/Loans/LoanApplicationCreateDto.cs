namespace CreditFinancialOrganization.Application.DTOs.Loans;

public record LoanApplicationCreateDto(
    decimal Amount,
    DateTime StartDate,
    Guid LoanTypeId,
    decimal InterestRate,
    int LoanTermInMonths);