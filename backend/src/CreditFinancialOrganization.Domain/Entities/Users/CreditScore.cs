namespace CreditFinancialOrganization.Domain.Entities.Users;

public class CreditScore
{
    public Guid CustomerId { get; set; }
    public int Score { get; set; }
    public DateTime RatingDate { get; set; }
}