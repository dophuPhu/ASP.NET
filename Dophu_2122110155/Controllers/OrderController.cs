using Dophu_2122110155.Data;
using Dophu_2122110155.DTO;
using Dophu_2122110155.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dophu_2122110155.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequestDto request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
                return NotFound("Người dùng không tồn tại");

            double total = 0;
            var orderDetails = new List<OrderDetail>();

            foreach (var item in request.OrderDetails)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                    return NotFound($"Sản phẩm ID {item.ProductId} không tồn tại");

                if (product.Stock < item.Quantity)
                    return BadRequest($"Sản phẩm ID {item.ProductId} không đủ hàng");

                // Trừ hàng trong kho
                product.Stock -= item.Quantity;

                var detail = new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = product.Price
                };
                total += product.Price * item.Quantity;
                orderDetails.Add(detail);
            }

            var order = new Order
            {
                UserId = request.UserId,
                Phone = request.Phone,               // ✅ mới thêm
                Address = request.Address,
                OrderDate = DateTime.Now,
                TotalPrice = total,
                Status = "Pending",
                OrderDetails = orderDetails
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đặt hàng thành công", orderId = order.ID });
        }
        // GET: api/Order
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
                .ToListAsync();

            return Ok(orders);
        }

        // GET: api/Order/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(o => o.ID == id);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        // PUT: api/Order/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrderRequestDto dto)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.ID == id);

            if (order == null)
                return NotFound();

            // Xóa các OrderDetails cũ
            _context.OrderDetails.RemoveRange(order.OrderDetails);

            // Cập nhật lại danh sách mới
            order.OrderDetails = dto.OrderDetails.Select(d => new OrderDetail
            {
                OrderId = id,
                ProductId = d.ProductId,
                Quantity = d.Quantity,
                Price = _context.Products.FirstOrDefault(p => p.ID == d.ProductId)?.Price ?? 0
            }).ToList();

            await _context.SaveChangesAsync();
            return Ok(order);
        }

        // DELETE: api/Order/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.ID == id);

            if (order == null)
                return NotFound();

            _context.OrderDetails.RemoveRange(order.OrderDetails);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }





}
