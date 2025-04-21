using MixedAPI.Models;

namespace MixedAPI.Dtos
{
    public class ContactFormForPanelUpdateDto
    {
        public ContactFormStatus Status { get; set; }
        public PriorityLevel Priority { get; set; }
        public string? Notes { get; set; }
    }
}
