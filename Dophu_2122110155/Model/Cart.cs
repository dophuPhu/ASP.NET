using System;
using System.Collections.Generic;

namespace Dophu_2122110155.Model
{
    public class Cart
    {
        public int ID { get; set; }
        public int UserId { get; set; } // FK đến User
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public User? User { get; set; }
        public List<CartDetail>? CartDetails { get; set; }
    }
}
