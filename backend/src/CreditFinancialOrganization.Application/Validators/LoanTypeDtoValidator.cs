using CreditFinancialOrganization.Application.DTOs.Loans;
using FluentValidation;
using System.Text.RegularExpressions;

namespace CreditFinancialOrganization.Application.Validators;

public partial class LoanTypeDtoValidator : AbstractValidator<LoanTypeDto>
{
    public LoanTypeDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(50).WithMessage("Name can't be longer than 50 characters")
            .MinimumLength(2).WithMessage("Name can't be shorter than 2 characters")
            .Matches(CapitalLetterPattern()).WithMessage("Name start with a capital letter")
            .Matches(NamePattern()).WithMessage("Name contains invalid characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description can't exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.InterestRate)
            .InclusiveBetween(0, 100).WithMessage("Interest rate must be between 0 and 100");
    }

    [GeneratedRegex("^[A-ZА-Я].*")]
    private partial Regex CapitalLetterPattern();

    [GeneratedRegex(@"^[\p{L}\s\-]+$")]
    private partial Regex NamePattern();
}