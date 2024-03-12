using DotNetCoreApp1.Models.Interfaces;
using DotNetCoreApp1.Models.Repositories;
using DotNetCoreApp1.Models.Types;
using FluentValidation;

namespace DotNetCoreApp1.Validators
{
    public class BookDtoValidator : AbstractValidator<BookDto>
    {
        public BookDtoValidator(IBookRepository bookRepository)
        {
            RuleFor(r => r.BookId)
                .NotEmpty().WithMessage("Required value!")
                        .MustAsync(async (bookId, _) =>
                        {
                            return await bookRepository.GetBook(bookId) != null;
                        }).WithMessage("Book ID does not exist");
            RuleFor(r => r.Title)
                .NotEmpty().WithMessage("Required value!");
            RuleFor(r => r.Description)
                .NotEmpty().WithMessage("Required value!");
            RuleFor(r => r.Genre)
                .NotEmpty().WithMessage("Required value!");
        }
    }
}
