using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentHomework.Infrastructure.Data;
using StudentHomework.Application.Managers;
using StudentHomework.Application.DTOs;

namespace StudentHomework.Web.Pages.Students
{
    public class IndexModel : PageModel
    {
        private readonly StudentHomeworkDbContext _context;
        private readonly StudentManager _studentManager;
        private readonly CourseManager _courseManager;

        public IndexModel(StudentHomeworkDbContext context, StudentManager studentManager, CourseManager courseManager)
        {
            _context = context;
            _studentManager = studentManager;
            _courseManager = courseManager;
        }

        public List<StudentEditViewModel> Students { get; set; } = new();
        public _StudentEditModalModel StudentEditModal { get; set; } = new();
        public _StudentDeleteModalModel StudentDeleteModal { get; set; } = new();
        public _StudentToastModel StudentToast { get; set; } = new();

        [BindProperty]
        public string? StudentIds { get; set; } // Make nullable for add
        [BindProperty]
        public StudentEditViewModel Student { get; set; } = new();

        public void OnGet()
        {
            Students = _context.Students
                .Include(s => s.Courses)
                .Select(s => new StudentEditViewModel
                {
                    StudentId = s.StudentId,
                    Name = s.Name,
                    Email = s.Email,
                    DateOfBirth = s.DateOfBirth,
                    Courses = s.Courses.Select(c => new CourseViewModel
                    {
                        CourseId = c.CourseId,
                        CourseName = c.CourseName
                    }).ToList()
                }).ToList();
        }

        public async Task<IActionResult> OnGetStudentInfoAsync(int id)
        {
            var student = await _context.Students
                .Include(s => s.Courses)
                .FirstOrDefaultAsync(s => s.StudentId == id);
            if (student == null)
                return new JsonResult(new { });
            var result = new StudentInfoViewModel
            {
                StudentId = student.StudentId,
                Name = student.Name ?? string.Empty,
                Email = student.Email ?? string.Empty,
                DateOfBirth = student.DateOfBirth.ToShortDateString(),
                Courses = student.Courses?.Select(c => new StudentInfoViewModel.CourseInfo { CourseName = c.CourseName }).ToList() ?? new List<StudentInfoViewModel.CourseInfo>()
            };
            return new JsonResult(result);
        }

        public async Task<IActionResult> OnGetAvailableCoursesAsync(int id)
        {
            var student = await _context.Students.Include(s => s.Courses).FirstOrDefaultAsync(s => s.StudentId == id);
            if (student == null)
                return new JsonResult(new List<CourseViewModel>());
            var enrolledIds = student.Courses.Select(c => c.CourseId).ToList();
            var availableCourses = await _context.Courses
                .Where(c => !enrolledIds.Contains(c.CourseId))
                .Select(c => new CourseViewModel { CourseId = c.CourseId, CourseName = c.CourseName })
                .ToListAsync();
            return new JsonResult(availableCourses);
        }

        public IActionResult OnPost()
        {
            if (!string.IsNullOrWhiteSpace(StudentIds))
            {
                var ids = StudentIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.TryParse(id, out var i) ? (int?)i : null)
                    .Where(id => id.HasValue)
                    .Select(id => id!.Value)
                    .ToList();
                if (ids.Any())
                {
                    var students = _context.Students.Where(s => ids.Contains(s.StudentId)).ToList();
                    _context.Students.RemoveRange(students);
                    _context.SaveChanges();
                }
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddStudentAsync()
        {
            ModelState.Remove("StudentIds");
            if (!ModelState.IsValid)
            {
                OnGet();
                return Page();
            }
            if (Student.StudentId > 0)
            {
                // Edit existing student
                var student = _context.Students.FirstOrDefault(s => s.StudentId == Student.StudentId);
                if (student != null)
                {
                    student.Name = Student.Name!;
                    student.Email = Student.Email!;
                    student.DateOfBirth = Student.DateOfBirth;
                    _context.SaveChanges();
                }
            }
            else
            {
                // Add new student using StudentManager.AddAsync
                var studentDto = new StudentHomework.Application.DTOs.StudentDto
                {
                    Name = Student.Name!,
                    Email = Student.Email!,
                    DateOfBirth = Student.DateOfBirth
                };
                await _studentManager.AddAsync(studentDto);
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddStudentAjaxAsync([FromForm] StudentEditViewModel student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var studentDto = new StudentHomework.Application.DTOs.StudentDto
            {
                Name = student.Name!,
                Email = student.Email!,
                DateOfBirth = student.DateOfBirth
            };
            await _studentManager.AddAsync(studentDto);
            return new JsonResult(new {
                studentId = studentDto.StudentId,
                name = studentDto.Name,
                email = studentDto.Email,
                dateOfBirth = studentDto.DateOfBirth.ToString("yyyy-MM-dd")
            });
        }

        public async Task<IActionResult> OnPostEditStudentAjaxAsync([FromForm] StudentEditViewModel student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var studentDto = new StudentHomework.Application.DTOs.StudentDto
            {
                StudentId = student.StudentId,
                Name = student.Name!,
                Email = student.Email!,
                DateOfBirth = student.DateOfBirth
            };
            await _studentManager.UpdateAsync(studentDto);
            return new JsonResult(new {
                studentId = studentDto.StudentId,
                name = studentDto.Name,
                email = studentDto.Email,
                dateOfBirth = studentDto.DateOfBirth.ToString("yyyy-MM-dd")
            });
        }

        public async Task<IActionResult> OnPostDeleteStudentAjaxAsync([FromForm] int studentId)
        {
            await _studentManager.DeleteAsync(studentId);
            return new JsonResult(new { studentId });
        }

        public async Task<IActionResult> OnPostDeleteStudentsAsync()
        {
            if (!string.IsNullOrWhiteSpace(StudentIds))
            {
                Console.WriteLine($"Received StudentIds: {StudentIds}"); // Debugging log
                var ids = StudentIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.TryParse(id, out var i) ? (int?)i : null)
                    .Where(id => id.HasValue)
                    .Select(id => id!.Value)
                    .ToList();
                if (ids.Any())
                {
                    var students = _context.Students.Where(s => ids.Contains(s.StudentId)).ToList();
                    _context.Students.RemoveRange(students);
                    await _context.SaveChangesAsync();
                }
            }
            return new JsonResult(new { success = true });
        }

        public async Task<IActionResult> OnPostRegisterCourseAsync(int studentId, int courseId)
        {
            var student = await _context.Students.Include(s => s.Courses).FirstOrDefaultAsync(s => s.StudentId == studentId);
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);
            if (student == null || course == null)
                return new JsonResult(new { success = false, message = "Student or course not found." });
            if (student.Courses.Any(c => c.CourseId == courseId))
                return new JsonResult(new { success = false, message = "Student already enrolled in this course." });
            // Ensure course is attached to context
            if (_context.Entry(course).State == EntityState.Detached)
            {
                _context.Courses.Attach(course);
            }
            student.Courses.Add(course);
            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }

        public async Task<IActionResult> OnPostDropCourseAsync(DropCourseRequest req)
        {
            System.Diagnostics.Debug.WriteLine($"DropCourseAsync called with StudentId={req.StudentId}, CourseId={req.CourseId}");
            await _studentManager.DropCourseAsync(req.StudentId, req.CourseId);
            return new JsonResult(new { success = true });
        }
    }

    public class _StudentDeleteModalModel
    {
        public string StudentNames { get; set; } = string.Empty;
    }

    public class _StudentEditModalModel
    {
        public string ModalTitle { get; set; } = string.Empty;
        public string SubmitButtonText { get; set; } = string.Empty;
        public StudentEditViewModel Student { get; set; } = new();
    }

    public class StudentEditViewModel
    {
        public int StudentId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<CourseViewModel> Courses { get; set; } = new();
    }

    public class CourseViewModel
    {
        public int CourseId { get; set; }
        public string? CourseName { get; set; }
    }

    public class _StudentToastModel
    {
        public string Message { get; set; } = string.Empty;
    }

    public class StudentInfoViewModel
    {
        public int StudentId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? DateOfBirth { get; set; }
        public List<CourseInfo> Courses { get; set; } = new();
        public class CourseInfo
        {
            public string? CourseName { get; set; }
        }
    }

    public class RegisterCourseRequest
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
    }

    public class DropCourseRequest
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
    }
}
