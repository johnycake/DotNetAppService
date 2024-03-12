using DotNetCoreApp1.Controllers.Types;
using FluentValidation;

namespace DotNetCoreApp1.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(u => u.UserName)
                .NotEmpty().WithMessage("User Name cannot be empty!");
            RuleFor(u => u.FirstName)
                .NotEmpty().WithMessage("First Name cannot be empty!");
            RuleFor(u => u.Surname)
                .NotEmpty().WithMessage("Surname cannot be empty!");
            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("Your password cannot be empty")
                .MinimumLength(8).WithMessage("Your password length must be at least 8 chars")
                .MaximumLength(16).WithMessage("Your password length must not exceed 16 chars")
                .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                .Matches(@"[\!\?\*\.]+").WithMessage("Your password must contain at least one (!? *.).");
        }
    }
}
