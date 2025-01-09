using FluentValidation;

namespace Restaurant.Auth.Interface.Validations;
public interface IValidationService
{
    Task<ServiceResponse> ValidateAsync<T>(T model, IValidator<T> validator);
}
