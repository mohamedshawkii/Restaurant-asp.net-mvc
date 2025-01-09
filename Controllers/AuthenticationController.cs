using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Auth.Interface.Authentication;
using Restaurant.Models.Identity;
namespace Restaurant.Controllers;

public class AuthenticationController : Controller
{
    private readonly IAuthenticationService _authenticationService;
    private readonly SignInManager<BaseUserModel> _signInManager;
    private readonly UserManager<BaseUserModel> _userManager;
    public AuthenticationController(IAuthenticationService authenticationService,
        SignInManager<BaseUserModel> signInManager,
        UserManager<BaseUserModel> userManager)
    {
        _authenticationService = authenticationService;
        _signInManager = signInManager;
        _userManager = userManager;
    }
    public IActionResult CreateUser()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserModel user)
    {
        if (ModelState.IsValid)
        {
            var result =  await _authenticationService.CreateUser(user);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(user);
            }
            else
            {
                return Redirect("LoginUser");
            }
        }
        else {
            return View(user);
        }
    }
    public IActionResult LoginUser()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> LoginUser(BaseUserModel UserModel)
    {
        if (ModelState.IsValid)
        {
            var result = await _authenticationService.LoginUser(UserModel);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(UserModel);
            }
            else
            {
                var user = await _userManager.FindByNameAsync(UserModel.Email!);
                if (user != null)
                {
                    await _signInManager.SignInAsync(user!, isPersistent: false);
                    HttpContext.Response.Cookies.Append("AuthToken", result.Token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true, // Only over HTTPS
                        SameSite = SameSiteMode.Strict
                    });
                    return RedirectToAction("index", "Home");
                }
            }
        }
        return View(UserModel);
    }
    public async Task<IActionResult> LogOut()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("LoginUser");
    }

    [HttpGet("refreshToken/{refreshToken}")]
    public async Task<IActionResult> ReviveToken(string refreshToken)
    {
        var result = await _authenticationService.ReviveToken(refreshToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
