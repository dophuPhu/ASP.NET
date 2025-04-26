namespace Dophu_2122110155.Model
{
    public class ProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }
        public int Stock { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
