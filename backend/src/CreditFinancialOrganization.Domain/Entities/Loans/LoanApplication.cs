using CreditFinancialOrganization.Domain.Entities.Users;
using CreditFinancialOrganization.Domain.Enums;

namespace CreditFinancialOrganization.Domain.Entities.Loans;

public class LoanApplication : BaseEntity
{
    public DateTime Date { get; set; }
    public ApplicationStatus Status { get; set; }
    public DateOnly? ApprovalDate { get; set; }
    public Guid? EmployeeId { get; set; }

    public Loan Loan { get; set; } = null!;
    public User? Employee { get; set; }
}