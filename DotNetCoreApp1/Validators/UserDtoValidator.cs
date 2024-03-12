using DotNetCoreApp1.Models.Types;
using FluentValidation;

namespace DotNetCoreApp1.Validators
{
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        public UserDtoValidator()
        {
            RuleFor(u => u.UserId)
                .NotEmpty().WithMessage("Required value!");
            RuleFor(u => u.UserName)
                .NotEmpty().WithMessage("User Name cannot be empty!");
            RuleFor(u => u.FirstName)
                .NotEmpty().WithMessage("First Name cannot be empty!");
            RuleFor(u => u.Surname)
                .NotEmpty().WithMessage("Surname cannot be empty!");
        }
    }
}
