using CreditFinancialOrganization.Domain.Entities.Loans;
using CreditFinancialOrganization.Domain.Enums;

namespace CreditFinancialOrganization.Domain.Entities.Payments;

public class Payment : BaseEntity
{
    public Guid LoanId { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public PaymentMethod PaymentMethod { get; set; }

    public Loan Loan { get; set; } = null!;
}