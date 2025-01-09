using Restaurant.Models.Identity;

namespace Restaurant.Auth.Interface.Authentication;

public interface IAuthenticationService
{
    Task<ServiceResponse> CreateUser(CreateUserModel user);
    Task<LoginResponse> LoginUser(BaseUserModel user);
    Task<LoginResponse> ReviveToken(string refreshToken);
}
