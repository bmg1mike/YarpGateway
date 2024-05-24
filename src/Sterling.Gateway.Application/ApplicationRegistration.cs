using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Sterling.Gateway.Data;
using Sterling.Gateway.Domain;

namespace Sterling.Gateway.Application;

public static class ApplicationRegistration
{
    public static IServiceCollection AddApplicationRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfig:Secret"]!));
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = "https://localhost:7087",
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            opt.Authority = "https://localhost:7087";
        });
        services.AddAuthorization(options =>
        {
            options.AddPolicy("GeneralService", policy =>
                policy.RequireAuthenticatedUser());
        });

        services.AddScoped<IProfileManagementService, ProfileManagementService>();
        services.AddScoped<IEndpointProfilingService, EndpointProfilingService>();
        services.AddDbContext<ApplicationDbContext>(x => x.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        services.AddIdentity<GatewayApplication, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

        return services;
    }
}