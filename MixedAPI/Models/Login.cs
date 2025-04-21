using Microsoft.EntityFrameworkCore;

namespace MixedAPI.Models
{
    [Keyless]
    public class Login
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

}
