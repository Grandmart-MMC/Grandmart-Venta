using MixedAPI.Models;

namespace MixedAPI.Dtos
{
    public class ContactFormDto
    {
        public int CourseId { get; set; }  
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Avantage Avantage { get; set; }
        public string Message { get; set; }
    }
}
