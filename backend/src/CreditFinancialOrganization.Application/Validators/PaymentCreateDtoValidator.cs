using CreditFinancialOrganization.Application.DTOs.Payments;
using FluentValidation;

namespace CreditFinancialOrganization.Application.Validators;

public class PaymentCreateDtoValidator : AbstractValidator<PaymentCreateDto>
{
    public const int MinAmount = 100;
    public const int MaxAmount = 1_000_000;

    public PaymentCreateDtoValidator()
    {
        RuleFor(x => x.LoanId)
            .NotEmpty().WithMessage("Loan ID is required.");

        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage("Amount is required.")
            .InclusiveBetween(MinAmount, MaxAmount).WithMessage($"Amount must be between {MinAmount} and {MaxAmount}.");

        RuleFor(x => x.PaymentMethod)
            .IsInEnum().WithMessage("Invalid payment method.");
    }
}