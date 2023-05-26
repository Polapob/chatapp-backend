using FluentValidation;
using chatapp_backend.Dtos;

namespace chatapp_backend.Validators;
public class UpdateUserDTOValidator : AbstractValidator<UpdateUserDTO>
{
    public UpdateUserDTOValidator()
    {
        RuleFor(u => u.FirstName).NotEmpty();
        RuleFor(u => u.LastName).NotEmpty();
        RuleFor(u => u.Password).NotEmpty();
        RuleFor(u => u.UserName).NotEmpty();
    }
}