using Sterling.Gateway.Application;
using Sterling.Gateway.Domain;

namespace Sterling.Gateway.Api;

public static class ProfileEndpointsRegistration
{
    public static RouteGroupBuilder UseProfileEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/UserProfile");

        group.MapPost("/register", async (RegisterDto request, IProfileManagementService service) =>
        {
            return Results.Ok(await service.Register(request));
        });

        group.MapPost("/login", async (LoginDto request, IProfileManagementService service) =>
        {
            return Results.Ok(await service.Login(request));
        });

        group.MapPost("/AddMicroService", async (AddMicroServiceDto request, IEndpointProfilingService service) =>
        {
            return Results.Ok(await service.AddMicroService(request));
        });

        group.MapGet("/GetAllMicroServices", async (IEndpointProfilingService service) =>
        {
            return Results.Ok(await service.GetMicroServices());
        });

        group.MapGet("/GetMicroServiceById/{id}", async (string id, IEndpointProfilingService service) =>
        {
            return Results.Ok(await service.GetMicroserviceById(id));
        });

        group.MapPost("/AddController", async (AddController request, IEndpointProfilingService service) =>
        {
            return Results.Ok(await service.AddControllerToMicroservice(request));
        });

        return group;
    }
}
