namespace CreditFinancialOrganization.Application.DTOs.Loans;

public class LoanTypeCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal InterestRate { get; set; }
}
