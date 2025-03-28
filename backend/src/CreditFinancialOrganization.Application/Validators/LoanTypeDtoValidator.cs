using CreditFinancialOrganization.Application.DTOs.Loans;
using FluentValidation;
using System.Text.RegularExpressions;

namespace CreditFinancialOrganization.Application.Validators;

public partial class LoanTypeDtoValidator : AbstractValidator<LoanTypeCreateDto>
{
    public const int MinNameLength = 2;
    public const int MaxNameLength = 50;
    public const int MaxDescriptionLength = 500;
    public const int MinInterestRate = 0;
    public const int MaxInterestRate = 100;

    public LoanTypeDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(MaxNameLength).WithMessage($"Name can't be longer than {MaxNameLength} characters")
            .MinimumLength(MinNameLength).WithMessage($"Name can't be shorter than {MinNameLength} characters")
            .Matches(CapitalLetterPattern()).WithMessage("Name start with a capital letter")
            .Matches(NamePattern()).WithMessage("Name contains invalid characters.");

        RuleFor(x => x.Description)
            .MaximumLength(MaxDescriptionLength).WithMessage($"Description can't exceed {MaxDescriptionLength} characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.InterestRate)
            .InclusiveBetween(MinInterestRate, MaxInterestRate).WithMessage($"Interest rate must be between {MinInterestRate} and {MaxInterestRate}");
    }

    [GeneratedRegex("^[A-ZА-Я].*")]
    private partial Regex CapitalLetterPattern();

    [GeneratedRegex(@"^[\p{L}\s\-]+$")]
    private partial Regex NamePattern();
}