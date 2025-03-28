using CreditFinancialOrganization.Application.DTOs.Users;
using FluentValidation;

namespace CreditFinancialOrganization.Application.Validators;

public class AddressDtoValidator : AbstractValidator<AddressDto>
{
    public const int MinLineLength = 200;
    public const int MinCityLength = 100;
    public const int MinStateLength = 100;
    public const int MinCountryLength = 100;
    public const int MinPostalCodeLength = 20;

    public AddressDtoValidator()
    {
        RuleFor(x => x.Line)
            .NotEmpty().WithMessage("Address Line is required.")
            .MaximumLength(MinLineLength).WithMessage($"Address Line cannot exceed {MinLineLength} characters.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(MinCityLength).WithMessage($"City cannot exceed {MinCityLength} characters.");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required.")
            .MaximumLength(MinStateLength).WithMessage($"State cannot exceed {MinStateLength} characters.");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required.")
            .MaximumLength(MinCountryLength).WithMessage($"Country cannot exceed {MinCountryLength} characters.");

        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Postal Code is required.")
            .MaximumLength(MinPostalCodeLength).WithMessage($"Postal Code cannot exceed {MinPostalCodeLength} characters.");
    }
}