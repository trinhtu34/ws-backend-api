using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend_api_luanvan.Models;
using backend_api_luanvan.DataTransferObject;

namespace backend_api_luanvan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly Dbluanvan2Context _context;

        public CategoryController(Dbluanvan2Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO_Category>>> GetAllCategories()
        {
            return await _context.Categories
                .Select(c => new DTO_Category
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DTO_Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return new DTO_Category
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName
            };
        }

        [HttpPost]
        public async Task<ActionResult<DTO_Category>> PostCategory([FromBody] DTO_Category dto)
        {
            var category = new Category
            {
                CategoryName = dto.CategoryName
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            dto.CategoryId = category.CategoryId;

            return CreatedAtAction(nameof(GetCategory), new { id = dto.CategoryId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DTO_Category>> UpdateCategory(int id,[FromBody] DTO_Category dto)
        {
            if (id != dto.CategoryId)
            {
                return BadRequest("ID in URL and Body do not match");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            category.CategoryName = dto.CategoryName;
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new DTO_Category
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName
            });
        }

        //// DELETE: api/Category/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}
