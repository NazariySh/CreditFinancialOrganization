using CreditFinancialOrganization.Domain.Entities.Loans;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CreditFinancialOrganization.Infrastructure.Configurations.Loans;

internal sealed class LoanTypeConfiguration : IEntityTypeConfiguration<LoanType>
{
    public void Configure(EntityTypeBuilder<LoanType> builder)
    {
        builder.ToTable("LoanTypes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.InterestRate)
            .HasColumnType("decimal(5,2)")
            .IsRequired();
    }
}