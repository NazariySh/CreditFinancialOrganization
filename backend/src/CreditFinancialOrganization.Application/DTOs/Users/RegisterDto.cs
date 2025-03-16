namespace CreditFinancialOrganization.Application.DTOs.Users;

public record RegisterDto(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password);