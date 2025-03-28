using CreditFinancialOrganization.Application.DTOs.Users;
using FluentValidation;

namespace CreditFinancialOrganization.Application.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public const int MinFirstNameLength = 100;
    public const int MinLastNameLength = 100;
    public const int MinPasswordLength = 8;

    public RegisterDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First Name is required.")
            .MaximumLength(MinFirstNameLength).WithMessage($"First Name cannot exceed {MinFirstNameLength} characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last Name is required.")
            .MaximumLength(MinLastNameLength).WithMessage($"Last Name cannot exceed {MinLastNameLength} characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?\d{7,21}$").WithMessage("Invalid phone number format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(MinPasswordLength).WithMessage($"Password must be at least {MinPasswordLength} characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"\d").WithMessage("Password must contain at least one number.")
            .Matches(@"[\W]").WithMessage("Password must contain at least one special character.");
    }
}