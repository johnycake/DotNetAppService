using AutoMapper;
using DotNetCoreApp1.Controllers.Types;
using DotNetCoreApp1.Models;
using DotNetCoreApp1.Models.Interfaces;
using DotNetCoreApp1.Models.Types;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using User = DotNetCoreApp1.Controllers.Types.User;

namespace DotNetCoreApp1.Controllers
{
    [ApiController]
    [Authorize(Policy = "MustBeAdmin")]
    [Route("api/users")]

    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly IValidator<User> _userValidator;
        private readonly IValidator<UserDto> _userDtoValidator;
        private readonly IValidator<PasswordChange> _passwordChangeValidator;
        private const int maximumPageSize = 20;

        public UsersController(
            AppDbContext appDBContext,
            IUserRepository userRepository,
            IMapper mapper,
            IValidator<User> userValidator,
            IValidator<UserDto> userDtoValidator,
            IValidator<PasswordChange> passwordChangeValidator
            )
        {
            _appDbContext = appDBContext;
            _userRepository = userRepository;
            _mapper = mapper;
            _userValidator = userValidator;
            _userDtoValidator = userDtoValidator;
            _passwordChangeValidator = passwordChangeValidator;
        }

        [HttpGet]
        public async Task<ActionResult<UserDto>> GetUsers(
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

            var (foundUsers, paginationMetadata) = await _userRepository.GetUsers(orderBy, searchQuery, order?.Equals("DESC"), pageNumber, pageSize);

            if (paginationMetadata != null)
            {
                Response.Headers["X-Pagination"] = JsonSerializer.Serialize(paginationMetadata);
            }

            return Ok(foundUsers);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDto>> GetUser(Guid userId)
        {
            var foundUser = await _userRepository.GetUser(userId);
            if (foundUser == null) { return BadRequest("User not found"); }
            return Ok(foundUser);
        }

        [HttpPost("create")]
        public async Task<ActionResult<Guid>> CreateUser(User? userToCreate)
        {
            if (userToCreate == null) { return BadRequest("User is null"); } 

            var result = await _userValidator.ValidateAsync(userToCreate);

            if (!result.IsValid)
            {
                return ValidationProblem(new ValidationProblemDetails(result.ToDictionary()));
            }

            var newUserDto = _mapper.Map<UserDto>(userToCreate);
            var passwordToCreate = _mapper.Map<PasswordDto>(new Tuple<User, UserDto>(userToCreate, newUserDto));

            await _appDbContext.Passwords.AddAsync(passwordToCreate);
            await _appDbContext.Users.AddAsync(newUserDto);
            await _appDbContext.SaveChangesAsync();

            return Ok(newUserDto.UserId);
        }

        [HttpPut]
        public async Task<ActionResult<UserDto>> UpdateUser(UserDto? userToUpdate)
        {
            if (userToUpdate == null) { return BadRequest("User is null"); }

            var result = await _userDtoValidator.ValidateAsync(userToUpdate);

            if (!result.IsValid)
            {
                return ValidationProblem(new ValidationProblemDetails(result.ToDictionary()));
            }

            _appDbContext.Users.Update(userToUpdate);
            await _appDbContext.SaveChangesAsync();
            return Ok(userToUpdate.UserId);
        }

        [HttpPut("password")]
        public async Task<ActionResult<UserDto>> UpdateUserPassword(PasswordChange? passwordToUpdate)
        {
            if (passwordToUpdate == null) { return BadRequest("Password object is null"); }
            
            var originalPassword = await _userRepository.GetPassword(passwordToUpdate.UserId);

            if (originalPassword == null) { return BadRequest("User not found"); }

            var result = await _passwordChangeValidator.ValidateAsync(passwordToUpdate);

            if (!result.IsValid)
            {
                return ValidationProblem(new ValidationProblemDetails(result.ToDictionary()));
            }

            if (!originalPassword!.PasswordValue.Equals(passwordToUpdate.OriginalPassword))
            {
                return BadRequest("Provide correct original password");
            }

            var passwordDtoToUpdate = _mapper.Map<PasswordChange, PasswordDto>(passwordToUpdate, originalPassword!);

            _appDbContext.Passwords.Update(passwordDtoToUpdate);
            await _appDbContext.SaveChangesAsync();
            return Ok(passwordToUpdate.UserId);
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult<Guid>> DeleteUser(Guid userId)
        {
            var userToDelete = await _userRepository.GetUser(userId);
            if (userToDelete == null) { return BadRequest("User not found"); }
            await _userRepository.DeleteUser(userToDelete);
            return Ok(userToDelete.UserId);
        }
    }
}
