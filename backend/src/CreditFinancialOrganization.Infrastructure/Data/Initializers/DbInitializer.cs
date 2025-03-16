using CreditFinancialOrganization.Domain.Entities.Loans;
using CreditFinancialOrganization.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json;
using CreditFinancialOrganization.Application.DTOs.Users;
using CreditFinancialOrganization.Application.Interfaces;

namespace CreditFinancialOrganization.Infrastructure.Data.Initializers;

public class DbInitializer : IDbInitializer
{
    private const string AdminEmail = "admin@gmail.com";
    private const string AdminPassword = "Admin123*";
    private const string EmployeeEmail = "alice.smith@company.com";
    private const string EmployeePassword = "AliceSecure123!";

    private readonly ApplicationDbContext _dbContext;
    private readonly IUserService _userService;
    private readonly ILogger<DbInitializer> _logger;

    public DbInitializer(
        ApplicationDbContext dbContext,
        IUserService userService,
        ILogger<DbInitializer> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await ApplyMigrationsAsync(cancellationToken);

        await SeedAdminAsync(cancellationToken);
        await SeedEmployeeAsync(cancellationToken);
        await SeedLoanTypesAsync(cancellationToken);
    }

    private async Task ApplyMigrationsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var migrations = await _dbContext.Database.GetPendingMigrationsAsync(cancellationToken);

            if (migrations.Any())
            {
                await _dbContext.Database.MigrateAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }

    private async Task SeedLoanTypesAsync(CancellationToken cancellationToken)
    {
        if (await _dbContext.LoanTypes.AnyAsync(cancellationToken))
        {
            return;
        }

        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var loanTypesData = await File.ReadAllTextAsync(
            $"{path}/Data/Initializers/SeedData/loan-types.json",
            cancellationToken);
        var loanTypes = JsonSerializer.Deserialize<List<LoanType>>(loanTypesData);

        if (loanTypes == null) return;

        _dbContext.LoanTypes.AddRange(loanTypes);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedAdminAsync(CancellationToken cancellationToken)
    {
        if (await AnyUserAsync(AdminEmail, cancellationToken))
        {
            return;
        }

        var registerDto = new RegisterDto("Admin",
            "Admin",
            AdminEmail,
            "+380123456789",
            AdminPassword);

        await _userService.CreateAsync(registerDto, RoleType.Admin, cancellationToken);
    }

    private async Task SeedEmployeeAsync(CancellationToken cancellationToken)
    {
        if (await AnyUserAsync(EmployeeEmail, cancellationToken))
        {
            return;
        }

        var registerDto = new RegisterDto("Alice",
            "Smith",
            EmployeeEmail,
            "+1987654321",
            EmployeePassword);

        await _userService.CreateAsync(registerDto, RoleType.Employee, cancellationToken);
    }

    private async Task<bool> AnyUserAsync(string userName, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.AnyAsync(x => x.UserName == userName, cancellationToken);
    }
}