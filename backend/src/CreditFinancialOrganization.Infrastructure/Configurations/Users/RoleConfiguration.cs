using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CreditFinancialOrganization.Domain.Enums;

namespace CreditFinancialOrganization.Infrastructure.Configurations.Users;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<IdentityRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
    {
        var roles = new List<IdentityRole<Guid>>
        {
            new()
            {
                Id = Guid.Parse("f4a3d9b2-28b9-4670-b7f5-07b6e5cf107f"),
                Name = RoleType.Customer.ToString(),
                NormalizedName = RoleType.Customer.ToString().ToUpper()
            },
            new()
            {
                Id = Guid.Parse("a1b9c8f0-1d2e-43b2-b3d2-3c30b354697d"),
                Name = RoleType.Employee.ToString(),
                NormalizedName = RoleType.Employee.ToString().ToUpper()
            },
            new()
            {
                Id = Guid.Parse("b3e1a5f0-9d4c-4c57-95c4-7a8f2b6d9e3f"),
                Name = RoleType.Admin.ToString(),
                NormalizedName = RoleType.Admin.ToString().ToUpper()
            }
        };

        builder.HasData(roles);
    }
}