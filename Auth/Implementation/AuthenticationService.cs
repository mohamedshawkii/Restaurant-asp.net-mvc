using FluentValidation;
using Restaurant.Auth.Interface;
using Restaurant.Auth.Interface.Authentication;
using Restaurant.Auth.Interface.Validations;
using Restaurant.Models.Identity;

namespace Restaurant.Auth.Implementation;

public class AuthenticationService(ITokenManagement tokenManagement,
    IUserManagement userManagement, IRoleManagement roleManagement,
    IValidator<BaseUserModel> UserValidator,
    IValidationService validationService) : IAuthenticationService
{
    public async Task<ServiceResponse> CreateUser(CreateUserModel user)
    {
        var _validationResult = await validationService.ValidateAsync(user, UserValidator);
        if (!_validationResult.Success) return _validationResult;

        user.UserName = user.Email;
        user.PasswordHash = user.Password;
        var result = await userManagement.CreateUser(user);
        if (!result)
            return new ServiceResponse { Message = "Email Address might be already in use or unknown error occured" };

        var _user = await userManagement.GetUserByEmail(user.Email!);
        var users = await userManagement.GetAllUsers();
        bool assignedResult = await roleManagement.AddUserToRole(_user!, users!.Count() > 1 ? "User" : "Admin");

        if (!assignedResult)
        {
            int removeUserResult = await userManagement.RemovUserByEmail(_user!.Email!);
            if (removeUserResult <= 0)
            {
                return new ServiceResponse { Message = "Error occured in creating account" };
            }
        }
        return new ServiceResponse { Success = true, Message = "Account created!" };
        //verify Email
    }

    public async Task<LoginResponse> LoginUser(BaseUserModel user)
    {
        var _validationResult = await validationService.ValidateAsync(user, UserValidator);
        if (!_validationResult.Success)
            return new LoginResponse(Message: _validationResult.Message);

        user.PasswordHash = user.Password;
        bool loginResult = await userManagement.LoginUser(user);
        if (!loginResult)
            return new LoginResponse(Message: "Email not found or Invalid credentials");

        var _user = await userManagement.GetUserByEmail(user!.Email!);
        var claims = await userManagement.GetUserClaims(_user!.Email!);

        string jwtToken = tokenManagement.GenerateToken(claims);
        string refreshToken = tokenManagement.GetRefreshTokens();

        int saveTokenResult = 0;
        bool userTokenCheck = await tokenManagement.ValidationRefreshToken(refreshToken);
        if (userTokenCheck)
            saveTokenResult = await tokenManagement.UpdateRefreshToken(_user.Id, refreshToken);
        else
            saveTokenResult = await tokenManagement.AddRefreshToken(_user.Id, refreshToken);

        return saveTokenResult <= 0 ? new LoginResponse(Message: "Internal error occurred while authentication") :
            new LoginResponse(Success: true, Token: jwtToken, RefreshToken: refreshToken);
    }

    public async Task<LoginResponse> ReviveToken(string refreshToken)
    {
        bool validateTokenResult = await tokenManagement.ValidationRefreshToken(refreshToken);
        if (!validateTokenResult)
            return new LoginResponse(Message: "Invalid token");

        string userId = await tokenManagement.GetUserIdByRefreshToken(refreshToken);
        BaseUserModel? user = await userManagement.GetUserByEmail(userId);
        var claims = await userManagement.GetUserClaims(user!.Email!);
        string newJwtToken = tokenManagement.GenerateToken(claims);
        string newRefreshToken = tokenManagement.GetRefreshTokens();
        await tokenManagement.UpdateRefreshToken(userId, newRefreshToken);
        return new LoginResponse(Success: true, Token: newJwtToken, RefreshToken: newRefreshToken);
    }
}
