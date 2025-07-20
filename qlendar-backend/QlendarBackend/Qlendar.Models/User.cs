using Microsoft.AspNetCore.Identity;

namespace QlendarBackend.Qlendar.Models
{
    public class User : IdentityUser
    {
        public string? Nickname { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<Todo> Todos { get; set; } = new();
    }
}