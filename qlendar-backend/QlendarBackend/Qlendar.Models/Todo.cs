namespace QlendarBackend.Qlendar.Models
{
    public class Todo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }

        // 
        public List<SubTask> SubTasks { get; set; } = new();
        public List<TodoTag> Tags { get; set; } = new();
        public string UserId { get; set; }
    }
}
