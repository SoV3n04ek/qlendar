using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QlendarBackend.Qlendar.Models;

namespace QlendarBackend.Qlendar.Infrastructure
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public DbSet<Todo> Todos { get; set; }
        public DbSet<SubTask> SubTasks { get; set; }
        public DbSet<TodoTag> Tags { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : 
            base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Todo>(entity =>
            {
                entity.HasMany(t => t.SubTasks)
                    .WithOne()
                    .HasForeignKey(st => st.TodoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(t => t.Tags)
                    .WithMany(t => t.Todos);

            });
        }
    }
}