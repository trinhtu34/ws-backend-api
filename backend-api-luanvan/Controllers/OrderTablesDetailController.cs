using backend_api_luanvan.DataTransferObject;
using backend_api_luanvan.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_api_luanvan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderTablesDetailController : ControllerBase
    {
        private readonly Dbluanvan2Context _context;
        public OrderTablesDetailController(Dbluanvan2Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO_OrderTablesDetail>>> GetAllOrderTablesDetails()
        {
            return await _context.OrderTablesDetails
                .Select(otd => new DTO_OrderTablesDetail
                {
                    OrderTablesDetailsId = otd.OrderTablesDetailsId,
                    OrderTableId = otd.OrderTableId,
                    TableId = otd.TableId
                })
                .ToListAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<DTO_OrderTablesDetail>> GetOrderTablesDetailById(int id)
        {
            var orderTableDetail = await _context.OrderTablesDetails.FindAsync(id);
            if (orderTableDetail == null)
                return NotFound();
            return new DTO_OrderTablesDetail
            {
                OrderTablesDetailsId = orderTableDetail.OrderTablesDetailsId,
                OrderTableId = orderTableDetail.OrderTableId,
                TableId = orderTableDetail.TableId
            };
        }

        [HttpGet("list/{orderTableId}")]
        public async Task<ActionResult<IEnumerable<DTO_OrderTablesDetail>>> GetOrderTablesDetailsByOrderTableId(long orderTableId)
        {
            var orderTableDetails = await _context.OrderTablesDetails
                .Where(o => o.OrderTableId == orderTableId)
                .Select(o => new DTO_OrderTablesDetail
                {
                    OrderTablesDetailsId = o.OrderTablesDetailsId,
                    OrderTableId = o.OrderTableId,
                    TableId = o.TableId
                })
                .ToListAsync();

            if (orderTableDetails == null || !orderTableDetails.Any())
                return NotFound();

            return orderTableDetails;
        }

        [HttpPost]
        public async Task<ActionResult<DTO_OrderTablesDetail>> CreateOrderTablesDetail([FromBody] DTO_OrderTablesDetail dto)
        {
            var orderTableDetail = new OrderTablesDetail
            {
                OrderTableId = dto.OrderTableId,
                TableId = dto.TableId
            };
            _context.OrderTablesDetails.Add(orderTableDetail);
            await _context.SaveChangesAsync();
            dto.OrderTablesDetailsId = orderTableDetail.OrderTablesDetailsId;
            return CreatedAtAction(nameof(GetOrderTablesDetailById), new { id = orderTableDetail.OrderTablesDetailsId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderTablesDetail(int id, [FromBody] DTO_OrderTablesDetail dto)
        {
            if (id != dto.OrderTablesDetailsId)
                return BadRequest();
            var orderTableDetail = await _context.OrderTablesDetails.FindAsync(id);
            if (orderTableDetail == null)
                return NotFound();
            orderTableDetail.OrderTableId = dto.OrderTableId;
            orderTableDetail.TableId = dto.TableId;
            _context.Entry(orderTableDetail).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderTablesDetail(int id)
        {
            var orderTableDetail = await _context.OrderTablesDetails.FindAsync(id);
            if (orderTableDetail == null)
                return NotFound();
            _context.OrderTablesDetails.Remove(orderTableDetail);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
public class tbResposen
{
    public int TableId { get; set; }
}