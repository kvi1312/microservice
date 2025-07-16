using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.Entities.Configuration;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(new IdentityRole
        {
            Name = "administrator",
            NormalizedName = "ADMINISTRATOR",
            Id = Guid.NewGuid().ToString(),
        });

        builder.HasData(new IdentityRole
        {
            Name = "customer",
            NormalizedName = "CUSTOMER",
            Id = Guid.NewGuid().ToString(),
        });
    }
}
