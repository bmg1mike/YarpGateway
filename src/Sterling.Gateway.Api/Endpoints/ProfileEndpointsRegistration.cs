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
        }).AllowAnonymous();

        group.MapPost("/login", async (LoginDto request, IProfileManagementService service) =>
        {
            return Results.Ok(await service.Login(request));
        }).AllowAnonymous();

        group.MapPost("/AddMicroService", async (AddMicroServiceDto request, IEndpointProfilingService service) =>
        {
            return Results.Ok(await service.AddMicroService(request));
        }).RequireAuthorization("AdminPolicy");

        group.MapGet("/GetAllMicroServices", async (IEndpointProfilingService service) =>
        {
            return Results.Ok(await service.GetMicroServices());
        }).RequireAuthorization();

        group.MapGet("/GetMicroServiceById/{id}", async (string id, IEndpointProfilingService service) =>
        {
            return Results.Ok(await service.GetMicroserviceById(id));
        }).RequireAuthorization();

        group.MapPost("/AddController", async (AddController request, IEndpointProfilingService service) =>
        {
            return Results.Ok(await service.AddControllerToMicroservice(request));
        }).RequireAuthorization("AdminPolicy");

        group.MapPost("/AddEndpoint", async (AddEndpoint request, IEndpointProfilingService service) =>
        {
            return Results.Ok(await service.AddEndpoint(request));
        }).RequireAuthorization("AdminPolicy");

        return group;
    }
}
