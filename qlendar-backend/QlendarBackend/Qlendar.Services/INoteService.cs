using QlendarBackend.Qlendar.Models;

namespace QlendarBackend.Qlendar.Services
{
    public interface INoteService
    {
        Task<IEnumerable<Note>> GetNotesForUserAsync(string userId);
        Task<Note?> GetNoteByIdAsync(int id, string userId);
        Task<Note> CreateNoteAsync(Note note, string userId);
        Task<bool> UpdateNoteAsync(int id, Note note, string userId);
        Task<bool> DeleteNoteAsync(int id, string userId);
        Task<IEnumerable<NoteTag>> GetTagsForUserAsync(string userId);
        Task<NoteTag> CreateTagAsync(NoteTag tag);
    }
}
