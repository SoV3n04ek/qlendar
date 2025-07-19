namespace QlendarBackend.Qlendar.Models
{
    public class TodoTag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ColorHex { get; set; } = "#3b82f6";
        public List<Todo> Todos { get; set; }
    }
}