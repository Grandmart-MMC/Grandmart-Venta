using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MixedAPI.Dtos;
using MixedAPI.Models;
using MixedAPI.Services;

namespace MixedAPI.Controllers
{
    [ApiController]
    [Route("api/contact")]
    public class ContactFormController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IContactFormService _contactFormService;

        public ContactFormController(ApplicationDbContext context, IContactFormService contactFormService)
        {
            _context = context;
            _contactFormService = contactFormService;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromBody] ContactFormDto form)
        {
            if (form == null)
                return BadRequest("Form məlumatı boşdur!");
            try
            {
                var createdForm = await _contactFormService.SubmitFormAsync(form);

                if (createdForm == null)
                    return StatusCode(500, new { message = "ContactForm yaradıla bilmədi." });

                int newPanelId = (_context.ContactFormForPanels.Any())
                    ? _context.ContactFormForPanels.Max(p => p.Id) + 1
                    : 1;

                var panelEntry = new ContactFormForPanel
                {
                    Id = newPanelId,
                    ContactFormId = createdForm.Id,
                    Status = ContactFormStatus.New,
                    Priority = PriorityLevel.Medium,
                    Notes = "",
                    ResponseTime = null,
                    AssignedDepartment = "Support",
                    ModeratorName = "Moderator"
                };

                await _context.ContactFormForPanels.AddAsync(panelEntry);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Success", contactFormId = createdForm.Id, panelId = panelEntry.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Xəta baş verdi", error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost("submit-company")]
        public async Task<IActionResult> SubmitForCompany([FromBody] ContactFormForCompanyDto form)
        {
            if (form == null)
                return BadRequest("Form məlumatı boşdur!");
            try
            {
                var createdForm = await _contactFormService.SubmitFormForCompanyAsync(form);

                if (createdForm == null)
                    return StatusCode(500, new { message = "ContactForm yaradıla bilmədi." });

                return Ok(new { message = "Success" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Xəta baş verdi", error = ex.InnerException?.Message ?? ex.Message });
            }
        }


        [HttpGet("panel")]
        [Authorize]
        public async Task<IActionResult> GetAllPanels(
       [FromQuery] ContactFormStatus? status,
       [FromQuery] PriorityLevel? priority,
       [FromQuery] DateTime? startDate,
       [FromQuery] DateTime? endDate,
       [FromQuery] string? search, 
       [FromQuery] int page = 1,
       [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest("Page və PageSize 1-dən kiçik ola bilməz.");

            var query = _context.ContactFormForPanels
                .Include(p => p.ContactForm)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);

            if (priority.HasValue)
                query = query.Where(p => p.Priority == priority.Value);

            if (startDate.HasValue)
                query = query.Where(p => p.ContactForm.SendTime >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(p => p.ContactForm.SendTime <= endDate.Value);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p =>
                    p.ContactForm.Name.Contains(search) ||
                    p.ContactForm.Email.Contains(search) ||
                    p.ContactForm.PhoneNumber.Contains(search) ||
                    p.ContactForm.Message.Contains(search)
                );
            }

            // 📌 Total count for pagination
            int totalCount = await query.CountAsync();

            var panels = await query
                .OrderByDescending(p => p.ContactForm.SendTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    p.Id,
                    ContactFormId = p.ContactForm.Id,
                    ContactFormName = p.ContactForm.Name,
                    ContactFormEmail = p.ContactForm.Email,
                    ContactFormPhone = p.ContactForm.PhoneNumber,
                    ContactFormMessage = p.ContactForm.Message,
                    ContactFormAvantage = p.ContactForm.Avantage,
                    ContactFormSendTime = p.ContactForm.SendTime,
                    p.Status,
                    p.Priority,
                    p.Notes
                })
                .ToListAsync();

            return Ok(new
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Data = panels
            });
        }


        [Authorize]
        [HttpGet("panel/{id}")]
        public async Task<IActionResult> GetPanelById(int id)
        {
            var panel = await _context.ContactFormForPanels
                .Include(p => p.ContactForm)
                .Select(p => new
                {
                    p.Id,
                    ContactFormId = p.ContactForm.Id,
                    ContactFormName = p.ContactForm.Name,
                    ContactFormEmail = p.ContactForm.Email,
                    ContactFormPhone = p.ContactForm.PhoneNumber,
                    ContactFormMessage = p.ContactForm.Message,
                    ContactFormAvantage = p.ContactForm.Avantage,
                    ContactFormSendTime = p.ContactForm.SendTime,
                    p.Status,
                    p.Priority,
                    p.Notes,
                })
                .FirstOrDefaultAsync(p => p.Id == id);

            if (panel == null) return NotFound("Panel tapılmadı.");
            return Ok(panel);
        }

        [Authorize]
        [HttpPut("panel/{id}")]
        public async Task<IActionResult> UpdatePanel(int id, [FromBody] ContactFormForPanelUpdateDto dto)
        {
            var panelEntry = await _context.ContactFormForPanels.FindAsync(id);
            if (panelEntry == null)
                return NotFound("Panel tapılmadı.");

            bool isUpdated = false;

            // 📌 Əgər `Status` göndərilibsə, onu yenilə və `ResponseTime`-ı qeyd et
            if (dto.Status != 0 && dto.Status != panelEntry.Status)
            {
                panelEntry.Status = dto.Status;
                panelEntry.ResponseTime = DateTime.UtcNow; // 📌 Response zamanı qeyd olunur
                isUpdated = true;
            }

            // 📌 Əgər `Status = Resolved` olarsa, `ResolvedTime`-ı qeyd et
            if (dto.Status == ContactFormStatus.Resolved)
            {
                panelEntry.ResolvedTime = DateTime.UtcNow;
            }

            // 📌 Əgər `Priority` göndərilibsə, onu yenilə
            if (dto.Priority != 0 && dto.Priority != panelEntry.Priority)
            {
                panelEntry.Priority = dto.Priority;
                isUpdated = true;
            }

            // 📌 Əgər `Notes` göndərilibsə və boş deyilsə, onu yenilə
            if (!string.IsNullOrEmpty(dto.Notes) && dto.Notes != panelEntry.Notes)
            {
                panelEntry.Notes = dto.Notes;
                isUpdated = true;
            }

            // 📌 Əgər heç bir dəyişiklik edilməyibsə, `BadRequest` qaytar
            if (!isUpdated)
                return BadRequest("Heç bir dəyişiklik edilməyib.");

            await _context.SaveChangesAsync();
            return Ok(new { message = "Panel uğurla yeniləndi"});
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var form = await _context.ContactForms.Include(c => c.ContactFormForPanel).FirstOrDefaultAsync(f => f.Id == id);
            if (form == null) return NotFound("Müraciət tapılmadı.");

            if (form.ContactFormForPanel != null)
                _context.ContactFormForPanels.Remove(form.ContactFormForPanel);

            _context.ContactForms.Remove(form);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
