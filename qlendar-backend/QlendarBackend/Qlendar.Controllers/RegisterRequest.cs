namespace QlendarBackend.Qlendar.Controllers
{
    public class RegisterRequest
    {
        public string Email { get; set; } 
        public string Password { get; set; }
        public string? Nickname { get; set; }
    }
}
