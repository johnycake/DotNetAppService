using AutoMapper;
using DotNetCoreApp1.Controllers.Types;
using DotNetCoreApp1.Models;
using DotNetCoreApp1.Models.Interfaces;
using DotNetCoreApp1.Models.Types;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DotNetCoreApp1.Controllers
{
    [ApiController]
    [Authorize(Policy = "MustBeUser")]
    [Route("api/records")]

    public class RecordsController : ControllerBase
    {
        private readonly IRecordRepository _recordRepository;
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly IValidator<Record> _recordValidator;
        private readonly IValidator<RecordDto> _recordDtoValidator;

        public RecordsController(
            AppDbContext appDBContext,
            IRecordRepository recordRepository,
            IMapper mapper,
            IValidator<Record> recordDtoValidator,
            IValidator<RecordDto> recordValidator
            )
        {
            _appDbContext = appDBContext;
            _recordRepository = recordRepository;
            _recordValidator = recordDtoValidator;
            _recordDtoValidator = recordValidator;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecordDto>>> GetRecords()
        {
            var allRecords = await _recordRepository.GetAllRecords();
            return Ok(allRecords);
        }

        [HttpGet("{recordId}")]
        public async Task<ActionResult<RecordDto>> GetRecord(Guid recordId)
        {
            var foundRecord = await _recordRepository.GetRecord(recordId);
            if (foundRecord == null) { return BadRequest("Record with this ID not found"); }
            return Ok(foundRecord);
        }

        [HttpPost("create")]
        public async Task<ActionResult<Guid>> CreateRecord(Record recordToCreate)
        {
            if (recordToCreate == null) { return ValidationProblem("record is null"); }

            var result = await _recordValidator.ValidateAsync(recordToCreate);

            if (!result.IsValid)
            {
                return ValidationProblem(new ValidationProblemDetails(result.ToDictionary()));
            }

            var newRecord = _mapper.Map<RecordDto>(recordToCreate);

            await _appDbContext.Records.AddAsync(newRecord);
            await _appDbContext.SaveChangesAsync();
            return Ok(newRecord.RecordId);
        }

        [HttpPut]
        public async Task<ActionResult<Guid>> UpdateRecord(RecordDto recordToUpdate)
        {
            if (recordToUpdate == null) { return BadRequest("record is null"); }

            var result = await _recordDtoValidator.ValidateAsync(recordToUpdate);

            if (!result.IsValid)
            {
                return ValidationProblem(new ValidationProblemDetails(result.ToDictionary()));
            }

            _appDbContext.Records.Update(recordToUpdate);
            await _appDbContext.SaveChangesAsync();
            return Ok(recordToUpdate.RecordId);
        }

        [HttpPatch("{recordId}")]
        public async Task<ActionResult<Guid>> PartiallyUpdateRecord(Guid recordId, JsonPatchDocument<RecordDto> recordPatchDocument)
        {
            if (recordPatchDocument == null) { return BadRequest("record is null"); }
            var recordToPatch = await _recordRepository.GetRecord(recordId);
            if (recordToPatch == null) { return BadRequest("record not found"); }

            recordPatchDocument.ApplyTo<RecordDto>(recordToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(recordToPatch))
            {
                return BadRequest(ModelState);
            }

            _appDbContext.Records.Update(recordToPatch);
            await _appDbContext.SaveChangesAsync();
            return Ok(recordToPatch.RecordId);
        }

        [HttpDelete("{recordId}")]
        public async Task<ActionResult<Guid>> DeleteRecord(Guid recordId)
        {
            var recordToDelete = await _recordRepository.GetRecord(recordId);
            if (recordToDelete == null) { return BadRequest("record ID no found"); }
            await _recordRepository.DeleteRecord(recordToDelete);
            return Ok(recordToDelete.RecordId);
        }
    }
}
