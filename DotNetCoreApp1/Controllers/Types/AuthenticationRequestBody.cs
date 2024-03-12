namespace DotNetCoreApp1.Controllers.Requests
{
    public class AuthenticationRequestBody
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}
