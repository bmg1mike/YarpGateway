using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sterling.Gateway.Data;
using Sterling.Gateway.Domain;

namespace Sterling.Gateway.Application;

public class EndpointProfilingService(ApplicationDbContext context, ILogger<EndpointProfilingService> logger) : IEndpointProfilingService
{
    private readonly ApplicationDbContext context = context;
    private readonly ILogger<EndpointProfilingService> logger = logger;

    public async Task<Result<string>> AddMicroService(AddMicroServiceDto request)
    {
        try
        {
            AddRoute(request);
            AddCluster(request);
            var microservice = new MicroService
            {
                MicroServiceName = request.ApplicationName,
                BaseUrl = request.ApplicationBaseUrl
            };

            await context.MicroServices.AddAsync(microservice);
            if (await context.SaveChangesAsync() > 0)
            {
                return Result<string>.Success("MicroService has been added successfully");
            }

            return Result<string>.Failure("There was a problem adding MicroService, please try again later");

        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result<string>.Failure("There was a problem adding MicroService, please try again later");
        }
    }

    public async Task<Result<List<GetMicroServiceDto>>> GetMicroServices()
    {
        try
        {
            var result = new List<GetMicroServiceDto>();
            var microservices = await context.MicroServices.ToListAsync();
            foreach (var microservice in microservices)
            {
                result.Add(new GetMicroServiceDto
                               (
                                    microservice.MicroServiceName,
                                    microservice.BaseUrl
                                )
                        );
            }

            return Result<List<GetMicroServiceDto>>.Success(result);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result<List<GetMicroServiceDto>>.Failure("There was a problem, Please try again later");
        }
    }

    public async Task<Result<GetMicroServiceDto>> GetMicroserviceById(string id)
    {
        try
        {
            var microservice = await context.MicroServices.FindAsync(id);
            if (microservice == null)
            {
                return Result<GetMicroServiceDto>.Failure("Invalid Id");
            }

            var result = new GetMicroServiceDto
            (
                microservice.MicroServiceName,
                microservice.BaseUrl
            );
            return Result<GetMicroServiceDto>.Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result<GetMicroServiceDto>.Failure("There was a problem, Please try again later");
        }
    }

    public async Task<Result<string>> AddControllerToMicroservice(AddController request)
    {
        try
        {
            var microservice = await context.MicroServices.FindAsync(request.MicroserviceId);
            if (microservice == null)
            {
                return Result<string>.Failure("Invalid Microservice");
            }
            var route = new AddMicroServiceDto(microservice.BaseUrl, microservice.MicroServiceName,request.isApiApended);
            AddRoute(route, request.ControllerName);

            return Result<string>.Success("Success");
        }
        catch (System.Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result<string>.Failure("There was a problem, Please try again later");
        }
    }

    // Add Endpoints to Api
    private void AddRoute(AddMicroServiceDto request, string controllerName = null!)
    {
        string jsonFilePath = $"{Directory.GetCurrentDirectory()}/Yarp.json";
        string jsonText = File.ReadAllText(jsonFilePath);

        string appendApi = request.isApiApended ? "/api" : string.Empty;
        string patternApiAppended = request.isApiApended ? "api/" : string.Empty;

        string pathName = request.ApplicationName;

        if (!string.IsNullOrEmpty(controllerName))
        {
            pathName = controllerName;
        }

        // Parse the JSON into a JsonObject
        JsonObject jsonObject = JsonNode.Parse(jsonText)!.AsObject();

        // New value for the "Routes" key
        string newRouteJson = @$"
        {{
          ""ClusterId"": ""{request.ApplicationName.ToLower()}"",
          ""RateLimiterPolicy"": ""fixed"",
          ""Match"": {{
            ""Path"": ""{appendApi}/{pathName.ToLower()}/{{**catch-all}}""
          }},
          ""Transforms"": [
            {{
              ""PathPattern"": ""{patternApiAppended}{pathName.ToLower()}/{{**catch-all}}""
            }}
          ]
        }}";
        JsonObject newRouteObject = JsonNode.Parse(newRouteJson)!.AsObject();

        // Get the existing "Routes" object
        JsonObject routesObject = jsonObject["ReverseProxy"]!["Routes"]!.AsObject();

        // Add the new route to the "Routes" object with a new key
        routesObject.Add(pathName, newRouteObject);

        // Serialize the modified JsonObject back to a JSON string
        string modifiedJsonText = jsonObject.ToJsonString(new JsonSerializerOptions { WriteIndented = true });

        // Save the modified JSON back to the file
        File.WriteAllText(jsonFilePath, modifiedJsonText);
    }
    private void AddCluster(AddMicroServiceDto request)
    {
        string jsonFilePath = $"{Directory.GetCurrentDirectory()}/Yarp.json";
        string jsonText = File.ReadAllText(jsonFilePath);

        // Parse the JSON into a JsonObject
        JsonObject jsonObject = JsonNode.Parse(jsonText)!.AsObject();

        // New value for the "Routes" key
        string newClusterJson = @$"
        {{
                ""Destinations"": {{
                    ""{request.ApplicationName.ToLower()}Api"": {{
                        ""Address"": ""{request.ApplicationBaseUrl}""
                    }}
                }}
            
        }}";
        JsonObject newClusterObject = JsonNode.Parse(newClusterJson)!.AsObject();

        // Get the existing "Routes" object
        JsonObject routesObject = jsonObject["ReverseProxy"]!["Clusters"]!.AsObject();

        // Add the new route to the "Routes" object with a new key
        routesObject.Add(request.ApplicationName.ToLower(), newClusterObject);

        // Serialize the modified JsonObject back to a JSON string
        string modifiedJsonText = jsonObject.ToJsonString(new JsonSerializerOptions { WriteIndented = true });

        // Save the modified JSON back to the file
        File.WriteAllText(jsonFilePath, modifiedJsonText);
    }
}
