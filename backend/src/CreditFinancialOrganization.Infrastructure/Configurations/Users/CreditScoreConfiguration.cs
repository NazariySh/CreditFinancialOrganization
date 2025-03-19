using CreditFinancialOrganization.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CreditFinancialOrganization.Infrastructure.Configurations.Users;

internal sealed class CreditScoreConfiguration : IEntityTypeConfiguration<CreditScore>
{
    public void Configure(EntityTypeBuilder<CreditScore> builder)
    {
        builder.ToTable("CreditScores");

        builder.HasKey(x => x.CustomerId);

        builder.Property(x => x.CustomerId)
            .ValueGeneratedNever();

        builder.Property(x => x.Score)
            .IsRequired();

        builder.Property(x => x.RatingDate)
            .IsRequired();
    }
}