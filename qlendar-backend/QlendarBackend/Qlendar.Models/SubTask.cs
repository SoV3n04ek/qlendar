using System.ComponentModel.DataAnnotations;

namespace QlendarBackend.Qlendar.Models
{
    public class SubTask
    {
        public int Id { get; set; }
        [Required, StringLength(100, MinimumLength = 1)]
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
        public int Order { get; set; }
        public int TodoId { get; set; } 
    }
}