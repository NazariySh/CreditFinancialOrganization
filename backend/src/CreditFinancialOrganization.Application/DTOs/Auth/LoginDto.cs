namespace CreditFinancialOrganization.Application.DTOs.Auth;

public record LoginDto(
    string Email,
    string Password);