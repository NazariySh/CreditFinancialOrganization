using CreditFinancialOrganization.Domain.Entities.Users;

namespace CreditFinancialOrganization.Application.DTOs.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public Address? Address { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = [];

    public RefreshToken? RefreshToken { get; set; }
}