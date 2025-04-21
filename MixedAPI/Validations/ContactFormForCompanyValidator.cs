using FluentValidation;
using MixedAPI.Dtos;

namespace MixedAPI.Validations
{
    public class ContactFormForCompanyValidator : AbstractValidator<ContactFormForCompanyDto>
    {
        public ContactFormForCompanyValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ad sahəsi boş ola bilməz.")
                .MaximumLength(50).WithMessage("Ad maksimum 50 simvol ola bilər.");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email boş ola bilməz.")
                .EmailAddress().WithMessage("Düzgün email formatı deyil.");
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Telefon nömrəsi boş ola bilməz.")
                .Matches(@"^[0-9+ ]+$").WithMessage("Telefon nömrəsi yalnız rəqəmlər və + işarəsi ola bilər.");
        }
    }
}
