using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Restaurant.Auth.Interface;
using Restaurant.Data;
using Restaurant.Models.Identity;
using System.Security.Claims;

namespace Restaurant.Auth.Implementation;
public class UserManagment(IRoleManagement roleManagement, UserManager<BaseUserModel> userManager, ApplicationDbContext context) : IUserManagement
{
    public async Task<bool> CreateUser(CreateUserModel user)
    {
        var _user = await GetUserByEmail(user.Email!);
        if (_user != null) return false;
        var result = await userManager.CreateAsync(user!, user!.PasswordHash!);
        if (result.Succeeded) { return true; } else { return false; }
    }

    public async Task<IEnumerable<BaseUserModel>?> GetAllUsers() => await context.Users.ToListAsync();

    public async Task<BaseUserModel?> GetUserByEmail(string email) =>
        await userManager.FindByEmailAsync(email);
     
    public async Task<BaseUserModel> GetUserById(string id) {
        var user = await userManager.FindByIdAsync(id);
        return user!;
    }

    public async Task<List<Claim>> GetUserClaims(string email)
    {
        var _user = await GetUserByEmail(email);
        string? roleName = await roleManagement.GetUserRole(_user!.Email!);

        List<Claim> claims = [
            new Claim(ClaimTypes.NameIdentifier, _user!.Id),
            new Claim(ClaimTypes.Email, _user!.Email!),
            new Claim(ClaimTypes.Role, roleName!)
        ];
        return claims;
    }

    public async Task<bool> LoginUser(BaseUserModel user)
    {
        var _user = await GetUserByEmail(user.Email!);
        if(_user is null) return false;

        string? roleName = await roleManagement.GetUserRole(_user!.Email!);
        if(string.IsNullOrEmpty(roleName)) return false;

        return await userManager.CheckPasswordAsync(_user, user.PasswordHash!);
    }

    public async Task<int> RemovUserByEmail(string email)
    {
        var user = await context.Users.FirstOrDefaultAsync(_ => _.Email == email);
        context.Users.Remove(user!);
        return await context.SaveChangesAsync();
    }

}
