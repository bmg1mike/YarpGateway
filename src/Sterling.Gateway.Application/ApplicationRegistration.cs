using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Sterling.Gateway.Data;
using Sterling.Gateway.Domain;
using Yarp.ReverseProxy.Configuration;

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
                ValidIssuer = configuration["JwtConfig:Issuer"],
                ValidAudience = configuration["JwtConfig:Issuer"],
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            opt.Authority = configuration["JwtConfig:Issuer"];
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("SuperAdminPolicy", policy =>
                policy.RequireAssertion(x => x.User.HasClaim(claim => 
                claim.Type =="Permission" &&
                (claim.Value == "SuperAdmin"))));

            options.AddPolicy("AdminPolicy", policy =>
                policy.RequireAssertion(x => x.User.HasClaim(claim => 
                claim.Type =="Permission" &&
                (claim.Value == "SuperAdmin" || claim.Value == "Admin"))));

            options.AddPolicy("GuestPolicy", policy =>
            policy.RequireAssertion(context =>
                context.User.HasClaim(claim =>
                claim.Type == "Permission" &&
                (claim.Value == "SuperAdmin" || claim.Value == "Admin" || claim.Value == "Guest"))
    )
);

        });

        services.AddSingleton<IProxyConfigProvider, CustomProxyConfigProvider>();
        services.AddSingleton<CustomProxyConfigProvider>();

        // Add YARP
        services.AddReverseProxy();

        services.AddScoped<IProfileManagementService, ProfileManagementService>();
        services.AddScoped<IEndpointProfilingService, EndpointProfilingService>();
        services.AddScoped<IRedisRepository, RedisRepository>();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("RedisConnection");
        });
        services.AddDbContext<ApplicationDbContext>(x => x.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null);
            npgsqlOptions.CommandTimeout(60);
        }));
        services.AddIdentityCore<GatewayApplication>().AddEntityFrameworkStores<ApplicationDbContext>().AddApiEndpoints();

        services.AddHostedService<ConfigReloadService>();

        return services;
    }
}