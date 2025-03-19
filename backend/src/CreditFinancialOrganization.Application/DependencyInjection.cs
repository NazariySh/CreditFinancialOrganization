using CreditFinancialOrganization.Application.Interfaces;
using CreditFinancialOrganization.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using CreditFinancialOrganization.Application.Validators;

namespace CreditFinancialOrganization.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        services.AddAutoMapper(currentAssemblies);

        services.AddValidatorsFromAssemblyContaining<LoanApplicationCreateDtoValidator>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ILoanApplicationService, LoanApplicationService>();
        services.AddScoped<ILoanService, LoanService>();
        services.AddScoped<ILoanTypeService, LoanTypeService>();
        services.AddScoped<IPaymentService, PaymentService>();

        return services;
    }
}