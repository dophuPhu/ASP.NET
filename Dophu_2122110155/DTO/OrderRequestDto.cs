namespace Dophu_2122110155.DTO
{
    public class OrderDetailRequestDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }

    public class OrderRequestDto
    {
        public int UserId { get; set; }
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
        public List<OrderDetailRequestDto> OrderDetails { get; set; } = new();
    }
}
