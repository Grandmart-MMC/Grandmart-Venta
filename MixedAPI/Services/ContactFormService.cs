using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using MixedAPI.Dtos;
using MixedAPI.Models;
using System.Net;
using System.Net.Mail;

namespace MixedAPI.Services
{
    public class ContactFormService : IContactFormService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ContactFormService> _logger;
        private readonly IValidator<ContactFormDto> _validator;

        public ContactFormService(
            ApplicationDbContext dbContext,
            IConfiguration configuration,
            ILogger<ContactFormService> logger,
            IValidator<ContactFormDto> validator)
        {
            _context = dbContext;
            _configuration = configuration;
            _logger = logger;
            _validator = validator;
        }

        public async Task<ContactForm> SubmitFormAsync(ContactFormDto form) 
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form), "Form məlumatı boş ola bilməz.");

            ValidationResult result = await _validator.ValidateAsync(form); 
            if (!result.IsValid)
            {
                throw new ValidationException("Formda xətalar mövcuddur:", result.Errors);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var course = await _context.Courses
                                           .Where(c => c.Id == form.CourseId)
                                           .FirstOrDefaultAsync();

                if (course == null)
                    throw new Exception("Seçilmiş kurs mövcud deyil.");

                await SendAdminNotificationAsync(form, course.TitleAz);
                if (!string.IsNullOrWhiteSpace(form.Email))
                {
                    await SendUserConfirmationAsync(form, course.TitleAz);
                }

                // 📌 Yeni `ContactForm` yarat
                var entity = new ContactForm
                {
                    Name = form.Name,
                    Email = form.Email,
                    PhoneNumber = form.PhoneNumber,
                    Message = form.Message,
                    CourseId = form.CourseId,
                    SendTime = DateTime.UtcNow
                };

                await _context.ContactForms.AddAsync(entity);
                await _context.SaveChangesAsync(); // 🚀 **Burada `await` vacibdir!**

                // 📌 **Ən böyük `Id` tapılır və `+1` artırılır (Əgər `IDENTITY` yoxdursa)**
                int newPanelId = (_context.ContactFormForPanels.Any())
                    ? _context.ContactFormForPanels.Max(p => p.Id) + 1
                    : 1;

                // 📌 **`ContactFormForPanel` avtomatik yaradılır**
                var panelEntry = new ContactFormForPanel
                {
                    Id = newPanelId,  // 📌 `Id` manual təyin edilir (Əgər `IDENTITY` yoxdursa)
                    ContactFormId = entity.Id,  // 📌 **Düzəliş edildi! `entity.Id` olmalıdır**
                    Status = ContactFormStatus.New, // 📌 Varsayılan: Yeni (0)
                    Priority = PriorityLevel.Medium, // 📌 Varsayılan: Orta
                    Notes = "", // 📌 Varsayılan: Boş
                    ResponseTime = null,
                    AssignedDepartment = "Support",
                    ModeratorName = "Moderator"
                };

                await _context.ContactFormForPanels.AddAsync(panelEntry);
                await _context.SaveChangesAsync(); // 📌 **İkinci `SaveChangesAsync()` ilə `panelEntry` DB-ə yazılır**

                await transaction.CommitAsync(); // ✅ **Bütün əməliyyatlar bitdikdən sonra transaction commit edilir**

                return entity;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Contact form submit error");
                throw;
            }
        }


        private async Task SendAdminNotificationAsync(ContactFormDto form, string courseName)
        {
            try
            {
                var smtpHost = _configuration["Smtp:Host"];
                var smtpPort = int.Parse(_configuration["Smtp:Port"]);
                var smtpUser = _configuration["Smtp:UserName"];
                var smtpPass = _configuration["Smtp:Password"];
                var smtpSsl = bool.Parse(_configuration["Smtp:EnableSsl"]);

                using var smtp = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = smtpSsl
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(smtpUser, "Venta Academy"),
                    Subject = $"Yeni müraciət: {courseName}",
                    IsBodyHtml = true,
                    Body = GenerateAdminEmailBody(form, courseName)
                };

                mail.To.Add("babek.agamuradli@grandmart.az");

                await smtp.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Admin bildirişi göndərilmədi");
                throw;
            }
        }

        private async Task SendUserConfirmationAsync(ContactFormDto form, string courseName)
        {
            try
            {
                var smtpHost = _configuration["Smtp:Host"];
                var smtpPort = int.Parse(_configuration["Smtp:Port"]);
                var smtpUser = _configuration["Smtp:UserName"];
                var smtpPass = _configuration["Smtp:Password"];
                var smtpSsl = bool.Parse(_configuration["Smtp:EnableSsl"]);

                using var smtp = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = smtpSsl
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(smtpUser, "Venta Academy"),
                    Subject = "Müraciətiniz Uğurla Qəbul Edildi!",
                    IsBodyHtml = true,
                    Body = GenerateUserEmailBody(form, courseName)
                };

                mail.To.Add(form.Email);

                await smtp.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GenerateAdminEmailBody(ContactFormDto form, string courseName)
        {
            return $@"
<div style='font-family: Arial, sans-serif; color: #333; margin: 0 auto; max-width: 600px; border: 1px solid #ddd; border-radius: 8px; overflow: hidden; background-color: #f9f9f9; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);'>

    <!-- Header -->
    <div style='background-color: #D53364; color: white; padding: 30px; text-align: center;'>
        <h1 style='margin: 0; font-size: 28px; letter-spacing: 1px;'>Venta Academy</h1>
        <p style='margin: 5px 0 0; font-size: 18px;'>Yeni Təlim Müraciəti</p>
    </div>

    <!-- Body Content -->
    <div style='padding: 20px; background-color: #ffffff;'>

        <!-- Details Section -->
        <table style='width: 100%; border-collapse: collapse; margin-bottom: 20px;'>
            <tr style='background-color: #f1f1f1;'>
                <td style='padding: 10px; font-weight: bold;'>Ad:</td>
                <td style='padding: 10px;'>{form.Name}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-weight: bold;'>Kursun Adı:</td>
                <td style='padding: 10px;'>{courseName}</td>
            </tr>
            <tr style='background-color: #f1f1f1;'>
                <td style='padding: 10px; font-weight: bold;'>Email:</td>
                <td style='padding: 10px;'><a href='mailto:{form.Email}' style='color: #D53364; text-decoration: none;'>{form.Email}</a></td>
            </tr>
            <tr>
                <td style='padding: 10px; font-weight: bold;'>Telefon:</td>
                <td style='padding: 10px;'>{form.PhoneNumber}</td>
            </tr>
            <tr style='background-color: #f1f1f1;'>
                <td style='padding: 10px; font-weight: bold;'>Üstünlük:</td>
                <td style='padding: 10px;'>{(form.Avantage == Avantage.Phone ? "Telefonla Əlaqə" : "Email ilə Əlaqə")}</td>
            </tr>
            <tr>
                <td style='padding: 10px; font-weight: bold;'>Göndərilmə Tarixi:</td>
                <td style='padding: 10px;'>{DateTime.Now:dd.MM.yyyy HH:mm}</td>
            </tr>
        </table>

        <!-- Message Section -->
        <div style='border-top: 3px solid #D53364; padding-top: 20px;'>
            <h3 style='margin: 0 0 10px; font-size: 20px; color: #D53364;'>Mesaj:</h3>
            <p style='font-size: 16px; line-height: 1.6; color: #333;'>{form.Message}</p>
        </div>
    </div>

    <!-- Footer -->
    <div style='background-color: #f1f1f1; text-align: center; padding: 15px;'>
        <p style='font-size: 14px; margin: 0; color: #555;'>© 2025 Venta Academy. Bütün hüquqlar qorunur.</p>
        <p style='font-size: 12px; margin: 5px 0 0; color: #777;'>Bu email avtomatik göndərilib. Xahiş edirik cavablandırmayın.</p>
    </div>
</div>";
        }

        private string GenerateUserEmailBody(ContactFormDto form, string courseName)
        {
            return $@"
    <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; border: 1px solid #ddd; border-radius: 8px; overflow: hidden; background-color: #f9f9f9;'>
        <div style='background-color: #0078D7; color: #fff; padding: 20px; text-align: center;'>
            <h1 style='margin: 0;'>Venta Academy</h1>
            <p style='margin: 5px 0; font-size: 18px;'>Müraciətiniz Uğurla Qəbul Edildi!</p>
        </div>
        <div style='padding: 20px;'>
            <p style='font-size: 16px;'>Hörmətli <strong>{form.Name}</strong>,</p>
            <p style='font-size: 16px;'>{courseName} təliminə müraciət etdiyiniz üçün təşəkkür edirik. Müraciətiniz uğurla qəbul edilmişdir.</p>
            <p style='font-size: 16px;'>Əlavə məlumat üçün bizimlə <a href='mailto:info@ventaacademy.com' style='color: #0078D7; text-decoration: none;'>info@ventaacademy.com</a> ünvanı vasitəsilə əlaqə saxlaya bilərsiniz.</p>
        </div>
        <div style='background-color: #f1f1f1; text-align: center; padding: 15px;'>
            <p style='margin: 0; font-size: 14px; color: #555;'>© 2025 Venta Academy. Bütün hüquqlar qorunur.</p>
            <p style='font-size: 12px; margin: 5px 0 0; color: #777;'>Bu email avtomatik göndərilib. Xahiş edirik cavablandırmayın.</p>
        </div>
    </div>";
        }
    }
}
