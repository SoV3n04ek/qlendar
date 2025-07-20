namespace QlendarBackend.Qlendar.DTOs
{
    public class TodoDto
    {
        public int Id { get; set; }
        public string Title { get; set; } 
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? DueDate { get; set; }
        public List<SubTaskDto> SubTasks { get; set; }
        public List<int> TagIds { get; set; }
    }
}
