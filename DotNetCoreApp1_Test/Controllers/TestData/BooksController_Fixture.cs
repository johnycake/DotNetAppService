using DotNetCoreApp1.Controllers.Types;
using DotNetCoreApp1.Models;
using DotNetCoreApp1.Models.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DotNetCoreApp1_Test.Controllers.TestData
{
    public class BooksController_Fixture : IDisposable
    {
        private readonly DbContextOptions<AppDbContext> _options;
        public AppDbContext AppDbContext { get; private set; }

        public static List<BookDto> FakeBookDtoList => [
                new() {
                        BookId = new Guid("801a9493-c0d4-4027-ab19-5943bc965588"),
                        Title = "Elektrotechnika pre zaciatocnikov",
                        Description = "pajkovanie, suciastky atd",
                        Genre = "technika",
                        Content = "blablabla"
                },
                new() {
                        BookId = new Guid("62b15c71-7109-4eb8-ac14-ee60d7d5575a"),
                        Title = "Medicina pre vsetkych",
                        Description = "ako uzivat acylpirin",
                        Genre = "zdravoveda",
                        Content = "blablabla"
                },
                new() {
                        BookId = new Guid("1faaab6f-fbbb-406e-9a28-c4052ca131a5"),
                        Title = "Programovanie",
                        Description = "Dot Net developement",
                        Genre = "Drama",
                        Content = "blabla"
                },
            ];

        public Book NewBookToCreate = new()  {
            Title = "Zahrakarcenie",
            Description = "stepenie stromcekov",
            Genre = "Dom a zahrada",
            Content = "blabla"
        };
        public BookDto bookDtoToUpdate = new() {
            BookId = new Guid("1faaab6f-fbbb-406e-9a28-c4052ca131a5"),
            Title = "Programovanie .NET",
            Description = "Dot Net developement na kazdy den",
            Genre = "Total Drama",
            Content = "treba kavu.... a vela"
        };
        public Guid bookIdToDelete = new("801a9493-c0d4-4027-ab19-5943bc965588");
        public BooksController_Fixture()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "BooksDatabase")
                .EnableSensitiveDataLogging()
                .Options;
            
            AppDbContext = new AppDbContext(_options);

            SeedDataToDatabase();
        }

        public AppDbContext GetContext()
        {
            return new AppDbContext(_options);
        }

        public void Dispose()
        {
            AppDbContext.Database.EnsureDeleted();
        }

        private async void SeedDataToDatabase()
        {
            AppDbContext.Database.EnsureDeleted();
            foreach (BookDto bookDto in FakeBookDtoList)
            {
                await AppDbContext.Books.AddAsync(bookDto);
            }

            await AppDbContext.SaveChangesAsync();
        }
    }
}
