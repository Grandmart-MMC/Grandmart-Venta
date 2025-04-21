using MixedAPI.Dtos;
using MixedAPI.Models;

namespace MixedAPI.Services
{
    public interface IContactFormService
    {
        Task<ContactForm> SubmitFormAsync(ContactFormDto formform);
        Task<ContactFormForCompanyDto> SubmitFormForCompanyAsync(ContactFormForCompanyDto form);
    }
}
