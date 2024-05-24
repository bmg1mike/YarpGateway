using Sterling.Gateway.Domain;

namespace Sterling.Gateway.Application;

public interface IProfileManagementService
{
    Task<Result<string>> Register(RegisterDto request);
    Task<Result<LoginResponse>> Login(LoginDto request);
}
