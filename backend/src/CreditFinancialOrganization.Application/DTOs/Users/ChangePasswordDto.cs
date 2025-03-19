namespace CreditFinancialOrganization.Application.DTOs.Users;

public record ChangePasswordDto(
    string CurrentPassword,
    string NewPassword);