using DotNetCoreApp1.Controllers.Types;
using DotNetCoreApp1.Models;
using DotNetCoreApp1.Models.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DotNetCoreApp1_Test.Controllers.TestData
{
    public class UsersController_Fixture : IDisposable
    {
        private readonly DbContextOptions<AppDbContext> _options;
        public AppDbContext AppDbContext { get; private set; }
        public static List<UserDto> FakeUserDtoList => [
                new() {
                    UserId = new Guid("5e388433-24e2-4b97-8e68-5a7c05d191f2"),
                    UserName = "BeznyUser1",
                    FirstName = "Jozko",
                    Surname = "Mrkvicka",
                    RoleId = "User"
                },
                new() {
                    UserId = new Guid("2f4c995f-4502-4f29-9b2d-26ff6f0b765c"),
                    UserName = "BeznyUser2",
                    FirstName = "Janko",
                    Surname = "Cibula",
                    RoleId = "User"
                },
                new() {
                    UserId = new Guid("7176f3ed-3859-4f54-917f-e3ecfadb2de8"),
                    UserName = "BigBoss",
                    FirstName = "Sef",
                    Surname = "Zelovocu",
                    RoleId = "Admin"
                },
            ];
        public static List<PasswordDto> FakePasswordDtoList => [
                new() {
                    UserId = new Guid("7176f3ed-3859-4f54-917f-e3ecfadb2de8"),
                    PasswordId = new Guid("7e71829b-2d1a-4f02-8fc4-75313f124f02"),
                    PasswordValue = "SuperHeslo123!"
                },
                new() {
                    UserId = new Guid("2f4c995f-4502-4f29-9b2d-26ff6f0b765c"),
                    PasswordId = new Guid("767ac3e1-7d46-4183-a744-b97bfa4a961e"),
                    PasswordValue = "BezneHeslo123!"
                },
                new() {
                    UserId = new Guid("5e388433-24e2-4b97-8e68-5a7c05d191f2"),
                    PasswordId = new Guid("df1837f3-b398-43e2-b760-89609d4668c6"),
                    PasswordValue = "BezneHeslo123!"
                },
            ];

        public User NewUserToCreate = new() {
                    UserName = "NovyUser",
                    FirstName = "Ferko",
                    Surname = "Jablko",
                    RoleId = "User",
                    Password = "NejakeHeslo123?"
                };
        public UserDto userDtoToUpdate = new() {
                    UserId = new Guid("2f4c995f-4502-4f29-9b2d-26ff6f0b765c"),
                    UserName = "UpdatovanyUser",
                    FirstName = "Ferko",
                    Surname = "Ceresna",
                    RoleId = "User"
                };
        public Guid userIdToFind = new("7176f3ed-3859-4f54-917f-e3ecfadb2de8");
        public Guid userIdToDelete = new("5e388433-24e2-4b97-8e68-5a7c05d191f2");
        public UsersController_Fixture()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "UsersDatabase")
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
            foreach (UserDto userDto in FakeUserDtoList)
            {
                await AppDbContext.Users.AddAsync(userDto);
            }
            foreach (PasswordDto passwordDto in FakePasswordDtoList)
            {
                await AppDbContext.Passwords.AddAsync(passwordDto);
            }

            await AppDbContext.SaveChangesAsync();
        }
    }
}
