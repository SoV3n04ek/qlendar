namespace QlendarBackend.Qlendar.Models
{
    public class SubTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
        public int Order { get; set; }
        public int TodoId { get; set; } 
    }
}