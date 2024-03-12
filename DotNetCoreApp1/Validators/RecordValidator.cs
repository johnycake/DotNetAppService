using DotNetCoreApp1.Controllers.Types;
using DotNetCoreApp1.Models.Interfaces;
using FluentValidation;

namespace DotNetCoreApp1.Validators
{
    public class RecordValidator : AbstractValidator<Record>
    {
        public RecordValidator(IBookRepository bookRepository)
        {
            RuleFor(r => r.LendedFrom)
                .NotEmpty().WithMessage("Required value!")
                .GreaterThanOrEqualTo(r => DateTime.Today).WithMessage($"Must be Grater than or Equal to {DateTime.Today}");
            RuleFor(r => r.LendedTo)
                .NotEmpty().WithMessage("Required value!")
                .GreaterThanOrEqualTo(r => r.LendedFrom).WithMessage("Must be after Date of LendingFrom");
            RuleFor(r => r.SendNotification)
                .NotEmpty().WithMessage("Required value!");
            RuleFor(r => r.BookId)
                .NotEmpty().WithMessage("Required value!")
                .MustAsync(async (bookId, _) =>
                    {
                        return await bookRepository.GetBook(bookId) != null;
                    }).WithMessage("Book ID does not exist");
        }
    }
}
