using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QlendarBackend.Qlendar.Infrastructure;
using QlendarBackend.Qlendar.Models;
using System.Security.Claims;

namespace QlendarBackend.Qlendar.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class NoteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NoteController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Note>>> GetNotes()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.Notes
                .Where(n => n.UserId == userId)
                .Include(n => n.Tags)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Note>> GetNote(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var note = await _context.Notes
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note == null) return NotFound();

            return note;
        }

        [HttpPost]
        public async Task<ActionResult<Note>> CreateNote([FromBody] Note note)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            note.UserId = userId;

            // Handle tags (existing or new)
            if (note.Tags != null && note.Tags.Any())
            {
                var existingTagIds = note.Tags.Where(t => t.Id != 0).Select(t => t.Id).ToList();
                var existingTags = await _context.NoteTags.Where(t => existingTagIds.Contains(t.Id)).ToListAsync();

                note.Tags = note.Tags.Select(t =>
                    t.Id == 0 ? t : existingTags.FirstOrDefault(et => et.Id == t.Id) ?? t
                ).ToList();
            }

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNote), new { id = note.Id }, note);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNote(int id, [FromBody] Note note)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingNote = await _context.Notes
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (existingNote == null) return NotFound();

            // Update properties
            existingNote.Title = note.Title;
            existingNote.Content = note.Content;

            // Handle tags update
            if (note.Tags != null)
            {
                // Clear existing tags
                existingNote.Tags.Clear();

                // Add new tags (either existing or new)
                foreach (var tag in note.Tags)
                {
                    var existingTag = tag.Id != 0
                        ? await _context.NoteTags.FindAsync(tag.Id)
                        : null;

                    existingNote.Tags.Add(existingTag ?? tag);
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note == null) return NotFound();

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("tags")]
        public async Task<ActionResult<IEnumerable<NoteTag>>> GetTags()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return await _context.NoteTags
                .Where(t => t.Notes.Any(n => n.UserId == userId))
                .ToListAsync();
        }

        [HttpPost("tags")]
        public async Task<ActionResult<NoteTag>> CreateTag([FromBody] NoteTag tag)
        {
            _context.NoteTags.Add(tag);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTags), new { id = tag.Id }, tag);
        }
    }
}