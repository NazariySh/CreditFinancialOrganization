using CreditFinancialOrganization.Application.DTOs.Users;
using CreditFinancialOrganization.Domain.Enums;

namespace CreditFinancialOrganization.Application.DTOs.Loans;

public class LoanDto
{
    public Guid Id { get; set; }
    public UserDto Customer { get; set; } = new();
    public LoanTypeDto LoanType { get; set; } = new();
    public LoanStatus Status { get; set; }
    public decimal Amount { get; set; }
    public decimal InterestRate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}