using System.ComponentModel.DataAnnotations;
namespace Sterling.Gateway.Domain;

public record RegisterDto
(
    [Required]
    string Email,

    [Required]
    string Password,

    [Required]
    string ApplicationName,

    [Required]
    Permission Permission
);

public record LoginDto
(
    [Required]
    string Email,

    [Required]
    string Password
);

public record LoginResponse
(
    string AccessToken,
    string RefreshToken
);

public record AddMicroServiceDto
(
    string MicroserviceBaseUrl,
    string MicroserviceName
);

public record GetMicroServiceDto
(
    string Id,
    string ApplicationName,
    string ApplicationBaseUrl
);

public record AddController(
    string ControllerName,
    string MicroserviceId,
    Permission Permission,
    bool isApiApended
);

public record AddEndpoint(
    ApiType ApiType,
    string SubUrl,
    string MicroServiceId,
    string RequestPayload,
    string ResponsePayload
);
public record GetEndpoint(
    string Id,
    string ApiType,
    string SubUrl,
    string MicroServiceId,
    string CallHeader,
    string CallHeaderValue,
    string RequestPayload,
    string ResponsePayload
);

public record GetEndpointList(
    List<GetEndpoint> Endpoints,
    int TotalCount,
    int TotalPages,
    int PageSize,
    int CurrentPage
);

public class PaginatedListRequest
{
    public int PageSize { get; set; } = 10; // default value
    public int PageNumber { get; set; } = 1; // default value
}
