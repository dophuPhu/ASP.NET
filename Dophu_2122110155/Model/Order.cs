using System;
using System.Collections.Generic;

namespace Dophu_2122110155.Model
{
    public class Order
    {
        public int ID { get; set; }

        public int UserId { get; set; }               // FK đến User
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public double TotalPrice { get; set; }
        public string Status { get; set; } = "Pending"; // Có thể là Pending, Shipped, etc.
        public string Phone { get; set; } = string.Empty;    // ✅ Thêm số điện thoại
        public string Address { get; set; } = string.Empty;
        // Navigation properties
        public User? User { get; set; }
        public List<OrderDetail>? OrderDetails { get; set; }
    }
}
