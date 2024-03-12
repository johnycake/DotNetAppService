using DotNetCoreApp1.Controllers.Types;
using DotNetCoreApp1.Models.Interfaces;
using DotNetCoreApp1.Models.Types;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreApp1.Models.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _appDbContext;

        public BookRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<(IEnumerable<BookDto>, PaginationMetadata?)> GetBooks(string? orderBy, string? searchQuery, bool? descending, int? pageNumber, int? pageSize)
        {
            var collection = _appDbContext.Books as IQueryable<BookDto>;
            PaginationMetadata? paginationMetadata = null;

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(c => 
                    c.Title.Contains(searchQuery) 
                    || (!string.IsNullOrEmpty(c.Description) && c.Description.Contains(searchQuery)) 
                    || (!string.IsNullOrEmpty(c.Genre) && c.Genre.Contains(searchQuery))
                );
            }

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                orderBy = orderBy.Trim();
                collection = (descending ?? false)
                    ? collection.OrderByDescending(c => EF.Property<object>(c, orderBy))
                    : collection.OrderBy(c => EF.Property<object>(c, orderBy));
            }
            else
            {
                collection = collection.OrderBy(c => c.Title);
            }

            if (pageNumber != null && pageSize != null)
            {
                var totalItemCount = await collection.CountAsync();
                paginationMetadata = new PaginationMetadata(totalItemCount, (int)pageSize, (int)pageNumber);

                collection = collection
                    .Skip((int)(pageSize * (pageNumber - 1)))
                    .Take((int)pageSize);
            }

            var collectionToReturn = await collection.ToListAsync();
            return (collectionToReturn, paginationMetadata);
        }

        public async Task<BookDto?> GetBook(Guid bookId)
        {
            return await _appDbContext.Books.AsNoTracking().FirstOrDefaultAsync(r => r.BookId == bookId);
        }

        public async Task CreateBook(Book bookTocreate)
        {
            var newBook = new BookDto()
            {
                BookId = Guid.NewGuid(),
                Title = bookTocreate.Title,
                Description = bookTocreate.Description,
                Genre = bookTocreate.Genre,
                Content = bookTocreate.Content,
            };

            await _appDbContext.Books.AddAsync(newBook);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task UpdateBook(BookDto bookToUpdate)
        {
            _appDbContext.Books.Update(bookToUpdate);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task DeleteBook(BookDto bookToDelete)
        {
            _appDbContext.Remove(bookToDelete);
            await _appDbContext.SaveChangesAsync();
        }
    }
}
