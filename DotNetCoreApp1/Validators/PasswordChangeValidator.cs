using DotNetCoreApp1.Controllers.Types;
using DotNetCoreApp1.Models.Repositories;
using FluentValidation;

namespace DotNetCoreApp1.Validators
{
    public class PasswordChangeValidator : AbstractValidator<PasswordChange>
    {
        public PasswordChangeValidator()
        {
            RuleFor(p => p.UserId)
                .NotEmpty().WithMessage("User Id cannot be empty!");
            RuleFor(p => p.OriginalPassword)
                .NotEmpty().WithMessage("Original password field cannot be empty!");
            RuleFor(p => p.NewPassword)
                .NotEmpty().WithMessage("New password field cannot be empty")
                .MinimumLength(8).WithMessage("Password length must be at least 8 chars")
                .MaximumLength(16).WithMessage("Password length must not exceed 16 chars")
                .Matches(@"[A-Z]+").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]+").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]+").WithMessage("Password must contain at least one number.")
                .Matches(@"[\!\?\*\.]+").WithMessage("Password must contain at least one (!? *.).");
        }
    }
}
