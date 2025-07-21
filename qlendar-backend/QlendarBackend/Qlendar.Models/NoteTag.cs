namespace QlendarBackend.Qlendar.Models
{
    public class NoteTag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ColorHex { get; set; } = "#3b82f6"; // Default color
        public List<Note> Notes { get; set; } = new(); // Navigation property for related notes
    }
}