using CreditFinancialOrganization.Domain.Entities.Loans;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CreditFinancialOrganization.Infrastructure.Configurations.Loans;

internal sealed class LoanApplicationConfiguration : IEntityTypeConfiguration<LoanApplication>
{
    public void Configure(EntityTypeBuilder<LoanApplication> builder)
    {
        builder.ToTable("LoanApplications");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.Date)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(e => e.ApprovalDate)
            .HasColumnType("date");

        builder.HasOne(x => x.Loan)
            .WithOne(l => l.Application)
            .HasForeignKey<Loan>(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}