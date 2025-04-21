using FluentValidation;
using MixedAPI.Dtos;
using MixedAPI.Models;

namespace MixedAPI.Validations
{
    public class ContactFormValidator : AbstractValidator<ContactFormDto>
    {
        public ContactFormValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("Kursun Id-si boş ola bilməz.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ad sahəsi boş ola bilməz.")
                .MaximumLength(50).WithMessage("Ad maksimum 50 simvol ola bilər.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email boş ola bilməz.")
                .EmailAddress().WithMessage("Düzgün email formatı deyil.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Telefon nömrəsi boş ola bilməz.")
                .Matches(@"^[0-9+ ]+$").WithMessage("Telefon nömrəsi yalnız rəqəmlər və + işarəsi ola bilər.");

            // Əgər isdəsəniz, Message sahəsinə də qaydalar yaza bilərsiniz
            // RuleFor(x => x.Message)
            //     .MaximumLength(500).WithMessage("Mesaj maksimum 500 simvol ola bilər.");
        }
    }
}