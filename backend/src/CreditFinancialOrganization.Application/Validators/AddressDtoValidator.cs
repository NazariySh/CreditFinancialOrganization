using CreditFinancialOrganization.Application.DTOs.Users;
using FluentValidation;

namespace CreditFinancialOrganization.Application.Validators;

public class AddressDtoValidator : AbstractValidator<AddressDto>
{
    public AddressDtoValidator()
    {
        RuleFor(x => x.Line)
            .NotEmpty().WithMessage("Address Line is required.")
            .MaximumLength(200).WithMessage("Address Line cannot exceed 200 characters.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(100).WithMessage("City cannot exceed 100 characters.");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required.")
            .MaximumLength(100).WithMessage("State cannot exceed 100 characters.");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required.")
            .MaximumLength(100).WithMessage("Country cannot exceed 100 characters.");

        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Postal Code is required.")
            .MaximumLength(20).WithMessage("Postal Code cannot exceed 20 characters.");
    }
}