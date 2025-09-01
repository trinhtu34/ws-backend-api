using backend_api_luanvan.DataTransferObject;
using backend_api_luanvan.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_api_luanvan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderFoodDetailController : ControllerBase
    {
        private readonly Dbluanvan2Context _context;
        public OrderFoodDetailController(Dbluanvan2Context context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO_OrderFoodDetail>>> GetAllOrderFoodDetails()
        {
            return await _context.OrderFoodDetails
                .Select(ofd => new DTO_OrderFoodDetail
                {
                    OrderFoodDetailsId = ofd.OrderFoodDetailsId,
                    OrderTableId = ofd.OrderTableId,
                    DishId = ofd.DishId,
                    Quantity = ofd.Quantity,
                    Price = ofd.Price,
                    Note = ofd.Note
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DTO_OrderFoodDetail>> GetOrderFoodDetailById(int id)
        {
            var orderFoodDetail = await _context.OrderFoodDetails.FindAsync(id);
            if (orderFoodDetail == null)
                return NotFound();
            return new DTO_OrderFoodDetail
            {
                OrderFoodDetailsId = orderFoodDetail.OrderFoodDetailsId,
                OrderTableId = orderFoodDetail.OrderTableId,
                DishId = orderFoodDetail.DishId,
                Quantity = orderFoodDetail.Quantity,
                Price = orderFoodDetail.Price,
                Note = orderFoodDetail.Note
            };
        }
        [HttpGet("list/{orderTableId}")]
        public async Task<ActionResult<IEnumerable<DTO_OrderFoodDetail>>> GetOrderFoodDetailsByOrderTableId(long orderTableId)
        {
            var orderFoodDetails = await _context.OrderFoodDetails
                .Where(ofd => ofd.OrderTableId == orderTableId)
                .Select(ofd => new DTO_OrderFoodDetail
                {
                    OrderFoodDetailsId = ofd.OrderFoodDetailsId,
                    OrderTableId = ofd.OrderTableId,
                    DishId = ofd.DishId,
                    Quantity = ofd.Quantity,
                    Price = ofd.Price,
                    Note = ofd.Note
                })
                .ToListAsync();
            if (orderFoodDetails == null || !orderFoodDetails.Any())
                return NotFound();
            return orderFoodDetails;
        }
        [HttpPost]
        public async Task<ActionResult<DTO_OrderFoodDetail>> CreateOrderFoodDetail(DTO_OrderFoodDetail dto)
        {
            var orderFoodDetail = new OrderFoodDetail
            {
                OrderTableId = dto.OrderTableId,
                DishId = dto.DishId,
                Quantity = dto.Quantity,
                Price = dto.Price,
                Note = ""
            };
            _context.OrderFoodDetails.Add(orderFoodDetail);
            await _context.SaveChangesAsync();
            dto.OrderFoodDetailsId = orderFoodDetail.OrderFoodDetailsId;
            return CreatedAtAction(nameof(GetOrderFoodDetailById), new { id = orderFoodDetail.OrderFoodDetailsId }, dto);
        }

        [HttpPut("quantity/{id}")]
        public async Task<IActionResult> UpdateOrderFoodDetail(int id, DTO_OrderFoodDetail_Update_Quantity dto)
        {
            var orderFoodDetail = await _context.OrderFoodDetails.FindAsync(id);
            if (orderFoodDetail == null)
            {
                return NotFound("This orderfood detail is not found in database");
            }

            orderFoodDetail.Quantity += dto.Quantity;
            _context.Entry(orderFoodDetail).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderFoodDetail(int id)
        {
            var orderFoodDetail = await _context.OrderFoodDetails.FindAsync(id);
            if (orderFoodDetail == null)
                return NotFound();
            _context.OrderFoodDetails.Remove(orderFoodDetail);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
