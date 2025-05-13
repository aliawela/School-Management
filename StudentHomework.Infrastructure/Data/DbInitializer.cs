using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudentHomework.Domain.Entities;

namespace StudentHomework.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new StudentHomeworkDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<StudentHomeworkDbContext>>());

            // Ensure database and tables are created before seeding
            context.Database.Migrate();

            if (context.Students.Any() || context.Courses.Any())
                return; // DB has been seeded

            var courses = new List<Course>
            {
                new() { CourseName = "Mathematics", CourseDescription = "Algebra, Geometry, Calculus" },
                new() { CourseName = "Physics", CourseDescription = "Mechanics, Thermodynamics" },
                new() { CourseName = "Chemistry", CourseDescription = "Organic, Inorganic, Physical" },
                new() { CourseName = "Biology", CourseDescription = "Botany, Zoology" },
                new() { CourseName = "History", CourseDescription = "World and Local History" },
                new() { CourseName = "English", CourseDescription = "Literature and Language" },
                new() { CourseName = "Computer Science", CourseDescription = "Programming, Algorithms" },
                new() { CourseName = "Art", CourseDescription = "Drawing, Painting" },
                new() { CourseName = "Music", CourseDescription = "Theory, Practice" },
                new() { CourseName = "Economics", CourseDescription = "Micro, Macro" },
                new() { CourseName = "Philosophy", CourseDescription = "Ethics, Logic" },
                new() { CourseName = "Geography", CourseDescription = "Physical, Human Geography" },
                new() { CourseName = "French", CourseDescription = "Language and Culture" },
                new() { CourseName = "Spanish", CourseDescription = "Language and Culture" },
                new() { CourseName = "Physical Education", CourseDescription = "Sports, Health" }
            };
            context.Courses.AddRange(courses);
            context.SaveChanges();

            var students = new List<Student>
            {
                new() { Name = "John Doe", Email = "john.doe@student.com", DateOfBirth = new DateTime(2005, 1, 10) },
                new() { Name = "Jane Roe", Email = "jane.roe@student.com", DateOfBirth = new DateTime(2004, 5, 22) },
                new() { Name = "Sam Green", Email = "sam.green@student.com", DateOfBirth = new DateTime(2006, 3, 15) },
                new() { Name = "Lisa White", Email = "lisa.white@student.com", DateOfBirth = new DateTime(2005, 7, 30) },
                new() { Name = "Tom Black", Email = "tom.black@student.com", DateOfBirth = new DateTime(2004, 11, 2) },
                new() { Name = "Anna Blue", Email = "anna.blue@student.com", DateOfBirth = new DateTime(2006, 9, 18) },
                new() { Name = "Mike Red", Email = "mike.red@student.com", DateOfBirth = new DateTime(2005, 2, 25) },
                new() { Name = "Sara Yellow", Email = "sara.yellow@student.com", DateOfBirth = new DateTime(2004, 12, 12) },
                new() { Name = "Chris Orange", Email = "chris.orange@student.com", DateOfBirth = new DateTime(2006, 6, 5) },
                new() { Name = "Nina Purple", Email = "nina.purple@student.com", DateOfBirth = new DateTime(2005, 8, 21) },
                new() { Name = "Paul Gray", Email = "paul.gray@student.com", DateOfBirth = new DateTime(2004, 4, 14) },
                new() { Name = "Olga Pink", Email = "olga.pink@student.com", DateOfBirth = new DateTime(2006, 10, 9) },
                new() { Name = "Ivan Gold", Email = "ivan.gold@student.com", DateOfBirth = new DateTime(2005, 12, 1) },
                new() { Name = "Emma Silver", Email = "emma.silver@student.com", DateOfBirth = new DateTime(2004, 3, 8) },
                new() { Name = "Alex Bronze", Email = "alex.bronze@student.com", DateOfBirth = new DateTime(2006, 1, 19) }
            };
            // Assign students to random courses
            var rand = new Random();
            foreach (var student in students)
            {
                var courseCount = rand.Next(2, 5);
                var studentCourses = courses.OrderBy(_ => rand.Next()).Take(courseCount).ToList();
                foreach (var course in studentCourses)
                {
                    student.Courses.Add(course);
                }
            }
            context.Students.AddRange(students);
            context.SaveChanges();
        }
    }
}
