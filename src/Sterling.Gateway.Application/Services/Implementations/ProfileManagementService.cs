using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sterling.Gateway.Domain;

namespace Sterling.Gateway.Application;

public class ProfileManagementService(UserManager<GatewayApplication> userManager, SignInManager<GatewayApplication> signinManager, IConfiguration config, ILogger<ProfileManagementService> logger) : IProfileManagementService
{
    private readonly UserManager<GatewayApplication> userManager = userManager;
    private readonly SignInManager<GatewayApplication> signinManager = signinManager;
    private readonly IConfiguration config = config;
    private readonly ILogger<ProfileManagementService> logger = logger;

    public async Task<Result<string>> Register(RegisterDto request)
    {
        try
        {
            var user = new GatewayApplication
            {
                UserName = request.Email,
                ApplicationName = request.ApplicationName,
                Email = request.Email
            };

            var checkUser = await userManager.FindByNameAsync(user.UserName);

            if (checkUser is not null)
            {
                logger.LogInformation($"An Application already exist with email {user.Email}");
                return Result<string>.Failure($"An Application already exist with email {user.Email}");
            }

            var createUser = await userManager.CreateAsync(user, request.Password);

            if (createUser.Succeeded)
            {
                return Result<string>.Success("User created successfully");
            }

            foreach (var item in createUser.Errors)
            {
                logger.LogError($"Create Application Error {item.Description}");
            }

            return Result<string>.Failure("There was a problem creating this user, please try again later");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result<string>.Failure(ex.Message);
        }
    }

    public async Task<Result<LoginResponse>> Login(LoginDto request)
    {
        try
        {
            var user = await userManager.FindByNameAsync(request.Email);

            if (user is null)
            {
                return Result<LoginResponse>.Failure("Invalid Login Credentials");
            }

            var passwordCheck = await userManager.CheckPasswordAsync(user, request.Password);

            if (!passwordCheck)
            {
                return Result<LoginResponse>.Failure("Invalid Login Credentials");
            }

            var response = new LoginResponse
            (
                Utilities.GenerateToken(user, config),
                Utilities.GenerateRefreshToken()
            );

            return Result<LoginResponse>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result<LoginResponse>.Failure(ex.Message);
        }
    }
}
