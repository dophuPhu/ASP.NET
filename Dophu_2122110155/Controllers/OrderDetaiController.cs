using Dophu_2122110155.Data;
using Dophu_2122110155.DTO;
using Dophu_2122110155.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dophu_2122110155.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderDetailController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/OrderDetail
        [HttpGet("orderdetails")]
        public IActionResult GetOrderDetails()
        {
            var orderDetails = _context.OrderDetails
                .Select(od => new OrderDetailRequestDto
                {
                    Id = od.ID,
                    OrderId = od.OrderId,
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    Price = od.Price
                })
                .ToList();

            return Ok(orderDetails);
        }

        // GET: api/OrderDetail/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDetail>> GetOrderDetail(int id)
        {
            try
            {
                var orderDetail = await _context.OrderDetails
                    .Include(d => d.Product)
                    .Include(d => d.Order)
                    .FirstOrDefaultAsync(d => d.ID == id);

                if (orderDetail == null)
                    return NotFound();

                return Ok(orderDetail);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        // POST: api/OrderDetail
        [HttpPost]
        public async Task<ActionResult<OrderDetail>> PostOrderDetail(OrderDetail detail)
        {
            _context.OrderDetails.Add(detail);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrderDetail), new { id = detail.ID }, detail);
        }

        // PUT: api/OrderDetail/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderDetail(int id, OrderDetail detail)
        {
            if (id != detail.ID)
                return BadRequest("ID không khớp");

            _context.Entry(detail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Cập nhật thành công");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderDetailExists(id))
                    return NotFound();
                else
                    throw;
            }
        }

        // DELETE: api/OrderDetail/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderDetail(int id)
        {
            var detail = await _context.OrderDetails.FindAsync(id);
            if (detail == null)
                return NotFound();

            _context.OrderDetails.Remove(detail);
            await _context.SaveChangesAsync();

            return Ok("Xóa thành công");
        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.ID == id);
        }
    }
}
