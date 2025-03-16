using CreditFinancialOrganization.Application.DTOs.Loans;
using FluentValidation;

namespace CreditFinancialOrganization.Application.Validators;

public class LoanApplicationCreateDtoValidator : AbstractValidator<LoanApplicationCreateDto>
{
    public LoanApplicationCreateDtoValidator()
    {
        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage("Amount is required.")
            .InclusiveBetween(100.00m, 1_000_000.00m).WithMessage("Amount must be between 100 and 1,000,000.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.")
            .Must(BeAValidStartDate).WithMessage("Start date must be a valid date.");

        RuleFor(x => x.LoanTypeId)
            .NotEmpty().WithMessage("Loan type is required.");

        RuleFor(x => x.InterestRate)
            .NotEmpty().WithMessage("Interest rate is required.")
            .InclusiveBetween(0, 100).WithMessage("Interest rate must be between 0 and 100.");

        RuleFor(x => x.LoanTermInMonths)
            .NotEmpty().WithMessage("Loan term in months is required.")
            .InclusiveBetween(1, 360).WithMessage("Loan term must be between 1 and 360 months.");
    }

    private static bool BeAValidStartDate(DateTime startDate)
    {
        return startDate > DateTime.MinValue && startDate <= DateTime.UtcNow;
    }
}