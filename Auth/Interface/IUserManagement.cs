using Restaurant.Models.Identity;
using System.Security.Claims;

namespace Restaurant.Auth.Interface;

public interface IUserManagement
{
    Task<bool> CreateUser(CreateUserModel user);
    Task<bool> LoginUser(BaseUserModel user);
    Task<BaseUserModel?> GetUserByEmail(string email);
    Task<BaseUserModel> GetUserById(string id);
    Task<IEnumerable<BaseUserModel>?> GetAllUsers();
    Task<int> RemovUserByEmail(string email);
    Task<List<Claim>> GetUserClaims(string email);
}
