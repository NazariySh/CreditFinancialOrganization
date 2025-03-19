using CreditFinancialOrganization.Application.DTOs.Payments;
using FluentValidation;

namespace CreditFinancialOrganization.Application.Validators;

public class PaymentCreateDtoValidator : AbstractValidator<PaymentCreateDto>
{
    public PaymentCreateDtoValidator()
    {
        RuleFor(x => x.LoanId)
            .NotEmpty().WithMessage("Loan ID is required.");

        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage("Amount is required.")
            .InclusiveBetween(100.00m, 1_000_000.00m).WithMessage("Amount must be between 100 and 1,000,000.");

        RuleFor(x => x.PaymentMethod)
            .IsInEnum().WithMessage("Invalid payment method.");
    }
}