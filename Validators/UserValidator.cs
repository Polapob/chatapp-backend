using FluentValidation;
using chatapp_backend.Dtos;

public class UpdateUserDTOValidator : AbstractValidator<UpdateUserDTO>
{
    public UpdateUserDTOValidator()
    {
        RuleFor(u => u.FirstName).NotNull();
        RuleFor(u => u.LastName).NotNull();
        RuleFor(u => u.Password).NotNull();
        RuleFor(u => u.UserName).NotNull();
    }
}