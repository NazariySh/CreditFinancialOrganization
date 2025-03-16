namespace CreditFinancialOrganization.Domain.Entities.Users;

public class Address
{
    public Guid CustomerId { get; set; }
    public string Line { get; set; } = null!;
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
}