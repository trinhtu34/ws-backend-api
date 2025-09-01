using backend_api_luanvan.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using backend_api_luanvan.DataTransferObject;
using Microsoft.EntityFrameworkCore;

namespace backend_api_luanvan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly Dbluanvan2Context _context;
        public TableController(Dbluanvan2Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO_Table>>> GetAllTables()
        {
            return await _context.Tables
                .Select(t => new DTO_Table
                {
                    TableId = t.TableId,
                    Capacity = t.Capacity,
                    Deposit = t.Deposit,
                    Description = t.Description,
                    RegionId = t.RegionId
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DTO_Table>> GetTableById(int id)
        {
            var table = await _context.Tables.FindAsync(id);
            if (table == null)
                return NotFound();
            return new DTO_Table
            {
                TableId = table.TableId,
                Capacity = table.Capacity,
                Deposit = table.Deposit,
                Description = table.Description,
                RegionId = table.RegionId
            };
        }

        [HttpGet("region/{regionId}")]
        public async Task<ActionResult<IEnumerable<DTO_Table>>> GetTablesByRegionId(int regionId)
        {
            var tables = await _context.Tables
                .Where(t => t.RegionId == regionId)
                .Select(t => new DTO_Table
                {
                    TableId = t.TableId,
                    Capacity = t.Capacity,
                    Deposit = t.Deposit,
                    Description = t.Description,
                    RegionId = t.RegionId
                })
                .ToListAsync();
            return tables;
        }

        [HttpPost]
        public async Task<ActionResult<DTO_Table>> CreateTable([FromBody] DTO_Table dto)
        {
            var table = new Table
            {
                TableId = dto.TableId,
                Capacity = dto.Capacity,
                Deposit = dto.Deposit,
                Description = dto.Description,
                RegionId = dto.RegionId
            };
            _context.Tables.Add(table);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTableById), new { id = table.TableId }, new DTO_Table
            {
                TableId = table.TableId,
                Capacity = table.Capacity,
                Deposit = table.Deposit,
                Description = table.Description,
                RegionId = table.RegionId
            });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTable(int id, [FromBody] DTO_Table dto)
        {
            if (id != dto.TableId)
                return BadRequest();
            var table = await _context.Tables.FindAsync(id);
            if (table == null)
                return NotFound();
            table.Capacity = dto.Capacity;
            table.Deposit = dto.Deposit;
            table.Description = dto.Description;
            table.RegionId = dto.RegionId;
            _context.Entry(table).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTable(int id)
        {
            var table = await _context.Tables.FindAsync(id);
            if (table == null)
                return NotFound();
            _context.Tables.Remove(table);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
