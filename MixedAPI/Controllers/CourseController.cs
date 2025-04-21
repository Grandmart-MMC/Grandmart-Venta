using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MixedAPI.Models;

namespace MixedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CourseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /api/Course?lang=az|en|ru
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string lang = "az")
        {
            var courses = await _context.Courses.ToListAsync();

            var result = courses.Select(course => new
            {
                course.Id,

                Title = GetTitleByLang(course, lang),

                Description = GetDescriptionByLang(course, lang),

                ProgramDuration = GetProgramDurationByLang(course, lang),

                IconUrl = $"{Request.Scheme}://{Request.Host}/CourseIcons/{course.Icon}",

                course.Participants,

            });

            return Ok(result);
        }

        // GET /api/Course/3?lang=az|en|ru
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] string lang = "az")
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound("Kurs tapılmadı");
            var result = new
            {
                course.Id,
                Title = GetTitleByLang(course, lang),
                Description = GetDescriptionByLang(course, lang),
                ProgramDuration = GetProgramDurationByLang(course, lang),
                IconUrl = $"{Request.Scheme}://{Request.Host}/CourseIcons/{course.Icon}",
                course.Participants
            };
            return Ok(result);
        }

        // Helper metodlar:
        private string GetTitleByLang(Course course, string lang)
        {
            return lang.ToLower() switch
            {
                "az" => course.TitleAz,
                "en" => course.TitleEn,
                "ru" => course.TitleRu,
                _ => course.TitleAz // Default AZ
            };
        }
        private string GetDescriptionByLang(Course course, string lang)
        {
            return lang.ToLower() switch
            {
                "az" => course.DescriptionAz,
                "en" => course.DescriptionEn,
                "ru" => course.DescriptionRu,
                _ => course.DescriptionAz
            };
        }
         private string GetProgramDurationByLang(Course course, string lang)
        {
            return lang.ToLower() switch
            {
                "az" => course.ProgramDurationAz,
                "en" => course.ProgramDurationEn,
                "ru" => course.ProgramDurationRu,
                _ => course.ProgramDurationAz
            };
        }
    }
}
