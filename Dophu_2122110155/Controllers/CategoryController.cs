using Microsoft.AspNetCore.Mvc;
using Dophu_2122110155.Data;
using Dophu_2122110155.Model;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Dophu_2122110155.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách danh mục
        [HttpGet]
        public async Task<IActionResult> GetCategoriesWithProducts()
        {
            var categories = await _context.Category
                .Include(c => c.Products) // Bao gồm các sản phẩm của mỗi category
                .ToListAsync();

            return Ok(categories);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _context.Category
                .Where(c => c.ID == id)
                .Include(c => c.Products)  // Bao gồm các sản phẩm của category này
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }


        // Thêm danh mục mới
        [HttpPost]
        public IActionResult AddCategory([FromBody] Category category)
        {
            _context.Category.Add(category);
            _context.SaveChanges();
            return Ok(category);
        }

        // Sửa danh mục
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category updatedCategory)
        {
            var category = await _context.Category.FirstOrDefaultAsync(c => c.ID == id);
            if (category == null)
                return NotFound(new { message = "Không tìm thấy danh mục" });

            // Chỉ cập nhật tên
            category.Name = updatedCategory.Name;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật thành công", category });
        }


        // Xóa danh mục
        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Category.FirstOrDefault(c => c.ID == id);
            if (category == null)
                return NotFound();

            _context.Category.Remove(category);
            _context.SaveChanges();
            return Ok(new { message = "Xóa thành công" });
        }
    }
}
