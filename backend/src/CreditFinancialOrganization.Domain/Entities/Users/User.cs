using Microsoft.AspNetCore.Identity;

namespace CreditFinancialOrganization.Domain.Entities.Users;

public class User : IdentityUser<Guid>, IEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime? BirthDate { get; set; }
    public CreditScore? CreditScore { get; set; }
    public Address? Address { get; set; }

    public RefreshToken RefreshToken { get; set; } = null!;
}