using System.Text;
using CreditFinancialOrganization.Application.Interfaces;
using CreditFinancialOrganization.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using CreditFinancialOrganization.Domain.Entities.Users;
using CreditFinancialOrganization.Domain.Repositories;
using CreditFinancialOrganization.Domain.Settings;
using CreditFinancialOrganization.Infrastructure.Data;
using CreditFinancialOrganization.Infrastructure.Data.Initializers;
using CreditFinancialOrganization.Infrastructure.Services;

namespace CreditFinancialOrganization.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));

        services.AddIdentity();
        services.AddAuthentication(configuration);
        services.AddAuthorization();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IDbInitializer, DbInitializer>();

        services.AddScoped<ITokenProvider, JwtTokenProvider>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ILoanApplicationRepository, LoanApplicationRepository>();
        services.AddScoped<ILoanRepository, LoanRepository>();
        services.AddScoped<ILoanTypeRepository, LoanTypeRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    private static void AddIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.User.RequireUniqueEmail = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
    }

    private static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(nameof(JwtSettings));

        var key = jwtSettings["Key"]
                  ?? throw new ArgumentException("Key is missing in JwtSettings section");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };
        });
    }
}