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
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UserController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Đăng nhập
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == login.Email && u.Password == login.Password);
            if (user == null)
                return Unauthorized("Email hoặc mật khẩu không đúng");

            var token = GenerateToken(user);

            // Giả sử bạn có trường Role trong User, sẽ trả về role và token
            var response = new
            {
                token,
                role = user.Role,
                Id = user.ID // Trả về role của người dùng
            };

            return Ok(response);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
        {
            var user = _context.Users.FirstOrDefault(u => u.ID == id);
            if (user == null)
                return NotFound("Không tìm thấy người dùng");

            user.Name = updatedUser.Name ?? user.Name;  // Nếu tên mới là null, giữ nguyên tên cũ
            user.Email = updatedUser.Email ?? user.Email;  // Nếu email mới là null, giữ nguyên email cũ
            user.Password = updatedUser.Password ?? user.Password;  // Tương tự với password
            user.Role = updatedUser.Role ?? user.Role;
            _context.Users.Update(user);
            _context.SaveChanges();

            return Ok(new { message = "Thông tin người dùng đã được cập nhật" });
        }

        // DELETE - Xóa người dùng
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.ID == id);
            if (user == null)
                return NotFound("Không tìm thấy người dùng");

            _context.Users.Remove(user);
            _context.SaveChanges();

            return Ok(new { message = "Người dùng đã được xóa" });
        }


        // Lấy thông tin người dùng từ JWT token
        // Lấy thông tin người dùng từ ID
        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.ID == id);

            if (user == null)
                return NotFound("Không tìm thấy người dùng");

            return Ok(new
            {
                user.ID,
                user.Name,
                user.Email
            });
        }

        // Lấy tất cả người dùng mà không cần JWT token
        [HttpGet("all")]
        public IActionResult GetAllUsers()
        {
            var users = _context.Users.ToList();  // Lấy tất cả người dùng từ cơ sở dữ liệu
            if (users == null || !users.Any())
                return NotFound("Không có người dùng nào");

            return Ok(users);
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

        // Hàm giải mã và kiểm tra token
        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

                var parameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                var principal = tokenHandler.ValidateToken(token, parameters, out var validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }

    // Lớp yêu cầu đăng nhập
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
