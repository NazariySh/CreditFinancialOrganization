namespace CreditFinancialOrganization.Domain.Entities.Users;

public class RefreshToken
{
    public Guid UserId { get; set; }
    public string? Token { get; set; }
    public DateTime? ExpiryTime { get; set; }
}