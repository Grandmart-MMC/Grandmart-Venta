using System.ComponentModel.DataAnnotations;

namespace MixedAPI.Models;

public class ContactFormForCompany
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Message { get; set; }
    public DateTime SendTime { get; set; }
}
