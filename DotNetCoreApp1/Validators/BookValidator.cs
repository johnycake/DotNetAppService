using DotNetCoreApp1.Controllers.Types;
using FluentValidation;

namespace DotNetCoreApp1.Validators
{
    public class BookValidator : AbstractValidator<Book>
    {
        public BookValidator()
        {
            RuleFor(r => r.Title)
                .NotEmpty().WithMessage("Required value!");
            RuleFor(r => r.Description)
                .NotEmpty().WithMessage("Required value!");
            RuleFor(r => r.Genre)
                .NotEmpty().WithMessage("Required value!");
        }
    }
}
