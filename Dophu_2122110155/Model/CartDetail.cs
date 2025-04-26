namespace Dophu_2122110155.Model
{
    public class CartDetail
    {
        public int ID { get; set; }

        public int CartId { get; set; }       // FK đến Cart
        public int ProductId { get; set; }    // FK đến Product

        public int Quantity { get; set; }
        public double Price { get; set; }     // Giá tại thời điểm thêm vào giỏ

        // Navigation properties
        public Cart? Cart { get; set; }
        public Product? Product { get; set; }
    }
}
