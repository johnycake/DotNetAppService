using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetCoreApp1.Models.Types
{
    public class PasswordDto
    {
        [Key]
        public required Guid PasswordId { get; set; }
        public required Guid UserId { get; set; }
        public required string PasswordValue { get; set; }
    }
}
