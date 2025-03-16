using CreditFinancialOrganization.Domain.Entities.Users;
using CreditFinancialOrganization.Domain.Enums;

namespace CreditFinancialOrganization.Domain.Entities.Loans;

public class Loan : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Guid LoanTypeId { get; set; }
    public LoanStatus Status { get; set; }
    public decimal Amount { get; set; }
    public decimal InterestRate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public User Customer { get; set; } = null!;
    public LoanType LoanType { get; set; } = null!;
    public LoanApplication Application { get; set; } = null!;
}