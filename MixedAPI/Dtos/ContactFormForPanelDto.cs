using MixedAPI.Models;

namespace MixedAPI.Dtos
{
    public class ContactFormForPanelDto
    {
        public int ContactFormId { get; set; }  // Əlaqəli ContactForm ID-si
        public ContactFormStatus Status { get; set; } = ContactFormStatus.New;
        public DateTime? ResolvedTime { get; set; }
        public string Notes { get; set; }
        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
        public DateTime? ResponseTime { get; set; }
        public string AssignedDepartment { get; set; }
        public string ModeratorName { get; set; }
    }
}
