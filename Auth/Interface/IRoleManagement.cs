using Restaurant.Models.Identity;

namespace Restaurant.Auth.Interface;

public interface IRoleManagement
{
    Task<string?> GetUserRole(string userEmail);
    Task<bool> AddUserToRole(BaseUserModel user, string roleName);
}
