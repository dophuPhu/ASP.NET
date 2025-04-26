namespace Dophu_2122110155.Model
{
    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        // Phân loại người dùng: "Admin" hoặc "User"
        public string Role { get; set; } = "User"; // mặc định là user thường
    }
}
