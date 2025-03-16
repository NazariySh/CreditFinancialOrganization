using CreditFinancialOrganization.Domain.Entities.Loans;
using CreditFinancialOrganization.Domain.Entities.Payments;
using CreditFinancialOrganization.Domain.Entities.Users;
using CreditFinancialOrganization.Infrastructure.Configurations.Loans;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CreditFinancialOrganization.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<CreditScore> CreditScores { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<LoanApplication> LoanApplications { get; set; }
    public DbSet<LoanType> LoanTypes { get; set; }
    public DbSet<Payment> Payments { get; set; }

    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LoanConfiguration).Assembly);
    }
}