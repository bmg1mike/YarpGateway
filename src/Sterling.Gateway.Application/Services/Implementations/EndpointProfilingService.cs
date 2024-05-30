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
            var clusterInDb = await context.ClusterConfigs.Where(x => x.ClusterId == request.MicroserviceName.ToLower().Trim()).AsNoTracking().FirstOrDefaultAsync();
            if (clusterInDb != null)
            {
                return Result<string>.Failure("Microservice already exist");
            }

            var cluster = new ClusterConfigEntity
            {
                ClusterId = request.MicroserviceName.ToLower().Trim(),
                DestinationAddress = request.MicroserviceBaseUrl
            };

            await context.ClusterConfigs.AddAsync(cluster);
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
            var microservices = await context.ClusterConfigs.ToListAsync();
            foreach (var microservice in microservices)
            {
                result.Add(new GetMicroServiceDto
                               (
                                    microservice.Id,
                                    microservice.ClusterId,
                                    microservice.DestinationAddress
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
            var microservice = await context.ClusterConfigs.FindAsync(id);
            if (microservice == null)
            {
                return Result<GetMicroServiceDto>.Failure("Invalid Id");
            }

            var result = new GetMicroServiceDto
            (
                microservice.Id,
                microservice.ClusterId,
                microservice.DestinationAddress
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
            string appendApi = request.isApiApended ? "/api" : string.Empty;
            string patternApiAppended = request.isApiApended ? "api/" : string.Empty;

            var cluster = await context.ClusterConfigs.FindAsync(request.MicroserviceId);
            if (cluster == null)
            {
                return Result<string>.Failure("Invalid Microservice");
            }

            var routeInDb = await context.RouteConfigs.Where(x => x.RouteId == $"{cluster.ClusterId.ToLower().Trim()}_{request.ControllerName.ToLower().Trim()}").AsNoTracking().FirstOrDefaultAsync();

            if (routeInDb != null)
            {
                return Result<string>.Failure("Controller already exist");
            }

            var route = new RouteConfigEntity
            {
                ClusterId = cluster.ClusterId,
                Path = $"{appendApi}/{request.ControllerName.ToLower()}/{{**catch-all}}",
                RouteId = $"{cluster.ClusterId.ToLower().Trim()}_{request.ControllerName.ToLower().Trim()}",
                AuthorizationPolicy = $"{request.Permission.ToString()}Policy",
                MicroServiceId = cluster.Id
            };
            await context.RouteConfigs.AddAsync(route);

            if (await context.SaveChangesAsync() > 0)
            {
                return Result<string>.Success("Controller has been added successfully");
            }

            return Result<string>.Failure("There was a problem adding Controller To Microservice, please try again later");
        }
        catch (System.Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result<string>.Failure("There was a problem, Please try again later");
        }
    }


    public async Task<Result<string>> AddEndpoint(AddEndpoint request)
    {

        try
        {
            var microservice = await context.ClusterConfigs.FindAsync(request.MicroServiceId);

            if (microservice == null)
            {
                return Result<string>.Failure("Invalid Microservice");
            }

            var endpoint = new Endpoint
            {
                MicroServiceId = request.MicroServiceId,
                ResponsePayload = request.ResponsePayload,
                SubUrl = request.SubUrl,
                ApiType = request.ApiType,
                RequestPayload = request.RequestPayload
            };

            await context.Endpoints.AddAsync(endpoint);

            if (await context.SaveChangesAsync() > 0)
            {
                return Result<string>.Success("Endpoint has been added successfully");
            }

            logger.LogError("Unable to save to database");
            return Result<string>.Failure("There was a problem, Please try again later");

        }
        catch (System.Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result<string>.Failure("There was a problem, Please try again later");
        }
    }

    public async Task<Result<GetEndpointList>> GetEndpoints(int pageSize = 10, int pageNumber = 1)
    {
        try
        {
            var count = await context.Endpoints.CountAsync();
            var totalPages = (int)Math.Ceiling(count / (double)pageSize);
            var endpoints = new List<GetEndpoint>();
            var items = await context.Endpoints.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            if (items.Count == 0)
            {
                return Result<GetEndpointList>.Failure("There are no endpoints at the moment");
            }


            foreach (var item in items)
            {
                endpoints.Add(new GetEndpoint(
                    item.Id,
                    item.ApiType.ToString(),
                    item.SubUrl,
                    item.MicroServiceId,
                    "X-Cluster",
                    item.MicroService!.ClusterId,
                    item.RequestPayload!,
                    item.ResponsePayload
                ));
            }

            var listOfEndpoints = new GetEndpointList(endpoints, count, totalPages, pageSize, pageNumber);

            return Result<GetEndpointList>.Success(listOfEndpoints);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result<GetEndpointList>.Failure("There was a problem, Please try again later");
        }
    }

    public async Task<Result<GetEndpoint>> GetEndpointById(string id)
    {
        try
        {
            var endpointInDb = await context.Endpoints.Include(x => x.MicroService).FirstOrDefaultAsync(x => x.Id == id);
            if (endpointInDb == null)
            {
                return Result<GetEndpoint>.Failure("Endpoint does not exist");
            }
            var endpoint = new GetEndpoint(
                    endpointInDb.Id,
                    endpointInDb.ApiType.ToString(),
                    endpointInDb.SubUrl,
                    endpointInDb.MicroServiceId,
                    "X-Cluster",
                    endpointInDb.MicroService!.ClusterId,
                    endpointInDb.RequestPayload!,
                    endpointInDb.ResponsePayload
            );

            return Result<GetEndpoint>.Success(endpoint);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result<GetEndpoint>.Failure("There was a problem, Please try again later");
        }
    }

}
