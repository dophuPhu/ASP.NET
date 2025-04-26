using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dophu_2122110155.Data;
using Dophu_2122110155.Model;

namespace Dophu_2122110155.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartDetailController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartDetailController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/CartDetail?userId=1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartDetail>>> GetCartDetails(int userId)
        {
            var cart = await _context.Carts
                                     .Include(c => c.CartDetails!)
                                     .ThenInclude(cd => cd.Product)
                                     .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return NotFound("Cart not found for user");

            return Ok(cart.CartDetails);
        }

        // POST: api/CartDetail
        [HttpPost]
        public async Task<ActionResult<CartDetail>> AddToCart(CartDetail cartDetail)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.ID == cartDetail.CartId);

            if (cart == null)
                return BadRequest("Cart not found");

            // Nếu sản phẩm đã có trong cart thì tăng số lượng
            var existing = await _context.CartDetails
                                         .FirstOrDefaultAsync(cd => cd.CartId == cartDetail.CartId && cd.ProductId == cartDetail.ProductId);

            if (existing != null)
            {
                existing.Quantity += cartDetail.Quantity;
                _context.CartDetails.Update(existing);
            }
            else
            {
                _context.CartDetails.Add(cartDetail);
            }

            await _context.SaveChangesAsync();
            return Ok(cartDetail);
        }

        // PUT: api/CartDetail/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCartDetail(int id, CartDetail updated)
        {
            if (id != updated.ID)
                return BadRequest();

            var existing = await _context.CartDetails.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.Quantity = updated.Quantity;
            _context.CartDetails.Update(existing);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/CartDetail/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartDetail(int id)
        {
            var cartDetail = await _context.CartDetails.FindAsync(id);
            if (cartDetail == null)
                return NotFound();

            _context.CartDetails.Remove(cartDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
