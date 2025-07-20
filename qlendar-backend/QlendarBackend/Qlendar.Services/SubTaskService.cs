using QlendarBackend.Qlendar.Infrastructure;
using QlendarBackend.Qlendar.Models;

namespace QlendarBackend.Qlendar.Services
{
    public class SubTaskService
    {
        private readonly AppDbContext _context;
        public SubTaskService(AppDbContext context) => _context = context;

        public async Task<SubTask> GetByIdAsync(int id)
        {
            var subTask = await _context.SubTasks.FindAsync(id);
            if (subTask == null)
                throw new KeyNotFoundException("SubTask not found.");
            return subTask;
        }

        public async Task<SubTask> CreateAsync(SubTask subTask)
        {
            _context.SubTasks.Add(subTask);
            await _context.SaveChangesAsync();
            return subTask;
        }

        public async Task<SubTask> UpdateAsync(SubTask subTask)
        {
            var existing = await _context.SubTasks.FindAsync(subTask.Id);
            if (existing == null)
                throw new KeyNotFoundException("SubTask not found");

            existing.Title = subTask.Title;
            existing.IsCompleted = subTask.IsCompleted;
            existing.Order = subTask.Order;
            existing.TodoId = subTask.TodoId;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task DeleteAsync(int id)
        {
            var subTask = await _context.SubTasks.FindAsync(id);
            if (subTask == null)
                throw new KeyNotFoundException("SubTask not found");

            _context.SubTasks.Remove(subTask);
            await _context.SaveChangesAsync();
        }
    }
}
