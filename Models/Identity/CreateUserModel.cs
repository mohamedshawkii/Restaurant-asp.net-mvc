using System.ComponentModel.DataAnnotations;
namespace Restaurant.Models.Identity;

public class CreateUserModel : BaseUserModel
{
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
