using DotNetCoreApp1.Controllers.Types;
using DotNetCoreApp1.Models.Types;

namespace DotNetCoreApp1.Models.Interfaces
{
    public interface IBookRepository
    {
        public Task<(IEnumerable<BookDto>, PaginationMetadata?)> GetBooks(string? orderBy, string? searchQuery, bool? descending, int? pageNumber, int? pageSize);
        public Task<BookDto?> GetBook(Guid bookId);
        public Task CreateBook(Book bookTocreate);
        public Task UpdateBook(BookDto bookToUpdate);
        public Task DeleteBook(BookDto bookToDelete);
    }
}
