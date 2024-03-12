using DotNetCoreApp1.Models.Interfaces;
using DotNetCoreApp1.Models.Types;
using FluentValidation;

namespace DotNetCoreApp1.Validators
{
    public class RecordDtoValidator : AbstractValidator<RecordDto>
    {
        public RecordDtoValidator(IBookRepository bookRepository, IRecordRepository recordRepository)
        {
            RuleFor(r => r.RecordId)
                .NotEmpty().WithMessage("Required value!")
                .MustAsync(async (recordId, _) =>
                {
                    return await recordRepository.GetRecord(recordId) != null;
                }).WithMessage("Record ID does not exist");
            RuleFor(r => r.LendedFrom)
                .NotEmpty().WithMessage("Required value!");
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
