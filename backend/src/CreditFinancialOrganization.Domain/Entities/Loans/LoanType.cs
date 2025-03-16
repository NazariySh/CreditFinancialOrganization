namespace CreditFinancialOrganization.Domain.Entities.Loans;

public class LoanType : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal InterestRate { get; set; }
}