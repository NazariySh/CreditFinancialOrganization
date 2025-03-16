using CreditFinancialOrganization.Application.DTOs.Users;
using CreditFinancialOrganization.Domain.Enums;

namespace CreditFinancialOrganization.Application.DTOs.Loans;

public class LoanApplicationDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public ApplicationStatus Status { get; set; }
    public DateOnly? ApprovalDate { get; set; }
    public LoanDto Loan { get; set; } = new();
    public UserDto? Employee { get; set; }
}