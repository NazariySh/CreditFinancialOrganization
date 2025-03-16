namespace CreditFinancialOrganization.Application.DTOs.Auth;

public record TokenDto(
    string AccessToken,
    RefreshTokenDto RefreshToken);