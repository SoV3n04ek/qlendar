using Microsoft.EntityFrameworkCore;
using QlendarBackend.Qlendar.Infrastructure;
using QlendarBackend.Qlendar.Models;

namespace QlendarBackend.Qlendar.Services
{
    public class NoteService : INoteService
    {
        private readonly AppDbContext _context;

        public NoteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Note>> GetNotesForUserAsync(string userId)
        {
            return await _context.Notes
                .Where(n => n.UserId == userId)
                .Include(n => n.Tags) // Include related tags
                .ToListAsync();
        }

        public async Task<Note?> GetNoteByIdAsync(int id, string userId)
        {
            return await _context.Notes
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
        }

        public async Task<Note> CreateNoteAsync(Note note, string userId)
        {
            note.UserId = userId;

            // Handle tags (attach existing ones or create new)
            if (note.Tags != null && note.Tags.Any())
            {
                var existingTagIds = note.Tags
                    .Where(t => t.Id != 0)
                    .Select(t => t.Id)
                    .ToList();

                var existingTags = await _context.NoteTags
                    .Where(t => existingTagIds.Contains(t.Id))
                    .ToListAsync();

                note.Tags = note.Tags.Select(t =>
                    t.Id == 0 ? t : existingTags.FirstOrDefault(et => et.Id == t.Id) ?? t
                ).ToList();
            }

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();
            return note;
        }

        public async Task<bool> UpdateNoteAsync(int id, Note note, string userId)
        {
            var existingNote = await _context.Notes
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (existingNote == null)
                return false;

            // Update properties
            existingNote.Title = note.Title;
            existingNote.Content = note.Content;

            // Update tags
            if (note.Tags != null)
            {
                existingNote.Tags.Clear();

                foreach (var tag in note.Tags)
                {
                    var existingTag = tag.Id != 0
                        ? await _context.NoteTags.FindAsync(tag.Id)
                        : null;

                    existingNote.Tags.Add(existingTag ?? tag);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteNoteAsync(int id, string userId)
        {
            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note == null)
                return false;

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<NoteTag>> GetTagsForUserAsync(string userId)
        {
            return await _context.NoteTags
                .Where(t => t.Notes.Any(n => n.UserId == userId))
                .ToListAsync();
        }

        public async Task<NoteTag> CreateTagAsync(NoteTag tag)
        {
            _context.NoteTags.Add(tag);
            await _context.SaveChangesAsync();
            return tag;
        }
    }
}
