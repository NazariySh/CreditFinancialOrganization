namespace CreditFinancialOrganization.Domain.Models;

public record ValidationErrorResponse(int StatusCode, Dictionary<string, string> Errors)
    : ErrorResponse(StatusCode, "Validation errors");