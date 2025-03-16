using CreditFinancialOrganization.Domain.Entities.Loans;
using CreditFinancialOrganization.Domain.Enums;

namespace CreditFinancialOrganization.Application.DTOs.Payments;

public class PaymentDto
{
    public Guid Id { get; set; }
    public Loan Loan { get; set; } = new();
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
}