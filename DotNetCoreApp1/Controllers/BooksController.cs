using AutoMapper;
using DotNetCoreApp1.Controllers.Types;
using DotNetCoreApp1.Models;
using DotNetCoreApp1.Models.Interfaces;
using DotNetCoreApp1.Models.Types;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace DotNetCoreApp1.Controllers
{
    [ApiController]
    [Authorize(Policy = "MustBeUser")]
    [Route("api/books")]

    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly IValidator<Book> _bookDtoValidator;
        private readonly IValidator<BookDto> _bookValidator;
        private const int maximumPageSize = 20;

        public BooksController(
            AppDbContext appDBContext,
            IBookRepository bookRepository,
            IMapper mapper,
            IValidator<Book> bookDtoValidator,
            IValidator<BookDto> bookValidator
            )
        {
            _appDbContext = appDBContext;
            _bookRepository = bookRepository;
            _bookDtoValidator = bookDtoValidator;
            _bookValidator = bookValidator;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<RecordDto>> GetBooks(
            string? orderBy,
            [FromQuery(Name = "fullTextSearchQuery")] string? searchQuery,
            [FromQuery(Name = "sortOrder")] string? order,
            int? pageNumber,
            int? pageSize)
        {
            if (pageSize > maximumPageSize)
            {
                pageSize = maximumPageSize;
            }

            var (foundBooks, paginationMetadata) = await _bookRepository.GetBooks(orderBy, searchQuery, order?.Equals("DESC"), pageNumber, pageSize);

            if (paginationMetadata != null)
            {
                Response.Headers["X-Pagination"] = JsonSerializer.Serialize(paginationMetadata);
            }

            return Ok(foundBooks);
        }

        [HttpGet("{bookId}")]
        public async Task<ActionResult<BookDto>> GetBook(Guid bookId)
        {
            var foundBook = await _bookRepository.GetBook(bookId);
            if (foundBook == null) { return BadRequest("Book not found"); }
            return Ok(foundBook);
        }

        [HttpPost("create")]
        public async Task<ActionResult<BookDto>> CreateBook(Book? bookToCreate)
        {
            if (bookToCreate == null) { return BadRequest("Book is null"); }

            var result = await _bookDtoValidator.ValidateAsync(bookToCreate);

            if (!result.IsValid)
            {
                return ValidationProblem(new ValidationProblemDetails(result.ToDictionary()));
            }

            var newBook = _mapper.Map<BookDto>(bookToCreate);

            await _appDbContext.Books.AddAsync(newBook);
            await _appDbContext.SaveChangesAsync();
            return Ok(newBook.BookId);
        }

        [HttpPut]
        public async Task<ActionResult<BookDto>> UpdateBook(BookDto? bookToUpdate)
        {
            if (bookToUpdate == null) { return BadRequest("Book is null"); }

            var result = await _bookValidator.ValidateAsync(bookToUpdate);

            if (!result.IsValid)
            {
                return ValidationProblem(new ValidationProblemDetails(result.ToDictionary()));
            }

            _appDbContext.Books.Update(bookToUpdate);
            await _appDbContext.SaveChangesAsync();
            return Ok(bookToUpdate.BookId);
        }

        [HttpDelete("{bookId}")]
        public async Task<ActionResult<Guid>> DeleteBook(Guid bookId)
        {
            var bookToDelete = await _bookRepository.GetBook(bookId);
            if (bookToDelete == null) { return BadRequest("Book not found"); }
            await _bookRepository.DeleteBook(bookToDelete);
            return Ok(bookToDelete.BookId);
        }
    }
}
