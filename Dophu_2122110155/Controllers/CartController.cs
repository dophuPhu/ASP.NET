using Dophu_2122110155.Data;
using Dophu_2122110155.DTO;
using Dophu_2122110155.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dophu_2122110155.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Thêm sản phẩm vào giỏ hàng
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(int userId, int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound("Sản phẩm không tồn tại");

            // Kiểm tra nếu số lượng sản phẩm trong kho đủ để thêm vào giỏ hàng
            if (product.Stock < quantity)
            {
                return BadRequest("Sản phẩm không đủ số lượng trong kho");
            }

            // Tìm hoặc tạo giỏ hàng cho user
            var cart = await _context.Carts
                .Include(c => c.CartDetails)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            // Kiểm tra sản phẩm đã có trong giỏ chưa
            var cartDetail = cart.CartDetails?.FirstOrDefault(cd => cd.ProductId == productId);

            if (cartDetail != null)
            {
                // Nếu sản phẩm đã có trong giỏ, cập nhật số lượng
                cartDetail.Quantity += quantity;
            }
            else
            {
                // Nếu sản phẩm chưa có trong giỏ, thêm mới
                cartDetail = new CartDetail
                {
                    CartId = cart.ID,
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.Price
                };
                _context.CartDetails.Add(cartDetail);
            }

            // Giảm số lượng sản phẩm trong bảng Product
            product.Stock -= quantity;

            // Cập nhật sản phẩm trong bảng Product
            _context.Products.Update(product);

            await _context.SaveChangesAsync();
            return Ok("Đã thêm vào giỏ hàng và cập nhật số lượng sản phẩm");
        }


        // ✅ Lấy giỏ hàng theo userId
        [HttpGet("get/{userId}")]
        public async Task<IActionResult> GetCart(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return NotFound("Không có giỏ hàng");

            return Ok(cart);
        }
        // ✅ Lấy tất cả giỏ hàng đầy đủ (bao gồm chi tiết sản phẩm và tổng giá trị giỏ hàng)
        [HttpGet("get-full")]
        public async Task<IActionResult> GetAllCarts()
        {
            // Lấy tất cả giỏ hàng và chi tiết giỏ hàng kèm theo sản phẩm
            var carts = await _context.Carts
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .ToListAsync();

            if (carts == null || !carts.Any())
                return NotFound("Không có giỏ hàng nào");

            // Tạo danh sách giỏ hàng với thông tin chi tiết
            var allCartInfo = carts.Select(cart => new
            {
                cart.ID,
                cart.UserId,
                CartDetails = cart.CartDetails.Select(cd => new
                {
                    cd.ProductId,
                    ProductName = cd.Product.Name,
                    ProductImage = cd.Product.Image, // Giả sử bạn có trường này trong bảng Products
                    cd.Quantity,
                    cd.Price,
                    SubTotal = cd.Quantity * cd.Price
                }).ToList(),
                TotalPrice = cart.CartDetails.Sum(cd => cd.Quantity * cd.Price)
            }).ToList();

            return Ok(allCartInfo);
        }
        // ❌ Xóa toàn bộ giỏ hàng theo userId
        [HttpDelete("clear/{userId}")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartDetails)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return NotFound("Không tìm thấy giỏ hàng");

            _context.CartDetails.RemoveRange(cart.CartDetails);
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return Ok("Đã xóa toàn bộ giỏ hàng");
        }
        // ❌ Xóa một sản phẩm trong giỏ hàng
        [HttpDelete("remove-item")]
        public async Task<IActionResult> RemoveItemFromCart(int userId, int productId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartDetails)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return NotFound("Không tìm thấy giỏ hàng");

            var cartItem = cart.CartDetails.FirstOrDefault(cd => cd.ProductId == productId);

            if (cartItem == null)
                return NotFound("Sản phẩm không tồn tại trong giỏ hàng");

            _context.CartDetails.Remove(cartItem);

            // Nếu sau khi xóa mà giỏ hàng không còn sản phẩm nào thì xóa luôn giỏ hàng
            if (cart.CartDetails.Count == 1) // vì đang chuẩn bị xóa cái cuối
            {
                _context.Carts.Remove(cart);
            }

            await _context.SaveChangesAsync();
            return Ok("Đã xóa sản phẩm khỏi giỏ hàng");
        }



        // ✅ Thanh toán (chuyển từ Cart ➜ Order)
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDto checkout)
        {
            var cart = await _context.Carts
                .Include(c => c.CartDetails)
                .FirstOrDefaultAsync(c => c.UserId == checkout.UserId);

            if (cart == null || cart.CartDetails == null || !cart.CartDetails.Any())
                return BadRequest("Giỏ hàng trống");

            var order = new Order
            {
                UserId = checkout.UserId,
                Phone = checkout.Phone,
                Address = checkout.Address,
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalPrice = cart.CartDetails.Sum(cd => cd.Quantity * cd.Price),
                OrderDetails = cart.CartDetails.Select(cd => new OrderDetail
                {
                    ProductId = cd.ProductId,
                    Quantity = cd.Quantity,
                    Price = cd.Price
                }).ToList()
            };

            _context.Orders.Add(order);

            // Xóa giỏ hàng
            _context.CartDetails.RemoveRange(cart.CartDetails);
            _context.Carts.Remove(cart);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã thanh toán thành công", orderId = order.ID });
        }

    }
}
