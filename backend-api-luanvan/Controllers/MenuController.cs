using backend_api_luanvan.DataTransferObject;
using backend_api_luanvan.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_api_luanvan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly Dbluanvan2Context _context;

        public MenuController(Dbluanvan2Context context)
        {
            _context = context;
        }

        // tất cả phương thức GET đều trả về DTO_MenuWithoutAvailabel , và đặt if là IsAvailable = true
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO_MenuWithoutAvailabel>>> GetAllMenus()
        {
            return await _context.Menus
                .Where(m => m.IsAvailable == true)
                .Select(m => new DTO_MenuWithoutAvailabel
                {
                    DishId = m.DishId,
                    DishName = m.DishName,
                    Price = m.Price,
                    Descriptions = m.Descriptions,
                    CategoryId = m.CategoryId,
                    RegionId = m.RegionId,
                    Images = m.Images
                })
                .ToListAsync();
        }
        [HttpGet("quantity")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllMenusByQuantity()
        {
            // B1: Lấy dữ liệu từ DB, thực hiện group và select đơn giản
            var groupedData = await _context.Menus
                .Where(m => m.IsAvailable == true)
                .GroupBy(m => EF.Functions.Collate(m.DishName, "utf8mb4_general_ci"))
                .Select(g => new
                {
                    Count = g.Count(),
                    DishId = g.Min(x => x.DishId) // Lấy DishId nhỏ nhất (ổn định)
                })
                .ToListAsync();

            // B2: Join lại để lấy chi tiết món ăn theo DishId
            var dishIds = groupedData.Select(x => x.DishId).ToList();

            var dishDetails = await _context.Menus
                .Where(m => dishIds.Contains(m.DishId))
                .ToListAsync();

            // B3: Merge dữ liệu và trả về
            var result = groupedData
                .Join(dishDetails, g => g.DishId, d => d.DishId, (g, d) => new
                {
                    g.Count,
                    DishId = d.DishId,
                    DishName = d.DishName,
                    Price = d.Price,
                    Descriptions = d.Descriptions,
                    CategoryId = d.CategoryId,
                    RegionId = d.RegionId,
                    Images = d.Images
                })
                .ToList();

            return Ok(result);
        }
        [HttpGet("quantity/excludecount")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllMenusByQuantity2()
        {
            // B1: Lấy dữ liệu từ DB, thực hiện group và select đơn giản
            var groupedData = await _context.Menus
                .Where(m => m.IsAvailable == true)
                .GroupBy(m => EF.Functions.Collate(m.DishName, "utf8mb4_general_ci"))
                .Select(g => new
                {
                    Count = g.Count(),
                    DishId = g.Min(x => x.DishId) // Lấy DishId nhỏ nhất (ổn định)
                })
                .ToListAsync();

            // B2: Join lại để lấy chi tiết món ăn theo DishId
            var dishIds = groupedData.Select(x => x.DishId).ToList();

            var dishDetails = await _context.Menus
                .Where(m => dishIds.Contains(m.DishId))
                .ToListAsync();

            // B3: Merge dữ liệu và trả về
            var result = groupedData
                .Join(dishDetails, g => g.DishId, d => d.DishId, (g, d) => new
                {
                    DishId = d.DishId,
                    DishName = d.DishName,
                    Price = d.Price,
                    Descriptions = d.Descriptions,
                    CategoryId = d.CategoryId,
                    RegionId = d.RegionId,
                    Images = d.Images
                })
                .ToList();

            return Ok(result);
        }
        [HttpGet("quantity/count")]
        public async Task<ActionResult<int>> GetDistinctMenuCount()
        {
            var count = await _context.Menus
                .Where(m => m.IsAvailable == true)
                .GroupBy(m => EF.Functions.Collate(m.DishName, "utf8mb4_general_ci"))
                .CountAsync(); // đếm số nhóm theo tên món ăn (không phân biệt hoa thường)

            return Ok(count);
        }



        [HttpGet("byadmin")]
        public async Task<ActionResult<IEnumerable<DTO_MenuFull>>> GetAllMenusByAdmin()
        {
            return await _context.Menus
                .Select(m => new DTO_MenuFull
                {
                    DishId = m.DishId,
                    DishName = m.DishName,
                    Price = m.Price,
                    Descriptions = m.Descriptions,
                    CategoryId = m.CategoryId,
                    RegionId = m.RegionId,
                    Images = m.Images,
                    IsAvailable = m.IsAvailable
                })
                .ToListAsync();
        }
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<DTO_MenuWithoutAvailabel>>> GetMenusByCategoryId(int categoryId)
        {
            var menus = await _context.Menus
                .Where(m => m.CategoryId == categoryId && m.IsAvailable == true)
                .Select(m => new DTO_MenuWithoutAvailabel
                {
                    DishId = m.DishId,
                    DishName = m.DishName,
                    Price = m.Price,
                    Descriptions = m.Descriptions,
                    CategoryId = m.CategoryId,
                    RegionId = m.RegionId,
                    Images = m.Images
                })
                .ToListAsync();

            if (menus == null || !menus.Any())
                return NotFound();

            return menus;
        }

        [HttpGet("region/{regionId}")]
        public async Task<ActionResult<IEnumerable<DTO_MenuWithoutAvailabel>>> GetMenusByRegionId(int regionId)
        {
            var menus = await _context.Menus
                .Where(m => m.RegionId == regionId && m.IsAvailable == true)
                .Select(m => new DTO_MenuWithoutAvailabel
                {
                    DishId = m.DishId,
                    DishName = m.DishName,
                    Price = m.Price,
                    Descriptions = m.Descriptions,
                    CategoryId = m.CategoryId,
                    RegionId = m.RegionId,
                    Images = m.Images
                })
                .ToListAsync();
            if (menus == null || !menus.Any())
                return NotFound();
            return menus;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<DTO_MenuWithoutAvailabel>> GetMenu(string id)
        {
            var menu = await _context.Menus
                .Where(m => m.DishId == id && m.IsAvailable == true)
                .FirstOrDefaultAsync();

            if (menu == null)
                return NotFound();

            return new DTO_MenuWithoutAvailabel
            {
                DishId = menu.DishId,
                DishName = menu.DishName,
                Price = menu.Price,
                Descriptions = menu.Descriptions,
                CategoryId = menu.CategoryId,
                RegionId = menu.RegionId,
                Images = menu.Images
            };
        }
        // method này chỉ dành cho admin để chỉnh sửa menu , nó trả về cả thuộc tính isAvailable
        [HttpGet("admin/edit/{id}")]
        public async Task<ActionResult<DTO_MenuFull>> GetMenuForEdit(string id)
        {
            var menu = await _context.Menus
                .Where(m => m.DishId == id)
                .FirstOrDefaultAsync();

            if (menu == null)
                return NotFound();

            return new DTO_MenuFull
            {
                DishId = menu.DishId,
                DishName = menu.DishName,
                Price = menu.Price,
                Descriptions = menu.Descriptions,
                CategoryId = menu.CategoryId,
                RegionId = menu.RegionId,
                Images = menu.Images,
                IsAvailable = menu.IsAvailable
            };
        }

        // thử nghiệm là thêm thuộc tính isAvailable vào DTO_MenuFull , không làm thay đổi các phương thức get ở trên , vì thuộc tính isAvailable này được thêm vào 1 thời gian sau khi project đã chạy , nên mới phải làm như vậy
        [HttpPost]
        public async Task<ActionResult<DTO_MenuWithoutAvailabel>> CreateMenu([FromBody] DTO_MenuWithoutAvailabel dto)
        {
            var menu = new Menu
            {
                DishId = dto.DishId,
                DishName = dto.DishName,
                Price = dto.Price,
                Descriptions = dto.Descriptions,
                CategoryId = dto.CategoryId,
                RegionId = dto.RegionId,
                IsAvailable = false,
                Images = dto.Images
            };
            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMenu), new { id = menu.DishId }, menu);
            //return Ok();
        }

        // api sửa thông tin của 1 đơn đặt món ăn , chỉ dành cho admin
        [HttpPut("admin/{id}")]
        public async Task<ActionResult<DTO_MenuFull>> UpdateMenu(string id, [FromBody] DTO_MenuFull dto)
        {
            if (id != dto.DishId)
                return BadRequest("DishId in URL and body do not match.");

            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
                return NotFound();

            menu.DishName = dto.DishName;
            menu.Price = dto.Price;
            menu.Descriptions = dto.Descriptions;
            menu.CategoryId = dto.CategoryId;
            menu.RegionId = dto.RegionId;
            menu.Images = dto.Images;
            menu.IsAvailable = dto.IsAvailable;

            _context.Entry(menu).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(dto);
        }
        //// DELETE: api/Menu/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenu(string id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
                return NotFound();

            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
