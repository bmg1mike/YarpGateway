using System.ComponentModel.DataAnnotations;

namespace Sterling.Gateway.Domain;

public record RegisterDto
(
    [Required]
    string Email,

    [Required]
    string Password,

    [Required]
    string ApplicationName
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
    string ApplicationBaseUrl,
    string ApplicationName
);

public record GetMicroServiceDto
(
    string ApplicationName,
    string ApplicationBaseUrl
);

public record AddController(
    string ControllerName,
    string MicroserviceId
);
