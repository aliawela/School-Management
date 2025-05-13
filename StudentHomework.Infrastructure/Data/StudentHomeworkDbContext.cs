using Microsoft.EntityFrameworkCore;
using StudentHomework.Domain.Entities;

namespace StudentHomework.Infrastructure.Data
{
    public class StudentHomeworkDbContext(DbContextOptions<StudentHomeworkDbContext> options) : DbContext(options)
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Student>()
                .HasMany(s => s.Courses)
                .WithMany(c => c.Students);
        }
    }
}
