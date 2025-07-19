using Microservice.IDP.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace IdentityServer.Persistence;

public class IdentityContext : IdentityDbContext<User>
{
    public IDbConnection Connection => Database.GetDbConnection();
    public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
    {
    }

    public DbSet<Permission> Permissions { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        //builder.ApplyConfiguration(new RoleConfiguration());
        builder.ApplyConfigurationsFromAssembly(typeof(IdentityContext).Assembly);
        builder.ApplyIdentityConfiguration();
    }
}
