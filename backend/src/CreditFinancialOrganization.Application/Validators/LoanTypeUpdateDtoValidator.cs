using CreditFinancialOrganization.Application.DTOs.Loans;
using FluentValidation;

namespace CreditFinancialOrganization.Application.Validators;

public class LoanTypeUpdateDtoValidator : AbstractValidator<LoanTypeUpdateDto>
{
    public LoanTypeUpdateDtoValidator(IValidator<LoanTypeCreateDto> validator)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");

        Include(validator);
    }
}