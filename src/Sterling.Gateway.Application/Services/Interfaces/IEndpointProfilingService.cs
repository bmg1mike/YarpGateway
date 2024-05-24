using Sterling.Gateway.Domain;

namespace Sterling.Gateway.Application;

public interface IEndpointProfilingService
{
    Task<Result<string>> AddMicroService(AddMicroServiceDto request);
    Task<Result<List<GetMicroServiceDto>>> GetMicroServices();
    Task<Result<GetMicroServiceDto>> GetMicroserviceById(string id);
    Task<Result<string>> AddControllerToMicroservice(AddController request);
}
