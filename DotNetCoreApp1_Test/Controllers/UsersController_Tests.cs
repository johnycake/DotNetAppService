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
    public class Users : IClassFixture<UsersController_Fixture>
    {
        private readonly UsersController_Fixture fixture;
        private UserValidator UserValidator { get; }
        private UserDtoValidator UserDtoValidator { get; }
        private PasswordChangeValidator PasswordChangeValidator { get; }
        private IMapper Mapper { get; }
        public Users(UsersController_Fixture usersController_Fixture)
        {
            fixture = usersController_Fixture;
            UserValidator = new UserValidator();
            UserDtoValidator = new UserDtoValidator();
            PasswordChangeValidator = new PasswordChangeValidator();
            Mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new UserProfile())));
        }

        [Fact]
        public async Task Create_User_Async()
        {
            // Arrange
            using var myAppDbContext = fixture.GetContext();
            var myUserRepository = new UserRepository(myAppDbContext);
            UsersController sut = new(myAppDbContext, myUserRepository, Mapper, UserValidator, UserDtoValidator, PasswordChangeValidator);

            // Act
            var actionResult = await sut.CreateUser(fixture.NewUserToCreate);

            // Assert
            Assert.NotNull(actionResult);
            var result = (actionResult.Result as OkObjectResult)?.Value as Guid?;
            var foundNewlyAddedUser = myAppDbContext.Users.AsNoTracking().FirstOrDefault(u => u.UserId == result);
            Assert.Equal("Jablko", foundNewlyAddedUser?.Surname);
            var foundNewlyAddedPassword = myAppDbContext.Passwords.AsNoTracking().FirstOrDefault(p => p.UserId == result);
            Assert.Equal("NejakeHeslo123?", foundNewlyAddedPassword?.PasswordValue);
        }

        [Fact]
        public async void Find_User_By_ID()
        {
            // Arrange
            var myUserDtoContextMock = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            myUserDtoContextMock
                .Setup(x => x.Users)
                .ReturnsDbSet(UsersController_Fixture.FakeUserDtoList);
            var myUserRepository = new UserRepository(myUserDtoContextMock.Object);
            UsersController sut = new(myUserDtoContextMock.Object, myUserRepository, Mapper, UserValidator, UserDtoValidator, PasswordChangeValidator);

            // Act
            var actionResult = await sut.GetUser(new Guid("2f4c995f-4502-4f29-9b2d-26ff6f0b765c"));

            // Assert
            Assert.NotNull(actionResult);
            var result = actionResult.Result as OkObjectResult;
            var userDtoSutResult = result?.Value as UserDto;
            Assert.Equal("Cibula", userDtoSutResult?.Surname);
        }

        [Fact]
        public async Task Delete_User_Async()
        {
            // Arrange
            using var myAppDbContext = fixture.GetContext();
            var myUserRepository = new UserRepository(myAppDbContext);
            UsersController sut = new(myAppDbContext, myUserRepository, Mapper, UserValidator, UserDtoValidator, PasswordChangeValidator);
            
            // Act
            var actionResult = await sut.DeleteUser(fixture.userIdToDelete);

            // Assert
            Assert.NotNull(actionResult);
            var result = (actionResult.Result as OkObjectResult)?.Value as Guid?;
            Assert.Equal(fixture.userIdToDelete, result);
            var deletedUserSearchResult = myAppDbContext.Users.AsNoTracking().FirstOrDefault(u => u.UserId == result);
            Assert.Null(deletedUserSearchResult);
        }

        [Fact]
        public async Task Update_User_Async()
        {
            // Arrange
            using var myAppDbContext = fixture.GetContext();
            var myUserRepository = new UserRepository(myAppDbContext);
            UsersController sut = new(myAppDbContext, myUserRepository, Mapper, UserValidator, UserDtoValidator, PasswordChangeValidator);

            // Act
            var actionResult = await sut.UpdateUser(fixture.userDtoToUpdate);

            // Assert
            Assert.NotNull(actionResult);
            var result = (actionResult.Result as OkObjectResult)?.Value as Guid?;
            var foundNewlyUpdatedUser = myAppDbContext.Users.AsNoTracking().FirstOrDefault(u => u.UserId == result);
            Assert.Equal("Ceresna", foundNewlyUpdatedUser?.Surname);
        }
    }
}