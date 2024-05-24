using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sterling.Gateway.Domain;

namespace Sterling.Gateway.Data;

public class ApplicationDbContext : IdentityDbContext<GatewayApplication>
{
    public DbSet<MicroService> MicroServices { get; set; }
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("Gateway");
        base.OnModelCreating(builder);
    }
}
