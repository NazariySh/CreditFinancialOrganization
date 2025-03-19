using CreditFinancialOrganization.Domain.Entities.Loans;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CreditFinancialOrganization.Infrastructure.Configurations.Loans;

internal sealed class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable("Loans");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.InterestRate)
            .HasColumnType("decimal(5,2)")
            .IsRequired();

        builder.Property(x => x.StartDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.EndDate)
            .HasColumnType("date")
            .IsRequired();

        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.LoanType)
            .WithMany()
            .HasForeignKey(x => x.LoanTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Application)
            .WithOne(la => la.Loan)
            .HasForeignKey<LoanApplication>(x => x.Id)
            .OnDelete(DeleteBehavior.NoAction);
    }
}