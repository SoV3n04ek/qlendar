using Microsoft.EntityFrameworkCore;
using QlendarBackend.Qlendar.Infrastructure;
using QlendarBackend.Qlendar.Models;

namespace QlendarBackend.Qlendar.Services
{
    public class TodoService
    {
        private readonly AppDbContext _context;

        public TodoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Todo>> GetTodosForUserAsync(string userId)
        {
            return await _context.Todos
                .Include(t => t.SubTasks)
                .Include(t => t.Tags)
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<Todo> GetTodoByIdAsync(int id, string userId)
        {
            var todo = await _context.Todos
                .Include(t => t.SubTasks)
                .Include(t => t.Tags)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (todo == null)
                throw new KeyNotFoundException("Todo not found or access denied.");

            return todo;
        }

        public async Task<Todo> CreateTodoAsync(Todo todo, string userId)
        {
            todo.UserId = userId;
            todo.CreatedAt = DateTime.UtcNow;

            // Attach tags if needed (assuming tags already exist)
            if (todo.Tags != null && todo.Tags.Count > 0)
            {
                var tagIds = todo.Tags.Select(t => t.Id).ToList();
                var existingTags = await _context.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync();
                todo.Tags = existingTags;
            }

            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();
            return todo;
        }

        public async Task<Todo> UpdateTodoAsync(Todo updatedTodo, string userId)
        {
            var existingTodo = await GetTodoByIdAsync(updatedTodo.Id, userId);

            // Update simple properties
            existingTodo.Title = updatedTodo.Title;
            existingTodo.Description = updatedTodo.Description;
            existingTodo.IsCompleted = updatedTodo.IsCompleted;
            existingTodo.DueDate = updatedTodo.DueDate;

            // Update Tags
            if (updatedTodo.Tags != null)
            {
                existingTodo.Tags.Clear();
                var tagIds = updatedTodo.Tags.Select(t => t.Id).ToList();
                var tags = await _context.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync();
                foreach (var tag in tags)
                {
                    existingTodo.Tags.Add(tag);
                }
            }

            // Update SubTasks
            if (updatedTodo.SubTasks != null)
            {
                // Remove old subtasks
                _context.SubTasks.RemoveRange(existingTodo.SubTasks);
                // Add new ones (reset their IDs)
                foreach (var sub in updatedTodo.SubTasks)
                {
                    sub.Id = 0;
                    sub.TodoId = existingTodo.Id;
                }
                existingTodo.SubTasks = updatedTodo.SubTasks;
            }

            await _context.SaveChangesAsync();
            return existingTodo;
        }

        public async Task DeleteTodoAsync(int id, string userId)
        {
            var todo = await GetTodoByIdAsync(id, userId);
            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
        }
    }
}