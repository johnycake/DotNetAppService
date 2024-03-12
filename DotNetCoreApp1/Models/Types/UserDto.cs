using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetCoreApp1.Models.Types
{
    public class UserDto
    {
        [Key]
        public required Guid UserId { get; set; }
        public required string UserName { get; set; }
        public required string FirstName { get; set; }
        public required string Surname { get; set; }
        public required string RoleId { get; set; }
    }
}
