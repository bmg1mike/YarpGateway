using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sterling.Gateway.Domain;

namespace Sterling.Gateway.Data;

public class ApplicationDbContext : IdentityDbContext<GatewayApplication>
{
    public DbSet<MicroService> MicroServices { get; set; }
    public DbSet<RouteConfigEntity> RouteConfigs { get; set; }
    public DbSet<ClusterConfigEntity> ClusterConfigs { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("Gateway");
        base.OnModelCreating(builder);

        builder.Entity<RouteConfigEntity>().ToTable("RouteConfigs");
        builder.Entity<ClusterConfigEntity>().ToTable("ClusterConfigs");
    }
}
