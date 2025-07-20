namespace QlendarBackend.Qlendar.DTOs
{
    public class SubTaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; } 
        public bool IsCompleted { get; set; }
        public int Order { get; set; }
    }
}