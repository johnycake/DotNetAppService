using AutoMapper;
using DotNetCoreApp1.AutoMapperProfiles;
using DotNetCoreApp1.Controllers;
using DotNetCoreApp1.Models.Repositories;
using DotNetCoreApp1.Models.Types;
using DotNetCoreApp1.Validators;
using DotNetCoreApp1_Test.Controllers.TestData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace UnitTesting.Controllers
{
    public class Records : IClassFixture<RecordController_Fixture>
    {
        private readonly RecordController_Fixture fixture;
        private IMapper Mapper { get; }
        public Records(RecordController_Fixture recordController_Fixture)
        {
            fixture = recordController_Fixture;
            Mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new RecordProfile())));
        }

        [Fact]
        public async Task Create_Record_Async()
        {
            // Arrange
            using var myAppDbContext = fixture.GetContext();
            var myBookRepository = new BookRepository(myAppDbContext);
            var myRecordRepository = new RecordRepository(myAppDbContext);
            RecordValidator myRecordValidator = new(myBookRepository);
            RecordDtoValidator myRecordDtoValidator = new(myBookRepository, myRecordRepository);
            RecordsController sut = new(myAppDbContext, myRecordRepository, Mapper, myRecordValidator, myRecordDtoValidator);

            // Act
            var actionResult = await sut.CreateRecord(fixture.NewRecordToCreate);

            // Assert
            Assert.NotNull(actionResult);
            var result = (actionResult.Result as OkObjectResult)?.Value as Guid?;
            var foundNewlyAddedRecord = myAppDbContext.Records.AsNoTracking().FirstOrDefault(u => u.RecordId == result);
            Assert.Equal(fixture.NewRecordToCreate.BookId, foundNewlyAddedRecord?.BookId);
        }

        [Fact]
        public async void Find_Record_By_ID()
        {
            // Arrange
            using var myAppDbContext = fixture.GetContext();
            var myBookRepository = new BookRepository(myAppDbContext);
            var myRecordRepository = new RecordRepository(myAppDbContext);
            RecordValidator myRecordValidator = new(myBookRepository);
            RecordDtoValidator myRecordDtoValidator = new(myBookRepository, myRecordRepository);
            RecordsController sut = new(myAppDbContext, myRecordRepository, Mapper, myRecordValidator, myRecordDtoValidator);

            // Act
            var actionResult = await sut.GetRecord(fixture.RecordIdToFind);

            // Assert
            Assert.NotNull(actionResult);
            var result = actionResult.Result as OkObjectResult;
            var recordDtoSutResult = result?.Value as RecordDto;
            Assert.Equal(fixture.RecordIdToFind, recordDtoSutResult?.RecordId);
        }

        [Fact]
        public async Task Delete_Record_Async()
        {
            // Arrange
            using var myAppDbContext = fixture.GetContext();
            var myBookRepository = new BookRepository(myAppDbContext);
            var myRecordRepository = new RecordRepository(myAppDbContext);
            RecordValidator myRecordValidator = new(myBookRepository);
            RecordDtoValidator myRecordDtoValidator = new(myBookRepository, myRecordRepository);
            RecordsController sut = new(myAppDbContext, myRecordRepository, Mapper, myRecordValidator, myRecordDtoValidator);

            // Act
            var actionResult = await sut.DeleteRecord(fixture.RecordIdToDelete);

            // Assert
            Assert.NotNull(actionResult);
            var result = (actionResult.Result as OkObjectResult)?.Value as Guid?;
            Assert.Equal(fixture.RecordIdToDelete, result);
            var deletedBookSearchResult = myAppDbContext.Books.AsNoTracking().FirstOrDefault(u => u.BookId == result);
            Assert.Null(deletedBookSearchResult);
        }

        [Fact]
        public async Task Update_Record_Async()
        {
            // Arrange
            using var myAppDbContext = fixture.GetContext();
            var myBookRepository = new BookRepository(myAppDbContext);
            var myRecordRepository = new RecordRepository(myAppDbContext);
            RecordValidator myRecordValidator = new(myBookRepository);
            RecordDtoValidator myRecordDtoValidator = new(myBookRepository, myRecordRepository);
            RecordsController sut = new(myAppDbContext, myRecordRepository, Mapper, myRecordValidator, myRecordDtoValidator);

            // Act
            var actionResult = await sut.UpdateRecord(fixture.RecordDtoToUpdate);

            // Assert
            Assert.NotNull(actionResult);
            var result = (actionResult.Result as OkObjectResult)?.Value as Guid?;
            var foundNewlyUpdatedRecord = myAppDbContext.Records.AsNoTracking().FirstOrDefault(u => u.RecordId == result);
            Assert.Equal(fixture.RecordDtoToUpdate.RecordId, foundNewlyUpdatedRecord?.RecordId);
        }
    }
}