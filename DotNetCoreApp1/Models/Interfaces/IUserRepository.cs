using DotNetCoreApp1.Controllers.Types;
using DotNetCoreApp1.Models.Types;

namespace DotNetCoreApp1.Models.Interfaces
{
    public interface IUserRepository
    {
        public Task<(IEnumerable<UserDto>, PaginationMetadata?)> GetUsers(string? orderBy, string? searchQuery, bool? descending, int? pageNumber, int? pageSize);
        public Task<UserDto?> GetUser(Guid userId);
        public Task<UserDto?> GetUserByCredentials(string? userName, string? password);
        public Task UpdateUser(UserDto userToUpdate);
        public Task DeleteUser(UserDto userToDelete);
        public Task<PasswordDto?> GetPassword(Guid userId);
        public Task UpdatePassword(PasswordDto passwordToUpdate);
        public Task<bool> IsUserRoleValid(string? userRoleIdToCheck);
    }
}
