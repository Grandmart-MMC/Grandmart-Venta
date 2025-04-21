using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MixedAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] Login model)
    {
        if (model.Username == "support@venta.az" && model.Password == "Supp@rt_1") 
        {
            var token = GenerateJwtToken(model.Username);
            return Ok(new { token });
        }
        else if (model.Username == "admin@venta.az" && model.Password == "@dmin_1")
        {
            var token = GenerateJwtToken(model.Username);
            return Ok(new { token });
        }
        else
        {
            return NotFound("Daxil olma uğursuz oldu!");
        }
    }

    private string GenerateJwtToken(string username)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "Moderator")
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
