using chatapp_backend.Dtos;
using FluentValidation;

namespace chatapp_backend.Validators;

public class RegisterDTOValidator : AbstractValidator<RegisterDTO>
{
    public RegisterDTOValidator()
    {
        RuleFor(
            x => x.FirstName
        ).NotNull();
        RuleFor(x => x.LastName).NotNull();
        RuleFor(x => x.UserName).NotNull();
        RuleFor(x => x.Email).NotNull().EmailAddress();
        RuleFor(x => x.Password).NotNull();
        RuleFor(x => x.ConfirmPassword).NotNull();
    }
}

public class LoginDTOValidator : AbstractValidator<LoginDTO>
{
    public LoginDTOValidator()
    {
        RuleFor(x => x.Email).NotNull().EmailAddress();
        RuleFor(x => x.Password).NotNull();
    }
}