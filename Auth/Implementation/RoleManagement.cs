using Microsoft.AspNetCore.Identity;
using Restaurant.Auth.Interface;
using Restaurant.Models.Identity;

namespace Restaurant.Auth.Implementation;

public class RoleManagement(UserManager<BaseUserModel> userManager) : IRoleManagement
{
    public async Task<bool> AddUserToRole(BaseUserModel user, string roleName) =>
        (await userManager.AddToRoleAsync(user, roleName)).Succeeded;

    public async Task<string?> GetUserRole(string userEmail)
    {
        var user =  await userManager.FindByEmailAsync(userEmail);
        return (await userManager.GetRolesAsync(user!)).FirstOrDefault();
    }
}
