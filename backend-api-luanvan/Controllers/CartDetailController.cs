using backend_api_luanvan.DataTransferObject;
using backend_api_luanvan.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_api_luanvan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartDetailController : ControllerBase
    {
        private readonly Dbluanvan2Context _context;
        public CartDetailController(Dbluanvan2Context context)
        {
            _context = context;
        }
        [HttpGet("cart/{cartId}")]
        public async Task<ActionResult<IEnumerable<DTO_CartDetail_WithName>>> GetCartDetailsByCartId(long cartId)
        {
            var cartDetails = await _context.CartDetails
                .Where(cd => cd.CartId == cartId)
                .Select(cd => new DTO_CartDetail_WithName
                {
                    CartDetailsId = cd.CartDetailsId,
                    CartId = cd.CartId,
                    DishId = cd.DishId,
                    DishName = _context.Menus
                        .Where(d => d.DishId == cd.DishId)
                        .Select(d => d.DishName)
                        .FirstOrDefault() ?? "Unknown Dish",
                    Quantity = cd.Quantity,
                    Price = cd.Price
                }).ToListAsync();
            if (cartDetails == null || cartDetails.Count == 0)
                return NoContent();
            return Ok(cartDetails);
        }
        [HttpPost]
        public async Task<ActionResult<DTO_CartDetail>> CreateCartDetail(DTO_CartDetail dtoCartDetail)
        {
            var cartDetail = new CartDetail
            {
                CartId = dtoCartDetail.CartId,
                DishId = dtoCartDetail.DishId,
                Quantity = dtoCartDetail.Quantity,
                Price = dtoCartDetail.Price
            };
            _context.CartDetails.Add(cartDetail);
            await _context.SaveChangesAsync();
            dtoCartDetail.CartDetailsId = cartDetail.CartDetailsId;
            return CreatedAtAction(nameof(GetCartDetailsByCartId), new { cartId = cartDetail.CartId }, dtoCartDetail);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCartDetail(int id, DTO_CartDetail dtoCartDetail)
        {
            if (id != dtoCartDetail.CartDetailsId)
                return BadRequest();
            var cartDetail = await _context.CartDetails.FindAsync(id);
            if (cartDetail == null)
                return NotFound();
            cartDetail.CartId = dtoCartDetail.CartId;
            cartDetail.DishId = dtoCartDetail.DishId;
            cartDetail.Quantity = dtoCartDetail.Quantity;
            cartDetail.Price = dtoCartDetail.Price;
            _context.Entry(cartDetail).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}