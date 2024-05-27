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
                ValidateIssuer = false,
                // ValidIssuer = "https://localhost:7087",
                ValidAudience = "https://localhost:7087",
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            opt.Authority = "https://localhost:7087";
        });
       
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminPolicy", policy =>
                policy.RequireAssertion(context => context.User.HasClaim("role", "admin") && context.User.HasClaim("permission", "ReadAndWrite")));

            // options.AddPolicy("GeneralPolicy", policy =>
            //     policy.RequireAssertion(context => context.User.HasClaim("permission", "ReadAndWrite")));

            // options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
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
        services.AddDbContext<ApplicationDbContext>(x => x.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        services.AddIdentityCore<GatewayApplication>().AddEntityFrameworkStores<ApplicationDbContext>().AddApiEndpoints();

        services.AddHostedService<ConfigReloadService>();

        return services;
    }
}