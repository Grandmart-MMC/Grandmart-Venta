//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using MixedAPI.Models;
//using Newtonsoft.Json;

//namespace MixedAPI.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class CourseAndDetailsController : ControllerBase
//    {
//        private readonly ApplicationDbContext _context;

//        public CourseAndDetailsController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // GET /api/CourseWithDetails?lang=az|en|ru
//        [HttpGet("WithDetails")]
//        public async Task<IActionResult> GetAllWithDetails([FromQuery] string lang = "az")
//        {
//            // Bütün Course-ları gətiririk
//            var query = _context.Courses
//                .Select(course => new
//                {
//                    course.Id,

//                    Title = (lang == "az") ? course.TitleAz
//                           : (lang == "en") ? course.TitleEn
//                           : course.TitleRu,

//                    Description = (lang == "az") ? course.DescriptionAz
//                                 : (lang == "en") ? course.DescriptionEn
//                                 : course.DescriptionRu,

//                    ProgramDuration = (lang == "az") ? course.ProgramDurationAz
//                                     : (lang == "en") ? course.ProgramDurationEn
//                                     : course.ProgramDurationRu,

//                    IconUrl = $"{Request.Scheme}://{Request.Host}/CourseIcons/{course.Icon}",
//                    course.Participants,

//                    // CourseDetail-ləri alt obyekt kimi gətiririk
//                    Details = _context.CourseDetails
//                        .Where(d => d.CourseId == course.Id)
//                        .Select(d => new
//                        {
//                            d.Id,
//                            Title = (lang == "az") ? d.TitleAz
//                                   : (lang == "en") ? d.TitleEn
//                                   : d.TitleRu,

//                            DetailDescription = (lang == "az") ? d.DetailDescriptionAz
//                                              : (lang == "en") ? d.DetailDescriptionEn
//                                              : d.DetailDescriptionRu,

//                            TargetAudience = GetListByLang(d.TargetAudienceAz,
//                                                           d.TargetAudienceEn,
//                                                           d.TargetAudienceRu,
//                                                           lang),
//                            Curriculum = GetListByLang(d.CurriculumAz,
//                                                       d.CurriculumEn,
//                                                       d.CurriculumRu,
//                                                       lang),
//                            Features = GetListByLang(d.FeaturesAz,
//                                                     d.FeaturesEn,
//                                                     d.FeaturesRu,
//                                                     lang)
//                        })
//                        .ToList()
//                });

//            var result = await query.ToListAsync();

//            return Ok(result);
//        }

//        // GET /api/CourseWithDetails/3?lang=az|en|ru
//        [HttpGet("WithDetails/{id}")]
//        public async Task<IActionResult> GetByIdWithDetails(int id, [FromQuery] string lang = "az")
//        {
//            var course = await _context.Courses
//                .Where(c => c.Id == id)
//                .Select(c => new
//                {
//                    c.Id,

//                    Title = (lang == "az") ? c.TitleAz
//                           : (lang == "en") ? c.TitleEn
//                           : c.TitleRu,

//                    Description = (lang == "az") ? c.DescriptionAz
//                                 : (lang == "en") ? c.DescriptionEn
//                                 : c.DescriptionRu,

//                    ProgramDuration = (lang == "az") ? c.ProgramDurationAz
//                                     : (lang == "en") ? c.ProgramDurationEn
//                                     : c.ProgramDurationRu,

//                    IconUrl = $"{Request.Scheme}://{Request.Host}/CourseIcons/{c.Icon}",
//                    c.Participants,

//                    Details = _context.CourseDetails
//                        .Where(d => d.CourseId == c.Id)
//                        .Select(d => new
//                        {
//                            d.Id,
//                            Title = (lang == "az") ? d.TitleAz
//                                   : (lang == "en") ? d.TitleEn
//                                   : d.TitleRu,

//                            DetailDescription = (lang == "az") ? d.DetailDescriptionAz
//                                              : (lang == "en") ? d.DetailDescriptionEn
//                                              : d.DetailDescriptionRu,

//                            TargetAudience = GetListByLang(d.TargetAudienceAz,
//                                                           d.TargetAudienceEn,
//                                                           d.TargetAudienceRu,
//                                                           lang),
//                            Curriculum = GetListByLang(d.CurriculumAz,
//                                                       d.CurriculumEn,
//                                                       d.CurriculumRu,
//                                                       lang),
//                            Features = GetListByLang(d.FeaturesAz,
//                                                     d.FeaturesEn,
//                                                     d.FeaturesRu,
//                                                     lang)
//                        })
//                        .ToList()
//                })
//                .FirstOrDefaultAsync();

//            if (course == null)
//                return NotFound("Kurs tapılmadı");

//            return Ok(course);
//        }

//        // JSON parse helper
//        private List<string> GetListByLang(
//            string listAz,
//            string listEn,
//            string listRu,
//            string lang)
//        {
//            string selectedJson = lang.ToLower() switch
//            {
//                "az" => listAz,
//                "en" => listEn,
//                "ru" => listRu,
//                _ => listAz
//            };

//            if (string.IsNullOrEmpty(selectedJson))
//                return new List<string>();

//            try
//            {
//                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(selectedJson)
//                       ?? new List<string>();
//            }
//            catch
//            {
//                return new List<string>();
//            }
//        }
//    }
//}
