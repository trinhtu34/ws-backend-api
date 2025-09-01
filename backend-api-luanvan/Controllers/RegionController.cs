using backend_api_luanvan.DataTransferObject;
using backend_api_luanvan.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_api_luanvan.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RegionController : ControllerBase
    {
        private readonly Dbluanvan2Context _context;

        public RegionController(Dbluanvan2Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO_Region>>> GetAllRegions()
        {
            return await _context.Regions
                .Select(r => new DTO_Region
                {
                    RegionId = r.RegionId,
                    RegionName = r.RegionName
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DTO_Region>> GetRegion(int id)
        {
            var region = await _context.Regions.FindAsync(id);

            if (region == null)
                return NotFound();

            return new DTO_Region
            {
                RegionId = region.RegionId,
                RegionName = region.RegionName
            };
        }

        [HttpPost]
        public async Task<ActionResult<DTO_Region>> CreateRegion([FromBody] DTO_Region regionDto)
        {
            var region = new Region
            {
                RegionName = regionDto.RegionName
            };

            _context.Regions.Add(region);
            await _context.SaveChangesAsync();

            regionDto.RegionId = region.RegionId;

            return CreatedAtAction(nameof(GetRegion), new { id = region.RegionId }, regionDto);
        }

        //// api đã chuẩn hóa
        //// sửa 
        //[HttpPut("{id}")]
        //public async Task<ActionResult<ApiResponse<DTO_Region>>> UpdateRegion(int id, [FromBody] DTO_Region regionDto)
        //{
        //    if (id != regionDto.RegionId)
        //    {
        //        return BadRequest(new ApiResponse<DTO_Region>
        //        {
        //            StatusCode = 400,
        //            Message = "ID in URL and body do not match",
        //            Data = null
        //        });
        //    }

        //    var region = await _context.Regions.FindAsync(id);
        //    if (region == null)
        //    {
        //        return NotFound(new ApiResponse<DTO_Region>
        //        {
        //            StatusCode = 404,
        //            Message = "Region not found",
        //            Data = null
        //        });
        //    }

        //    region.RegionName = regionDto.RegionName;
        //    _context.Entry(region).State = EntityState.Modified;
        //    await _context.SaveChangesAsync();

        //    var updatedDto = new DTO_Region
        //    {
        //        RegionId = region.RegionId,
        //        RegionName = region.RegionName
        //    };

        //    return Ok(new ApiResponse<DTO_Region>
        //    {
        //        StatusCode = 200,
        //        Message = "Region updated successfully",
        //        Data = updatedDto
        //    });
        //}

        // api chưa chuẩn hóa 
        // PUT: api/Region/2
        [HttpPut("{id}")]
        public async Task<ActionResult<DTO_Region>> UpdateRegion(int id, [FromBody] DTO_Region regionDto)
        {
            // So sánh id từ URL và từ body để đảm bảo khớp
            if (id != regionDto.RegionId)
                return BadRequest("ID in URL and body do not match.");

            var region = await _context.Regions.FindAsync(id);
            if (region == null)
                return NotFound();

            // Cập nhật tên vùng
            region.RegionName = regionDto.RegionName;

            _context.Entry(region).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Trả về dữ liệu đã cập nhật
            return Ok(new DTO_Region
            {
                RegionId = region.RegionId,
                RegionName = region.RegionName
            });
        }

        //// DELETE: api/Region/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegion(int id)
        {
            var region = await _context.Regions.FindAsync(id);
            if (region == null)
                return NotFound();

            _context.Regions.Remove(region);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //// GET: api/Region/search?name=North
        //[HttpGet("search")]
        //public async Task<ActionResult<IEnumerable<DTO_Region>>> SearchRegions(string name)
        //{
        //    return await _context.Regions

        //        .Where(r => r.RegionName.Contains(name))
        //        .Select(r => new DTO_Region
        //        {
        //            RegionId = r.RegionId,
        //            RegionName = r.RegionName
        //        })
        //        .ToListAsync();
        //}
    }
}
