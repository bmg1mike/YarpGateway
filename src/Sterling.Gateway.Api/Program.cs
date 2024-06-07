using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Sterling.Gateway.Data;
using Sterling.Gateway.Application;
using Sterling.Gateway.Api;
using System.Threading.RateLimiting;
using Sterling.Gateway.Domain;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider serviceProvider, LoggerConfiguration config) =>
{
    config.ReadFrom
        .Configuration(context.Configuration)
        .ReadFrom.Services(serviceProvider);
});

builder.Services.AddApplicationRegistration(builder.Configuration);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sterling Gateway", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Jwt auth header",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });
builder.Services.Configure<AESSettings>(builder.Configuration.GetSection("AESSettings"));
builder.Services.AddCors(options =>
{
    options.AddPolicy("customPolicy", b =>
    {
        b.AllowAnyHeader()
            .AllowAnyMethod().AllowAnyOrigin();
    });
});

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection"))
    .AddRedis(builder.Configuration.GetConnectionString("RedisConnection"));

// builder.Configuration.AddJsonFile("Yarp.json", optional: false, reloadOnChange: true);

// builder.Services.AddReverseProxy()
//     .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 4;
        opt.Window = TimeSpan.FromSeconds(12);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;
    });
});

// builder.Services.AddRequestTimeouts(options =>
// {
//     options.AddPolicy("timeOutPolicy", TimeSpan.FromSeconds(Convert.ToInt64(builder.Configuration[""])));
// });

var app = builder.Build();

// Exception middleware should be one of the first middlewares in the pipeline
app.UseExceptionMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();

// CORS should be configured early in the pipeline, before other middlewares
app.UseCors("customPolicy");

// HTTPS redirection should be one of the first middlewares for security
app.UseHttpsRedirection();

// Authentication and Authorization middlewares
// app.MapIdentityApi<GatewayApplication>();
// app.UseAuthentication();
// app.UseAuthorization();

// Middleware for handling profile endpoints

app.UseHealthChecks("/healthz");
// Rate Limiter Middleware
app.UseRateLimiter();



// AES Encryption Middleware
app.UseMiddleware<AesEncryptionMiddleware>();

// app.UseRequestTimeouts();

// Reverse Proxy Middleware
app.MapReverseProxy();

app.UseProfileEndpoints();

// Database Migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during migration");
    }
}



// The final middleware to handle requests
app.Run();


