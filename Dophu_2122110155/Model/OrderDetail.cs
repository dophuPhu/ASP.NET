namespace Dophu_2122110155.Model
{
    public class OrderDetail
    {
        public int ID { get; set; }

        public int OrderId { get; set; }        // FK đến Order
        public int ProductId { get; set; }      // FK đến Product

        public int Quantity { get; set; }
        public double Price { get; set; }       // Giá tại thời điểm đặt hàng

        // Navigation properties
        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}
