using Dophu_2122110155.Data;
using Dophu_2122110155.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Dophu_2122110155.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ĐĂNG KÝ
        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);
            if (existingUser != null)
                return BadRequest("Email đã tồn tại!");

            // 👇 Nếu không có role thì set mặc định là "User"
            if (string.IsNullOrEmpty(user.Role))
            {
                user.Role = "User";
            }

            _context.Users.Add(user);
            _context.SaveChanges();

            var token = GenerateToken(user);
            return Ok(new { message = "Đăng ký thành công!", token });
        }


        // Hàm tạo JWT token
        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("UserID", user.ID.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
