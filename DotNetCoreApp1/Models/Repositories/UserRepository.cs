using DotNetCoreApp1.Models.Interfaces;
using DotNetCoreApp1.Models.Types;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreApp1.Models.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _appDbContext;

        public UserRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            return await _appDbContext.Users.ToListAsync();
        }

        public async Task<(IEnumerable<UserDto>, PaginationMetadata?)> GetUsers(string? orderBy, string? searchQuery, bool? descending, int? pageNumber, int? pageSize)
        {
            var collection = _appDbContext.Users as IQueryable<UserDto>;
            PaginationMetadata? paginationMetadata = null;

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(c => 
                    c.UserName.Contains(searchQuery) 
                    || (!string.IsNullOrEmpty(c.FirstName) && c.FirstName.Contains(searchQuery)) 
                    || (!string.IsNullOrEmpty(c.Surname) && c.Surname.Contains(searchQuery))
                );
            }

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                orderBy = orderBy.Trim();
                collection = (descending ?? false)
                    ? collection.OrderByDescending(c => EF.Property<object>(c, orderBy))
                    : collection.OrderBy(c => EF.Property<object>(c, orderBy));
            }
            else
            {
                collection = collection.OrderBy(c => c.UserName);
            }

            if (pageNumber != null && pageSize != null)
            {
                var totalItemCount = await collection.CountAsync();
                paginationMetadata = new PaginationMetadata(totalItemCount, (int)pageSize, (int)pageNumber);

                collection = collection
                    .Skip((int)(pageSize * (pageNumber - 1)))
                    .Take((int)pageSize);
            }

            var collectionToReturn = await collection.ToListAsync();
            return (collectionToReturn, paginationMetadata);
        }

        public async Task<UserDto?> GetUserByCredentials(string? userName, string? password)
        {
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
            {
                return null;
            }

            var foundPassword = await _appDbContext.Passwords.FirstOrDefaultAsync(p => p.PasswordValue == password && p.UserId == user.UserId);

            return foundPassword != null ? user : null;
        }

        public async Task<UserDto?> GetUser(Guid userId)
        {
            return await _appDbContext.Users.AsNoTracking().FirstOrDefaultAsync(r => r.UserId == userId);
        }

        public async Task UpdateUser(UserDto userToUpdate)
        {
            _appDbContext.Users.Update(userToUpdate);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task DeleteUser(UserDto userToDelete)
        {
            var passwordToDelete = await GetPassword(userToDelete.UserId);

            if (passwordToDelete == null) 
            {
                throw new MissingMemberException("Missing password field for this User!");
            }

            _ = _appDbContext.Remove(passwordToDelete!);
            _appDbContext.Remove(userToDelete);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<PasswordDto?> GetPassword(Guid userId)
        {
            return await _appDbContext.Passwords.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task UpdatePassword(PasswordDto passwordToUpdate)
        {
            _appDbContext.Passwords.Update(passwordToUpdate);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<bool> IsUserRoleValid(string? userRoleIdToCheck)
        {
            return await _appDbContext.Users.AnyAsync(u => u.RoleId == userRoleIdToCheck);
        }
    }
}
