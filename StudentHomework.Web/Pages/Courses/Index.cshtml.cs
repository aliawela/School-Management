using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentHomework.Application.Managers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentHomework.Web.Pages.Courses
{
    public class IndexModel : PageModel
    {
        private readonly CourseManager _courseManager;
        public IndexModel(CourseManager courseManager)
        {
            _courseManager = courseManager;
        }

        public List<CourseEditViewModel> Courses { get; set; } = new();
        public _CourseEditModalModel CourseEditModal { get; set; } = new();
        public _CourseDeleteModalModel CourseDeleteModal { get; set; } = new();
        public _CourseToastModel CourseToast { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? TitleFilter { get; set; }
        [BindProperty]
        public CourseEditViewModel Course { get; set; } = new();

        public async Task OnGetAsync()
        {
            var courses = await _courseManager.FilterCoursesByNameAsync(TitleFilter ?? "");
            Courses = courses.Select(c => new CourseEditViewModel
            {
                CourseId = c.CourseId,
                Title = c.CourseName ?? string.Empty,
                CourseDescription = c.CourseDescription ?? string.Empty
            }).ToList();
        }

        public async Task<IActionResult> OnGetFilterCoursesByNameAsync(string name)
        {
            var courses = await _courseManager.FilterCoursesByNameAsync(name);
            var result = courses.Select(c => new CourseEditViewModel
            {
                CourseId = c.CourseId,
                Title = c.CourseName ?? string.Empty,
                CourseDescription = c.CourseDescription ?? string.Empty
            }).ToList();
            return new JsonResult(result);
        }

        public async Task<IActionResult> OnPostAddCourseAjaxAsync([FromForm] CourseEditViewModel course)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var courseDto = new StudentHomework.Application.DTOs.CourseDto
            {
                CourseName = course.Title,
                CourseDescription = course.CourseDescription
            };
            await _courseManager.AddAsync(courseDto);
            return new JsonResult(new {
                courseId = courseDto.CourseId,
                title = courseDto.CourseName,
                courseDescription = courseDto.CourseDescription
            });
        }

        public async Task<IActionResult> OnPostEditCourseAjaxAsync([FromForm] CourseEditViewModel course)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var courseDto = new StudentHomework.Application.DTOs.CourseDto
            {
                CourseId = course.CourseId,
                CourseName = course.Title,
                CourseDescription = course.CourseDescription
            };
            await _courseManager.UpdateAsync(courseDto);
            return new JsonResult(new {
                courseId = courseDto.CourseId,
                title = courseDto.CourseName,
                courseDescription = courseDto.CourseDescription
            });
        }

        public async Task<IActionResult> OnPostDeleteCourseAjaxAsync([FromForm] int courseId)
        {
            await _courseManager.DeleteAsync(courseId);
            return new JsonResult(new { courseId });
        }

        public async Task<IActionResult> OnPostDeleteCoursesAsync(string CourseIds)
        {
            System.Diagnostics.Debug.WriteLine($"Received CourseIds: {CourseIds}");
            if (!string.IsNullOrWhiteSpace(CourseIds))
            {
                var ids = CourseIds.Split(',', System.StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.TryParse(id, out var i) ? (int?)i : null)
                    .Where(id => id.HasValue)
                    .Select(id => id!.Value)
                    .ToList();
                if (ids.Any())
                {
                    foreach (var id in ids)
                    {
                        await _courseManager.DeleteAsync(id);
                    }
                }
            }
            return new JsonResult(new { success = true });
        }
    }

    public class _CourseEditModalModel
    {
        public string ModalTitle { get; set; } = string.Empty;
        public string SubmitButtonText { get; set; } = string.Empty;
        public CourseEditViewModel Course { get; set; } = new();
    }
    public class _CourseDeleteModalModel
    {
        public string CourseTitles { get; set; } = string.Empty;
    }
    public class _CourseToastModel
    {
        public string Message { get; set; } = string.Empty;
    }
    public class CourseEditViewModel
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CourseDescription { get; set; } = string.Empty;
    }
}
