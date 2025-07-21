using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QlendarBackend.Qlendar.Models;
using System.Reflection.Emit;

namespace QlendarBackend.Qlendar.Infrastructure
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public DbSet<Todo> Todos { get; set; }
        public DbSet<SubTask> SubTasks { get; set; }
        public DbSet<TodoTag> Tags { get; set; }

        public DbSet<Note> Notes { get; set; }
        public DbSet<NoteTag> NoteTags { get; set; }

         public AppDbContext(DbContextOptions<AppDbContext> options) : 
            base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Todo relationships
            builder.Entity<Todo>(entity =>
            {
                // One-to-Many: Todo -> SubTasks
                entity.HasMany(t => t.SubTasks)
                    .WithOne()
                    .HasForeignKey(st => st.TodoId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Many-to-Many: Todo <-> Tag
                entity.HasMany(t => t.Tags)
                    .WithMany(t => t.Todos);
            });

            // Configure Note relationships
            builder.Entity<Note>(entity =>
            {
                // Many-to-Many: Note <-> Tag
                entity.HasMany(n => n.Tags)
                    .WithMany(t => t.Notes);
            });
        }   

    }
}