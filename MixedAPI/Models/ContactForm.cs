using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MixedAPI.Models
{
    public class ContactForm
    {
        [Key]
        public int Id { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public Avantage Avantage { get; set; }
        public string? Message { get; set; }
        public DateTime SendTime { get; set; }
        public ContactFormForPanel? ContactFormForPanel { get; set; }
    }
    public enum Avantage
    {
        Phone,
        Email
    }
    public class ContactFormForPanel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  
        public ContactFormStatus Status { get; set; } = ContactFormStatus.New;
        public DateTime? ResolvedTime { get; set; }
        public string? Notes { get; set; }
        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
        public DateTime? ResponseTime { get; set; }
        public string? AssignedDepartment { get; set; }

        [MaxLength(100)]
        public string? ModeratorName { get; set; }
        [JsonIgnore]
        public ContactForm ContactForm { get; set; }
        [ForeignKey("ContactForm")]
        public int ContactFormId { get; set; }
    }

    /// <summary>
    /// Status Enum - Contact Formun vəziyyətini izləmək üçün
    /// </summary>
    public enum ContactFormStatus
    {
        New = 1,          
        InProgress = 2,   
        Resolved = 3,   
        Unresolved = 4
    }

    /// <summary>
    /// Priority Enum - Əhəmiyyət səviyyəsi
    /// </summary>
    public enum PriorityLevel
    {
        Low = 1,   
        Medium = 2,
        High = 3   
    }
    public class PaginatedResponse<T>
    {
        public List<T> Data { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

}
