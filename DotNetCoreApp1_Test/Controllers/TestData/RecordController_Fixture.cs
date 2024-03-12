using DotNetCoreApp1.Controllers.Types;
using DotNetCoreApp1.Models;
using DotNetCoreApp1.Models.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Record = DotNetCoreApp1.Controllers.Types.Record;

namespace DotNetCoreApp1_Test.Controllers.TestData
{
    public class RecordController_Fixture : IDisposable
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
        public static List<RecordDto> FakeRecordDtoList => [
                new() {
                        RecordId = new Guid("3728a9c0-4ed0-4584-4fa7-08dc41aa37ae"),
                        BookId = new Guid("801a9493-c0d4-4027-ab19-5943bc965588"),
                        LendedFrom = DateTime.Parse("2024-03-11T09:02:53.809"),
                        LendedTo = DateTime.Parse("2024-09-10T14:00:00.324"),
                        Tags = [
                          "Tag1"
                        ],
                        SendNotification = true
                },
                new() {
                        RecordId = new Guid("8c28c9cf-06e8-4d0f-8ce6-3b84c93fff18"),
                        BookId = new Guid("62b15c71-7109-4eb8-ac14-ee60d7d5575a"),
                        LendedFrom = DateTime.Parse("2024-03-10T13:57:16.324"),
                        LendedTo = DateTime.Parse("2024-03-10T13:57:16.324"),
                        Tags = [
                          "Tag2"
                        ],
                        SendNotification = true
                },
                new() {
                        RecordId = new Guid("b9aece44-1ec7-46f2-86d1-dbe01a1ef6de"),
                        BookId = new Guid("1faaab6f-fbbb-406e-9a28-c4052ca131a5"),
                        LendedFrom = DateTime.Parse("2024-03-11T08:59:49.531"),
                        LendedTo = DateTime.Parse("2024-03-11T08:59:49.531"),
                        Tags = [
                          "Tag3"
                        ],
                        SendNotification = true
                },
            ];

        public Record NewRecordToCreate = new() {
            BookId = new Guid("1faaab6f-fbbb-406e-9a28-c4052ca131a5"),
            LendedFrom = DateTime.Today,
            LendedTo = DateTime.Today.AddDays(1),
            Tags = [
               "NewlyAddedTag"
            ],
            SendNotification = true
        };
        public RecordDto RecordDtoToUpdate = new() {
            RecordId = new Guid("b9aece44-1ec7-46f2-86d1-dbe01a1ef6de"),
            BookId = new Guid("1faaab6f-fbbb-406e-9a28-c4052ca131a5"),
            LendedFrom = DateTime.Parse("2024-03-11T08:59:49.531"),
            LendedTo = DateTime.Parse("2024-03-11T08:59:49.531"),
            Tags = [
               "NewTag"
            ],
            SendNotification = true
        };
        public Guid RecordIdToFind = new("b9aece44-1ec7-46f2-86d1-dbe01a1ef6de");
        public Guid RecordIdToDelete = new("8c28c9cf-06e8-4d0f-8ce6-3b84c93fff18");
        public RecordController_Fixture()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "RecordsDatabase")
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


            foreach (RecordDto recordDto in FakeRecordDtoList)
            {
                await AppDbContext.Records.AddAsync(recordDto);
            }

            await AppDbContext.SaveChangesAsync();
        }
    }
}
