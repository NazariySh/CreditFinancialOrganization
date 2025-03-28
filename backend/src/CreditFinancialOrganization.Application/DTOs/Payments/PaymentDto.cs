using CreditFinancialOrganization.Application.DTOs.Loans;
using CreditFinancialOrganization.Domain.Enums;

namespace CreditFinancialOrganization.Application.DTOs.Payments;

public class PaymentDto
{
    public Guid Id { get; set; }
    public LoanDto Loan { get; set; } = new();
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
}