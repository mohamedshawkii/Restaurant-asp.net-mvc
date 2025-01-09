using FluentValidation;
using Restaurant.Models.Identity;

namespace Restaurant.Auth.Validations.Authentication;

public class LoginUserValidator : AbstractValidator<BaseUserModel>
{
    public LoginUserValidator ()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
