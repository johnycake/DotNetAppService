using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetCoreApp1.Controllers.Types
{
    public class User
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required string FirstName { get; set; }
        public required string Surname { get; set; }
        public required string RoleId { get; set; }
    }
}
