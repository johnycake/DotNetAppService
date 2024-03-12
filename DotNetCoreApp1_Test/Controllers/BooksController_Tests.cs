using AutoMapper;
using DotNetCoreApp1.AutoMapperProfiles;
using DotNetCoreApp1.Controllers;
using DotNetCoreApp1.Models;
using DotNetCoreApp1.Models.Repositories;
using DotNetCoreApp1.Models.Types;
using DotNetCoreApp1.Validators;
using DotNetCoreApp1_Test.Controllers.TestData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;

namespace UnitTesting.Controllers
{
    public class Books : IClassFixture<BooksController_Fixture>
    {
        private readonly BooksController_Fixture fixture;
        private BookValidator BookValidator { get; }
        private IMapper Mapper { get; }
        public Books(BooksController_Fixture booksController_Fixture)
        {
            fixture = booksController_Fixture;
            BookValidator = new BookValidator();
            Mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new BookProfile())));
        }

        [Fact]
        public async Task Create_Book_Async()
        {
            // Arrange
            using var myAppDbContext = fixture.GetContext();
            var myBookRepository = new BookRepository(myAppDbContext);
            BookDtoValidator myBookDtoValidator = new(myBookRepository);
            BooksController sut = new(myAppDbContext, myBookRepository, Mapper, BookValidator, myBookDtoValidator);

            // Act
            var actionResult = await sut.CreateBook(fixture.NewBookToCreate);

            // Assert
            Assert.NotNull(actionResult);
            var result = (actionResult.Result as OkObjectResult)?.Value as Guid?;
            var foundNewlyAddedBook = myAppDbContext.Books.AsNoTracking().FirstOrDefault(u => u.BookId == result);
            Assert.Equal("Zahrakarcenie", foundNewlyAddedBook?.Title);
        }

        [Fact]
        public async void Find_Book_By_ID()
        {
            // Arrange
            var myBookDtoContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            myBookDtoContextMock
                .Setup(x => x.Books)
                .ReturnsDbSet(BooksController_Fixture.FakeBookDtoList);
            var myBookRepository = new BookRepository(myBookDtoContextMock.Object);
            BookDtoValidator myBookDtoValidator = new(myBookRepository);
            BooksController sut = new(myBookDtoContextMock.Object, myBookRepository, Mapper, BookValidator, myBookDtoValidator);
         

            // Act
            var actionResult = await sut.GetBook(new Guid("62b15c71-7109-4eb8-ac14-ee60d7d5575a"));

            // Assert
            Assert.NotNull(actionResult);
            var result = actionResult.Result as OkObjectResult;
            var userDtoSutResult = result?.Value as BookDto;
            Assert.Equal("Medicina pre vsetkych", userDtoSutResult?.Title);
        }

        [Fact]
        public async Task Delete_Book_Async()
        {
            // Arrange
            using var myAppDbContext = fixture.GetContext();
            var myBookRepository = new BookRepository(myAppDbContext);
            BookDtoValidator myBookDtoValidator = new(myBookRepository);
            BooksController sut = new(myAppDbContext, myBookRepository, Mapper, BookValidator, myBookDtoValidator);
            
            // Act
            var actionResult = await sut.DeleteBook(fixture.bookIdToDelete);

            // Assert
            Assert.NotNull(actionResult);
            var result = (actionResult.Result as OkObjectResult)?.Value as Guid?;
            Assert.Equal(fixture.bookIdToDelete, result);
            var deletedBookSearchResult = myAppDbContext.Books.AsNoTracking().FirstOrDefault(u => u.BookId == result);
            Assert.Null(deletedBookSearchResult);
        }

        [Fact]
        public async Task Update_Book_Async()
        {
            // Arrange
            using var myAppDbContext = fixture.GetContext();
            var myBookRepository = new BookRepository(myAppDbContext);
            BookDtoValidator myBookDtoValidator = new(myBookRepository);
            BooksController sut = new(myAppDbContext, myBookRepository, Mapper, BookValidator, myBookDtoValidator);

            // Act
            var actionResult = await sut.UpdateBook(fixture.bookDtoToUpdate);

            // Assert
            Assert.NotNull(actionResult);
            var result = (actionResult.Result as OkObjectResult)?.Value as Guid?;
            var foundNewlyUpdatedBook = myAppDbContext.Books.AsNoTracking().FirstOrDefault(u => u.BookId== result);
            Assert.Equal("Programovanie .NET", foundNewlyUpdatedBook?.Title);
        }
    }
}