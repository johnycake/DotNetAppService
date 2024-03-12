
namespace DotNetCoreApp1.Controllers.Types
{
    public class PasswordChange
    {
        public required Guid UserId { get; set; }
        public required string OriginalPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
