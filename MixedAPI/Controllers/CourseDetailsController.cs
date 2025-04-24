using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MixedAPI.Models;
using Newtonsoft.Json;

namespace MixedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseDetailsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CourseDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /api/CourseDetail?lang=az|en|ru
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string lang = "az")
        {
            var details = await _context.CourseDetails.ToListAsync();

            // Xüsusi helper metodlarla hər dilə uyğun dəyərlər qaytarırıq
            var result = details.Select(detail => new
            {
                detail.Id,
                Title = GetTitleByLang(detail, lang),
                DetailDescription = GetDetailDescriptionByLang(detail, lang),

                // Siyahıları JSON-dan parse edib qaytarırıq
                TargetAudience = GetListByLang(
                    detail.TargetAudienceAz,
                    detail.TargetAudienceEn,
                    detail.TargetAudienceRu,
                    lang),

                Curriculum = GetListByLang(
                    detail.CurriculumAz,
                    detail.CurriculumEn,
                    detail.CurriculumRu,
                    lang),

                Features = GetListByLang(
                    detail.FeaturesAz,
                    detail.FeaturesEn,
                    detail.FeaturesRu,
                    lang)
            });

            return Ok(result);
        }

        // GET /api/CourseDetail/3?lang=az|en|ru
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id, [FromQuery] string lang = "az")
        {
            var detail = await _context.CourseDetails.FindAsync(id);
            if (detail == null)
                return NotFound("Bu ID-yə uyğun CourseDetail tapılmadı.");

            var result = new
            {
                detail.Id,
                Title = GetTitleByLang(detail, lang),
                DetailDescription = GetDetailDescriptionByLang(detail, lang),

                TargetAudience = GetListByLang(
                    detail.TargetAudienceAz,
                    detail.TargetAudienceEn,
                    detail.TargetAudienceRu,
                    lang),

                Curriculum = GetListByLang(
                    detail.CurriculumAz,
                    detail.CurriculumEn,
                    detail.CurriculumRu,
                    lang),

                Features = GetListByLang(
                    detail.FeaturesAz,
                    detail.FeaturesEn,
                    detail.FeaturesRu,
                    lang)
            };

            return Ok(result);
        }

        /// <summary>
        /// GET /api/Course/GetCourseAndDetailById/{id}?lang=az|en|ru
        /// Tək bir kursun (və ona bağlı CourseDetail-in) bütün məlumatlarını tək JSON obyektində qaytarır.
        /// </summary>
        [HttpGet("GetCourseAndDetailById/{id}")]
        public async Task<IActionResult> GetCourseAndDetailById(int id, [FromQuery] string lang = "az")
        {
            var course = await _context.Courses
                .Include(c => c.CourseDetail)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return NotFound("Kurs tapılmadı");

            // 1) Kursun öz sahələrini dilə uyğun seçirik
            var title = lang.ToLower() switch
            {
                "az" => course.TitleAz,
                "en" => course.TitleEn,
                "ru" => course.TitleRu,
                _ => course.TitleAz
            };

            var description = lang.ToLower() switch
            {
                "az" => course.DescriptionAz,
                "en" => course.DescriptionEn,
                "ru" => course.DescriptionRu,
                _ => course.DescriptionAz
            };

            // 2) Detail varsa, onun üçün də dilə uyğun sahələri seçirik
            var detailTitle = course.CourseDetail == null ? null : (lang.ToLower() switch
            {
                "az" => course.CourseDetail.TitleAz,
                "en" => course.CourseDetail.TitleEn,
                "ru" => course.CourseDetail.TitleRu,
                _ => course.CourseDetail.TitleAz
            });

            var detailDescription = course.CourseDetail == null ? null : (lang.ToLower() switch
            {
                "az" => course.CourseDetail.DetailDescriptionAz,
                "en" => course.CourseDetail.DetailDescriptionEn,
                "ru" => course.CourseDetail.DetailDescriptionRu,
                _ => course.CourseDetail.DetailDescriptionAz
            });

            // 3) TargetAudience (və digərləri) üçün JSON parse edərək List<string> şəklində qaytarmaq
            var targetAudience = course.CourseDetail == null
                ? new List<string>()
                : GetListByLang(
                    course.CourseDetail.TargetAudienceAz,
                    course.CourseDetail.TargetAudienceEn,
                    course.CourseDetail.TargetAudienceRu,
                    lang);

            // Eyni məntiqlə curriculum, features və s. üçün də parse
            var curriculum = course.CourseDetail == null
                ? new List<string>()
                : GetListByLang(
                    course.CourseDetail.CurriculumAz,
                    course.CourseDetail.CurriculumEn,
                    course.CourseDetail.CurriculumRu,
                    lang);

            var features = course.CourseDetail == null
                ? new List<string>()
                : GetListByLang(
                    course.CourseDetail.FeaturesAz,
                    course.CourseDetail.FeaturesEn,
                    course.CourseDetail.FeaturesRu,
                    lang);

            var result = new
            {
                id = course.Id,
                title,
                description,
                programDuration = lang.ToLower() switch
                {
                    "az" => course.ProgramDurationAz,
                    "en" => course.ProgramDurationEn,
                    "ru" => course.ProgramDurationRu,
                    _ => course.ProgramDurationAz
                },
                iconUrl = $"{Request.Scheme}://{Request.Host}:6002/CourseIcons/{course.Icon}",
                participants = course.Participants,

                detailTitle,
                detailDescription,

                // Artıq 3 ayrı property yox, tək massiv kimi
                targetAudience,
                curriculum,
                features
            };

            return Ok(result);
        }

        // Helper metod (dəyişməz qalır)
        private List<string> GetListByLang(
            string listAz,
            string listEn,
            string listRu,
            string lang)
        {
            string selectedJson = lang.ToLower() switch
            {
                "az" => listAz,
                "en" => listEn,
                "ru" => listRu,
                _ => listAz
            };

            if (string.IsNullOrEmpty(selectedJson))
                return new List<string>();

            try
            {
                return JsonConvert.DeserializeObject<List<string>>(selectedJson)
                       ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }


        /// <summary>
        /// GET /api/Course/GetAllWithDetails?lang=az|en|ru
        /// Bütün kursları və onlara bağlı CourseDetail məlumatlarını tək JSON obyektlərində qaytarır.
        /// </summary>
        [HttpGet("GetAllWithDetails")]
        public async Task<IActionResult> GetAllWithDetails([FromQuery] string lang = "az")
        {
            var courses = await _context.Courses
                .Include(c => c.CourseDetail)
                .ToListAsync();

            var result = courses.Select(course => new
            {
                id = course.Id,
                title = lang.ToLower() switch
                {
                    "az" => course.TitleAz,
                    "en" => course.TitleEn,
                    "ru" => course.TitleRu,
                    _ => course.TitleAz
                },
                description = lang.ToLower() switch
                {
                    "az" => course.DescriptionAz,
                    "en" => course.DescriptionEn,
                    "ru" => course.DescriptionRu,
                    _ => course.DescriptionAz
                },
                programDuration = lang.ToLower() switch
                {
                    "az" => course.ProgramDurationAz,
                    "en" => course.ProgramDurationEn,
                    "ru" => course.ProgramDurationRu,
                    _ => course.ProgramDurationAz
                },
                iconUrl = $"{Request.Scheme}://{Request.Host}:6002/CourseIcons/{course.Icon}",
                participants = course.Participants,

                detailTitle = course.CourseDetail == null ? null : (lang.ToLower() switch
                {
                    "az" => course.CourseDetail.TitleAz,
                    "en" => course.CourseDetail.TitleEn,
                    "ru" => course.CourseDetail.TitleRu,
                    _ => course.CourseDetail.TitleAz
                }),
                detailDescription = course.CourseDetail == null ? null : (lang.ToLower() switch
                {
                    "az" => course.CourseDetail.DetailDescriptionAz,
                    "en" => course.CourseDetail.DetailDescriptionEn,
                    "ru" => course.CourseDetail.DetailDescriptionRu,
                    _ => course.CourseDetail.DetailDescriptionAz
                }),
                targetAudience = course.CourseDetail == null
                    ? new List<string>()
                    : GetListByLang(
                        course.CourseDetail.TargetAudienceAz,
                        course.CourseDetail.TargetAudienceEn,
                        course.CourseDetail.TargetAudienceRu,
                        lang),
                curriculum = course.CourseDetail == null
                    ? new List<string>()
                    : GetListByLang(
                        course.CourseDetail.CurriculumAz,
                        course.CourseDetail.CurriculumEn,
                        course.CourseDetail.CurriculumRu,
                        lang),
                features = course.CourseDetail == null
                    ? new List<string>()
                    : GetListByLang(
                        course.CourseDetail.FeaturesAz,
                        course.CourseDetail.FeaturesEn,
                        course.CourseDetail.FeaturesRu,
                        lang)
            });

            return Ok(result);
        }

        // ==================================================
        // Helper Method-lar
        // ==================================================

        private string GetTitleByLang(CourseDetail detail, string lang )
        {
            return lang.ToLower() switch
            {
                "az" => detail.TitleAz,
                "en" => detail.TitleEn,
                "ru" => detail.TitleRu,
                _ => detail.TitleAz // Default
            };
        }

        private string GetDetailDescriptionByLang(CourseDetail detail, string lang)
        {
            return lang.ToLower() switch
            {
                "az" => detail.DetailDescriptionAz,
                "en" => detail.DetailDescriptionEn,
                "ru" => detail.DetailDescriptionRu,
                _ => detail.DetailDescriptionAz
            };
        }

        /// <summary>
        /// Bu metod siyahıları (TargetAudience, Curriculum, Features və s.) 
        /// JSON formatından List<string> kimi parse edir.
        /// </summary>
        //private List<string> GetListByLang(
        //    string listAz,
        //    string listEn,
        //    string listRu,
        //    string lang)
        //{
        //    string selectedJson = lang.ToLower() switch
        //    {
        //        "az" => listAz,
        //        "en" => listEn,
        //        "ru" => listRu,
        //        _ => listAz
        //    };

        //    // Parse edərkən mümkün null-u da nəzərə alırıq:
        //    if (string.IsNullOrEmpty(selectedJson))
        //        return new List<string>();

        //    try
        //    {
        //        return JsonConvert.DeserializeObject<List<string>>(selectedJson)
        //               ?? new List<string>();
        //    }
        //    catch
        //    {
        //        // JSON parse oluna bilmirsə, boş qaytarırıq
        //        return new List<string>();
        //    }
        //}
    }
}
