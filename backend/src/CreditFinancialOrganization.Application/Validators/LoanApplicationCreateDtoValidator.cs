using CreditFinancialOrganization.Application.DTOs.Loans;
using FluentValidation;

namespace CreditFinancialOrganization.Application.Validators;

public class LoanApplicationCreateDtoValidator : AbstractValidator<LoanApplicationCreateDto>
{
    public const int MinAmount = 100;
    public const int MaxAmount = 1_000_000;
    public const int MinInterestRate = 0;
    public const int MaxInterestRate = 100;
    public const int MinLoanTermInMonths = 1;
    public const int MaxLoanTermInMonths = 360;

    public LoanApplicationCreateDtoValidator()
    {
        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage("Amount is required.")
            .InclusiveBetween(MinAmount, MaxAmount).WithMessage($"Amount must be between {MinAmount} and {MaxAmount}.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.")
            .Must(BeAValidStartDate).WithMessage("Start date must be a valid date.");

        RuleFor(x => x.LoanTypeId)
            .NotEmpty().WithMessage("Loan type is required.");

        RuleFor(x => x.InterestRate)
            .NotEmpty().WithMessage("Interest rate is required.")
            .InclusiveBetween(MinInterestRate, MaxInterestRate).WithMessage($"Interest rate must be between {MinInterestRate} and {MaxInterestRate}.");

        RuleFor(x => x.LoanTermInMonths)
            .NotEmpty().WithMessage("Loan term in months is required.")
            .InclusiveBetween(MinLoanTermInMonths, MaxLoanTermInMonths).WithMessage($"Loan term must be between {MinLoanTermInMonths} and {MaxLoanTermInMonths} months.");
    }

    private static bool BeAValidStartDate(DateTime startDate)
    {
        return startDate > DateTime.MinValue && startDate <= DateTime.UtcNow;
    }
}