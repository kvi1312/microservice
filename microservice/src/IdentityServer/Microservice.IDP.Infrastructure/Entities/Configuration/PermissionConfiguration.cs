using Microservice.IDP.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservice.IDP.Infrastructure.Entities.Configuration;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions", SystemConstants.IdentitySchema)
            .HasKey(p => p.Id);;

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.HasIndex(c => new { c.RoleId, c.Function, c.Command })
            .IsUnique();
    }
}