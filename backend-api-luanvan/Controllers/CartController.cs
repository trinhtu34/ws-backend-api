using backend_api_luanvan.DataTransferObject;
using backend_api_luanvan.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_backend_api_luanvan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly Dbluanvan2Context _context;
        public CartController(Dbluanvan2Context context)
        {
            _context = context;
        }
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<DTO_Cart>>> GetCartByUserId(string userId)
        {
            var cartItems = await _context.Carts
                .Where(c => c.UserId == userId)
                .Select(c => new DTO_Cart
                {
                    CartId = c.CartId,
                    UserId = c.UserId,
                    OrderTime = c.OrderTime,
                    TotalPrice = c.TotalPrice,
                    IsCancel = c.IsCancel
                }).ToListAsync();
            if (cartItems == null || cartItems.Count == 0)
                return NotFound();
            return Ok(cartItems);
        }
        [HttpGet("user/includepaymentandfinish/{userId}")]
        public async Task<ActionResult<IEnumerable<DTO_Cart_WithPaymentInfo_andIsFinish>>> GetCartByUserIdWithPaymentandFinish(string userId)
        {
            var cartItems = await _context.Carts
                .Where(c => c.UserId == userId)
                .Select(c => new DTO_Cart_WithPaymentInfo_andIsFinish
                {
                    CartId = c.CartId,
                    UserId = c.UserId,
                    OrderTime = c.OrderTime,
                    TotalPrice = c.TotalPrice,
                    IsCancel = c.IsCancel,
                    IsFinish = c.IsFinish,
                    IsPaid = _context.PaymentResults.Any(p => p.CartId == c.CartId && p.IsSuccess == true)
                }).ToListAsync();
            if (cartItems == null || cartItems.Count == 0)
                return NoContent();
            return Ok(cartItems);
        }

        // lấy tất cả thông tin giỏ hàng , kèm theo thông tin thanh toán và trạng thái hoàn thành
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO_Cart_WithPaymentInfo_andIsFinish>>> GetAllCarts()
        {
            var cartItems = await _context.Carts
                .Select(c => new DTO_Cart_WithPaymentInfo_andIsFinish
                {
                    CartId = c.CartId,
                    UserId = c.UserId,
                    OrderTime = c.OrderTime,
                    TotalPrice = c.TotalPrice,
                    IsCancel = c.IsCancel,
                    IsFinish = c.IsFinish,
                    IsPaid = _context.PaymentResults.Any(p => p.CartId == c.CartId && p.IsSuccess == true)
                }).ToListAsync();
            if (cartItems == null || cartItems.Count == 0)
                return NotFound();
            return Ok(cartItems);
        }

        // lấy thông tin giỏ hàng từ 2 tiếng trước trở về sau 
        [HttpGet("afterCurrentOrderTime")]
        public async Task<ActionResult<IEnumerable<DTO_Cart_WithPaymentInfo_andIsFinish>>> GetCartsAfterCurrentOrderTime()
        {
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone).AddHours(-2);
            var cartItems = await _context.Carts
                .Where(c => c.OrderTime > currentTime && c.IsCancel == false)
                .Select(c => new DTO_Cart_WithPaymentInfo_andIsFinish
                {
                    CartId = c.CartId,
                    UserId = c.UserId,
                    OrderTime = c.OrderTime,
                    TotalPrice = c.TotalPrice,
                    IsCancel = c.IsCancel,
                    IsFinish = c.IsFinish,
                    IsPaid = _context.PaymentResults.Any(p => p.CartId == c.CartId && p.IsSuccess == true)
                }).ToListAsync();
            if (cartItems == null || cartItems.Count == 0)
                return NotFound();
            return Ok(cartItems);
        }

        // api lấy thông tin giỏ hàng từ thời gian trong tuần này ( monday to sunday )
        // api lấy thông tin giỏ hàng từ thời gian trong tháng này ( 1st to last day of month ) 
        // api lấy thông tin giỏ hàng từ thời gian trong quý này 
        // api lấy thông tin giỏ hàng từ thời gian trong năm này ( 1st Jan to 31st Dec )


        // api lấy thông tin giỏ hàng từ 2 khoảng thời gian input 
        [HttpGet("byTimestampRange")]
        public async Task<ActionResult<IEnumerable<DTO_Cart_WithPaymentInfo_andIsFinish>>> GetCartsByTimestampRange(
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            try
            {
                var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

                // Chuyển đổi thời gian input sang UTC để so sánh với database
                var fromUtc = TimeZoneInfo.ConvertTimeToUtc(fromDate, vietnamTimeZone);
                var toUtc = TimeZoneInfo.ConvertTimeToUtc(toDate.AddDays(1).AddSeconds(-1), vietnamTimeZone); // Đến cuối ngày

                var cartItems = await _context.Carts
                    .Where(c => c.IsCancel == false &&
                               _context.PaymentResults.Any(p => p.CartId == c.CartId && p.Timestamp >= fromUtc && p.Timestamp <= toUtc))
                    .Select(c => new DTO_Cart_WithPaymentInfo_andIsFinish
                    {
                        CartId = c.CartId,
                        UserId = c.UserId,
                        OrderTime = c.OrderTime,
                        TotalPrice = c.TotalPrice,
                        IsCancel = c.IsCancel,
                        IsFinish = c.IsFinish,
                        IsPaid = _context.PaymentResults.Any(p => p.CartId == c.CartId && p.IsSuccess == true),
                        //PaymentTimestamp = _context.PaymentResults
                        //    .Where(p => p.CartId == c.CartId && p.Timestamp >= fromUtc && p.Timestamp <= toUtc)
                        //    .OrderByDescending(p => p.Timestamp)
                        //    .Select(p => p.Timestamp)
                        //    .FirstOrDefault()
                    })
                    .ToListAsync();

                if (cartItems == null || cartItems.Count == 0)
                    return NotFound($"Không tìm thấy giỏ hàng nào có thanh toán từ {fromDate:dd/MM/yyyy} đến {toDate:dd/MM/yyyy}");

                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }
        // lấy thông tin giỏ hàng theo mã giỏ hàng 
        [HttpGet("{cartId}")]
        public async Task<ActionResult<DTO_Cart>> GetCartById(int cartId)
        {
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var cartItem = await _context.Carts
                .Where(c => c.CartId == cartId)
                .Select(c => new DTO_Cart
                {
                    CartId = c.CartId,
                    UserId = c.UserId,
                    OrderTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone),
                    TotalPrice = c.TotalPrice,
                    IsCancel = c.IsCancel
                }).FirstOrDefaultAsync();
            if (cartItem == null)
                return NotFound();
            return Ok(cartItem);
        }

        [HttpPost]
        public async Task<ActionResult<DTO_Cart>> CreateCart(DTO_Cart dtoCart)
        {
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var newCart = new Cart
            {
                UserId = dtoCart.UserId,
                OrderTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone),
                TotalPrice = dtoCart.TotalPrice,
                IsCancel = false
            };
            _context.Carts.Add(newCart);
            await _context.SaveChangesAsync();
            dtoCart.CartId = newCart.CartId;
            dtoCart.OrderTime = newCart.OrderTime;
            dtoCart.IsCancel = newCart.IsCancel;
            return CreatedAtAction(nameof(GetCartById), new { cartId = newCart.CartId }, dtoCart);
        }

        [HttpPut("{cartId}")]
        public async Task<IActionResult> UpdateCart(int cartId, DTO_Cart dtoCart)
        {
            var existingCart = await _context.Carts.FindAsync(cartId);
            if (existingCart == null)
                return NotFound();
            existingCart.IsCancel = dtoCart.IsCancel;
            _context.Entry(existingCart).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // api thay đổi trạng thái của 1 giỏ hàng 
        [HttpPut("state/{id}")]
        public async Task<ActionResult<DTO_Cart_Cancel_Status>> UpdateOrderTableByState(long id, [FromBody] DTO_Cart_Cancel_Status dto)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
                return NotFound();

            cart.IsCancel = dto.IsCancel;
            _context.Entry(cart).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // api thay đổi trạng thái hoàn thành của 1 giỏ hàng 
        [HttpPut("stateFinish/{id}")]
        public async Task<ActionResult<DTO_Cart_Finish_Status>> UpdateOrderTableByState(long id, [FromBody] DTO_Cart_Finish_Status dto)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
                return NotFound();

            cart.IsFinish = dto.IsFinish;
            _context.Entry(cart).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // đếm số lượng tất cả đơn đặt món ăn ( giỏ hàng ) của 1 user id
        [HttpGet("user/count/{userid}")]
        public async Task<ActionResult<int>> GetCartCountByUserId(string userid)
        {
            var count = await _context.Carts
                .Where(c => c.UserId == userid && c.IsCancel == false)
                .CountAsync();
            return Ok(count);
        }
        // đếm số lượng giỏ hàng đã thanh toán của 1 user id
        [HttpGet("user/paid/count/{userid}")]
        public async Task<ActionResult<int>> GetPaidCartCountByUserId(string userid)
        {
            var count = await _context.Carts
                .Where(c => c.UserId == userid && c.IsCancel == false)
                .Where(c => _context.PaymentResults
                    .Any(p => p.CartId == c.CartId && p.IsSuccess == true))
                .CountAsync();
            return Ok(count);
        }
        // đếm số lượng giỏ hàng chưa thanh toán của 1 user id
        [HttpGet("user/unpaid/count/{userid}")]
        public async Task<ActionResult<int>> GetUnpaidCartCountByUserId(string userid)
        {
            var count = await _context.Carts
                .Where(c => c.UserId == userid && c.IsCancel == false)
                .Where(c => !_context.PaymentResults
                    .Any(p => p.CartId == c.CartId && p.IsSuccess == true))
                .CountAsync();
            return Ok(count);
        }
        // tổng giá trị của tất cả giỏ hàng của 1 user id
        [HttpGet("user/totalPrice/{userId}")]
        public async Task<ActionResult<decimal>> GetTotalPriceByUserId(string userId)
        {
            var totalPrice = await _context.Carts
                .Where(c => c.UserId == userId && c.IsCancel == false)
                .SumAsync(c => c.TotalPrice);
            return Ok(totalPrice);
        }
        // tổng giá trị của giỏ hàng đã thanh toán của 1 user id
        [HttpGet("user/totalPrice/paid/{userId}")]
        public async Task<ActionResult<decimal>> GetTotalPaidPriceByUserId(string userId)
        {
            var totalPrice = await _context.Carts
                .Where(c => c.UserId == userId && c.IsCancel == false)
                .Where(c => _context.PaymentResults
                    .Any(p => p.CartId == c.CartId && p.IsSuccess == true))
                .SumAsync(c => c.TotalPrice);
            return Ok(totalPrice);
        }
        // tổng giá trị của giỏ hàng chưa thanh toán của 1 user id
        [HttpGet("user/totalPrice/unpaid/{userId}")]
        public async Task<ActionResult<decimal>> GetTotalUnpaidPriceByUserId(string userId)
        {
            var totalPrice = await _context.Carts
                .Where(c => c.UserId == userId && c.IsCancel == false)
                .Where(c => !_context.PaymentResults
                    .Any(p => p.CartId == c.CartId && p.IsSuccess == true))
                .SumAsync(c => c.TotalPrice);
            return Ok(totalPrice);
        }

        // api lấy doanh thu của món ăn theo khoảng thời gian và loại món ăn ------- chưa deploy fucntion này 
        // api này đang bị lỗi trong việc tính tổng số tiền , có thể do thiếu data
        [HttpGet("dish-revenue")]
        public async Task<ActionResult> GetDishRevenue(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int? categoryId = null ,
            [FromQuery] int? regionId = null )
        {
            var orderQuery = _context.OrderFoodDetails
                .Include(ofd => ofd.Dish)
                .ThenInclude(d => d.Category)
                .Include(ofd => ofd.Dish)
                .Include(ofd => ofd.Dish.Region)
                .Include(ofd => ofd.OrderTable)
                .ThenInclude(ot => ot.PaymentResults)
                .Where(ofd => ofd.OrderTable.IsCancel != true &&
                             ofd.OrderTable.PaymentResults.Any(pr => pr.IsSuccess == true && pr.Amount == ofd.OrderTable.TotalPrice));

            var cartQuery = _context.CartDetails
                .Include(cd => cd.Dish)
                .ThenInclude(d => d.Category)
                .Include(cd => cd.Dish)
                .Include(cd => cd.Dish.Region)
                .Include(cd => cd.Cart)
                .ThenInclude(c => c.PaymentResults)
                .Where(cd => cd.Cart.IsCancel != true &&
                            cd.Cart.IsFinish == true &&
                            cd.Cart.PaymentResults.Any(pr => pr.IsSuccess == true && pr.Amount == cd.Cart.TotalPrice));

            if (startDate.HasValue)
                orderQuery = orderQuery.Where(ofd => ofd.OrderTable.OrderDate >= startDate.Value);
            if (endDate.HasValue)
                orderQuery = orderQuery.Where(ofd => ofd.OrderTable.OrderDate <= endDate.Value);

            if (startDate.HasValue)
                cartQuery = cartQuery.Where(cd => cd.Cart.OrderTime >= startDate.Value);
            if (endDate.HasValue)
                cartQuery = cartQuery.Where(cd => cd.Cart.OrderTime <= endDate.Value);

            if (categoryId.HasValue)
                orderQuery = orderQuery.Where(ofd => ofd.Dish.CategoryId == categoryId.Value);
            if (categoryId.HasValue)
                cartQuery = cartQuery.Where(cd => cd.Dish.CategoryId == categoryId.Value);

            if (regionId.HasValue)
                orderQuery = orderQuery.Where(ofd => ofd.Dish.RegionId == regionId.Value);
            if (regionId.HasValue)
                cartQuery = cartQuery.Where(cd => cd.Dish.RegionId == regionId.Value);

            // phần này chỉ tính cho ordertable thôi
            var orderRevenue = await orderQuery
                .GroupBy(ofd => new {
                    ofd.DishId,
                    ofd.Dish.DishName,
                    ofd.Dish.Category.CategoryName,
                    ofd.Dish.Region.RegionName,
                    ofd.Dish.Price
                })
                .Select(g => new
                {
                    DishId = g.Key.DishId,
                    DishName = g.Key.DishName,
                    CategoryName = g.Key.CategoryName,
                    RegionName = g.Key.RegionName,
                    UnitPrice = g.Key.Price,
                    TotalQuantitySold = g.Sum(x => x.Quantity),
                    // Vừa sửa đoạn này 16:33 25/07/2025
                    TotalRevenue = g.Sum(x => x.Price * x.Quantity),
                    OrderCount = g.Count(),
                    Source = "OrderTable"
                })
                .ToListAsync();

            // chỗ này tính cho cart , xong là gộp vào , vậy thôi
            var cartRevenue = await cartQuery
                .GroupBy(cd => new {
                    cd.DishId,
                    cd.Dish.DishName,
                    cd.Dish.Category.CategoryName,
                    cd.Dish.Region.RegionName,
                    cd.Dish.Price,
                })
                .Select(g => new
                {
                    DishId = g.Key.DishId,
                    DishName = g.Key.DishName,
                    CategoryName = g.Key.CategoryName,
                    RegionName = g.Key.RegionName,
                    UnitPrice = g.Key.Price,
                    TotalQuantitySold = g.Sum(x => x.Quantity ?? 0),
                    // Vừa sửa đoạn này 16:33 25/07/2025
                    TotalRevenue = g.Sum(x => (x.Price ?? 0) * (x.Quantity ?? 0)),
                    OrderCount = g.Count(),
                    Source = "Cart"
                })
                .ToListAsync();

            // gộp cả 2 vào 1 danh sách
            var combinedRevenue = orderRevenue.Concat(cartRevenue)
                .GroupBy(x => new { x.DishId, x.DishName, x.CategoryName, x.UnitPrice , x.RegionName})
                .Select(g => new
                {
                    DishId = g.Key.DishId,
                    DishName = g.Key.DishName,
                    CategoryName = g.Key.CategoryName,
                    RegionName = g.Key.RegionName,
                    UnitPrice = g.Key.UnitPrice,
                    TotalQuantitySold = g.Sum(x => x.TotalQuantitySold),
                    TotalRevenue = g.Sum(x => x.TotalRevenue),
                    OrderCount = g.Sum(x => x.OrderCount),
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();

            return Ok(combinedRevenue);
        }


        // lấy thông tin xem số lượng đặt của từng loại món ăn theo khu vực 
        [HttpGet("revenue-by-category-region")]
        public async Task<ActionResult> GetRevenueByCategoryAndRegion(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var orderQuery = _context.OrderFoodDetails
                .Include(ofd => ofd.Dish)
                .ThenInclude(d => d.Category)
                .Include(ofd => ofd.OrderTable)
                .ThenInclude(ot => ot.PaymentResults)
                .Include(ofd => ofd.OrderTable)
                .ThenInclude(ot => ot.OrderTablesDetails)
                .ThenInclude(otd => otd.Table)
                .ThenInclude(t => t.Region)
                .Where(ofd => ofd.OrderTable.IsCancel != true &&
                             ofd.OrderTable.PaymentResults.Any(pr => pr.IsSuccess == true && pr.Amount == ofd.OrderTable.TotalPrice));

            var cartQuery = _context.CartDetails
                .Include(cd => cd.Dish)
                .ThenInclude(d => d.Category)
                .Include(cd => cd.Cart)
                .ThenInclude(c => c.PaymentResults)
                .Where(cd => cd.Cart.IsCancel != true &&
                            cd.Cart.IsFinish == true &&
                            cd.Cart.PaymentResults.Any(pr => pr.IsSuccess == true));

            if (startDate.HasValue)
                orderQuery = orderQuery.Where(ofd => ofd.OrderTable.StartingTime >= startDate.Value);
            if (endDate.HasValue)
                orderQuery = orderQuery.Where(ofd => ofd.OrderTable.StartingTime <= endDate.Value);

            if (startDate.HasValue)
                cartQuery = cartQuery.Where(cd => cd.Cart.OrderTime >= startDate.Value);
            if (endDate.HasValue)
                cartQuery = cartQuery.Where(cd => cd.Cart.OrderTime <= endDate.Value);

            // thống kê theo region và category từ ordertable
            var orderRegionCategoryRevenue = await orderQuery
                .SelectMany(ofd => ofd.OrderTable.OrderTablesDetails.Select(otd => new
                {
                    RegionId = otd.Table.RegionId,
                    RegionName = otd.Table.Region.RegionName,
                    CategoryId = ofd.Dish.CategoryId,
                    CategoryName = ofd.Dish.Category.CategoryName,
                    DishId = ofd.DishId,
                    Quantity = ofd.Quantity,
                    Revenue = ofd.Price * ofd.Quantity,
                    OrderTableId = ofd.OrderTableId
                }))
                .GroupBy(x => new {
                    x.RegionId,
                    x.RegionName,
                    x.CategoryId,
                    x.CategoryName
                })
                .Select(g => new
                {
                    RegionId = g.Key.RegionId,
                    RegionName = g.Key.RegionName,
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,
                    TotalQuantitySold = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Revenue),
                    DishCount = g.Select(x => x.DishId).Distinct().Count(),
                    OrderCount = g.Select(x => x.OrderTableId).Distinct().Count(),
                    Source = "OrderTable"
                })
                .ToListAsync();

            // thống kê cho cart , chưa có region
            var cartCategoryRevenue = await cartQuery
                .GroupBy(cd => new {
                    cd.Dish.CategoryId,
                    cd.Dish.Category.CategoryName
                })
                .Select(g => new
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,
                    TotalQuantitySold = g.Sum(x => x.Quantity ?? 0),
                    TotalRevenue = g.Sum(x => (x.Price ?? 0) * (x.Quantity ?? 0)),
                    DishCount = g.Select(x => x.DishId).Distinct().Count(),
                    OrderCount = g.Select(x => x.CartId).Distinct().Count(),
                    Source = "Cart"
                })
                .ToListAsync();

            var result = new
            {
                // Thống kê theo Region và Category từ mỗi ordertable thôi -- hiện tại đang bị sai 
                RegionCategoryStats = orderRegionCategoryRevenue
                    .OrderBy(x => x.RegionName)
                    .ThenByDescending(x => x.TotalRevenue)
                    .ToList(),

                // Tổng quan theo Categorycái này gộp cả ordertable và cart bằng concat ----- 
                OverallCategoryStats = orderRegionCategoryRevenue
                    .GroupBy(x => new { x.CategoryId, x.CategoryName })
                    .Select(g => new
                    {
                        CategoryId = g.Key.CategoryId,
                        CategoryName = g.Key.CategoryName,
                        TotalQuantitySold = g.Sum(x => x.TotalQuantitySold),
                        TotalRevenue = g.Sum(x => x.TotalRevenue),
                        DishCount = g.Sum(x => x.DishCount),
                        OrderTableCount = g.Sum(x => x.OrderCount)
                    })
                    .Concat(cartCategoryRevenue.Select(c => new
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName,
                        TotalQuantitySold = c.TotalQuantitySold,
                        TotalRevenue = c.TotalRevenue,
                        DishCount = c.DishCount,
                        OrderTableCount = 0
                    }))
                    .GroupBy(x => new { x.CategoryId, x.CategoryName })
                    .Select(g => new
                    {
                        CategoryId = g.Key.CategoryId,
                        CategoryName = g.Key.CategoryName,
                        TotalQuantitySold = g.Sum(x => x.TotalQuantitySold),
                        TotalRevenue = g.Sum(x => x.TotalRevenue),
                        OrderTableCount = g.Sum(x => x.OrderTableCount),
                        CartCount = cartCategoryRevenue.FirstOrDefault(c => c.CategoryId == g.Key.CategoryId)?.OrderCount ?? 0
                    })
                    .OrderByDescending(x => x.TotalRevenue)
                    .ToList(),

                //CartStats = cartCategoryRevenue.OrderByDescending(x => x.TotalRevenue).ToList()
            };

            return Ok(result);
        }


        // lấy thông tin doanh thu theo loại món ăn theo thời gian , ý là in ra loại món ăn và doanh thu của nó 
        [HttpGet("revenue-by-category")]
        public async Task<ActionResult> GetRevenueByCategory(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var orderQuery = _context.OrderFoodDetails
                .Include(ofd => ofd.Dish)
                .ThenInclude(d => d.Category)
                .Include(ofd => ofd.OrderTable)
                .ThenInclude(ot => ot.PaymentResults)
                .Where(ofd => ofd.OrderTable.IsCancel != true &&
                             ofd.OrderTable.PaymentResults.Any(pr => pr.IsSuccess == true && pr.Amount == ofd.OrderTable.TotalPrice));

            var cartQuery = _context.CartDetails
                .Include(cd => cd.Dish)
                .ThenInclude(d => d.Category)
                .Include(cd => cd.Cart)
                .ThenInclude(c => c.PaymentResults)
                .Where(cd => cd.Cart.IsCancel != true &&
                            cd.Cart.IsFinish == true &&
                            cd.Cart.PaymentResults.Any(pr => pr.IsSuccess == true && pr.Amount == cd.Cart.TotalPrice));

            if (startDate.HasValue)
                orderQuery = orderQuery.Where(ofd => ofd.OrderTable.OrderDate >= startDate.Value);
            if (endDate.HasValue)
                orderQuery = orderQuery.Where(ofd => ofd.OrderTable.OrderDate <= endDate.Value);
            if (startDate.HasValue)
                cartQuery = cartQuery.Where(cd => cd.Cart.OrderTime >= startDate.Value);
            if (endDate.HasValue)
                cartQuery = cartQuery.Where(cd => cd.Cart.OrderTime <= endDate.Value);

            var orderCategoryRevenue = await orderQuery
                .GroupBy(ofd => new {
                    ofd.Dish.CategoryId,
                    ofd.Dish.Category.CategoryName
                })
                .Select(g => new
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,
                    TotalQuantitySold = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Price * x.Quantity),
                    DishIds = g.Select(x => x.DishId).Distinct().ToList(), // Lưu danh sách DishId
                    OrderCount = g.Count(),
                    Source = "OrderTable"
                })
                .ToListAsync();

            var cartCategoryRevenue = await cartQuery
                .GroupBy(cd => new {
                    cd.Dish.CategoryId,
                    cd.Dish.Category.CategoryName
                })
                .Select(g => new
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,
                    TotalQuantitySold = g.Sum(x => x.Quantity ?? 0),
                    TotalRevenue = g.Sum(x => (x.Price ?? 0) * (x.Quantity ?? 0)),
                    DishIds = g.Select(x => x.DishId).Distinct().ToList(), // Lưu danh sách DishId
                    OrderCount = g.Count(),
                    Source = "Cart"
                })
                .ToListAsync();

            // lỗi ở đây , lấy thiếu vì món nào được khách đặt mới thống kê , không thì nó sẽ không lấy data
            var dishCountByCategory = await _context.Menus
                .Include(d => d.Category)
                .GroupBy(d => new { d.CategoryId, d.Category.CategoryName })
                .Select(g => new
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,
                    TotalDishCount = g.Count()
                })
                .ToDictionaryAsync(x => x.CategoryId, x => x.TotalDishCount);

            var combinedCategoryRevenue = orderCategoryRevenue.Concat(cartCategoryRevenue)
                .GroupBy(x => new { x.CategoryId, x.CategoryName })
                .Select(g => new
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,
                    TotalQuantitySold = g.Sum(x => x.TotalQuantitySold),
                    TotalRevenue = g.Sum(x => x.TotalRevenue),
                    DishCount = dishCountByCategory.GetValueOrDefault(g.Key.CategoryId, 0),
                    OrderedDishCount = g.SelectMany(x => x.DishIds).Distinct().Count(),
                    OrderCount = g.Sum(x => x.OrderCount)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();

            return Ok(combinedCategoryRevenue);
        }
        [HttpGet("revenue-by-region")]
        public async Task<ActionResult> GetRevenueByRegion(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var orderQuery = _context.OrderFoodDetails
                .Include(ofd => ofd.Dish)
                .ThenInclude(d => d.Region)
                .Include(ofd => ofd.OrderTable)
                .ThenInclude(ot => ot.PaymentResults)
                .Where(ofd => ofd.OrderTable.IsCancel != true &&
                             ofd.OrderTable.PaymentResults.Any(pr => pr.IsSuccess == true && pr.Amount == ofd.OrderTable.TotalPrice));

            var cartQuery = _context.CartDetails
                .Include(cd => cd.Dish)
                .ThenInclude(d => d.Region)
                .Include(cd => cd.Cart)
                .ThenInclude(c => c.PaymentResults)
                .Where(cd => cd.Cart.IsCancel != true &&
                            cd.Cart.IsFinish == true &&
                            cd.Cart.PaymentResults.Any(pr => pr.IsSuccess == true && pr.Amount == cd.Cart.TotalPrice));

            if (startDate.HasValue)
                orderQuery = orderQuery.Where(ofd => ofd.OrderTable.OrderDate >= startDate.Value);
            if (endDate.HasValue)
                orderQuery = orderQuery.Where(ofd => ofd.OrderTable.OrderDate <= endDate.Value);

            if (startDate.HasValue)
                cartQuery = cartQuery.Where(cd => cd.Cart.OrderTime >= startDate.Value);
            if (endDate.HasValue)
                cartQuery = cartQuery.Where(cd => cd.Cart.OrderTime <= endDate.Value);

            var orderRegionRevenue = await orderQuery
                .GroupBy(ofd => new {
                    ofd.Dish.RegionId,
                    ofd.Dish.Region.RegionName
                })
                .Select(g => new
                {
                    RegionId = g.Key.RegionId,
                    RegionName = g.Key.RegionName,
                    TotalQuantitySold = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Price * x.Quantity),
                    DishIds = g.Select(x => x.DishId).Distinct().ToList(),
                    OrderCount = g.Count(),
                    Source = "OrderTable"
                })
                .ToListAsync();

            var cartRegionRevenue = await cartQuery
                .GroupBy(cd => new {
                    cd.Dish.RegionId,
                    cd.Dish.Region.RegionName
                })
                .Select(g => new
                {
                    RegionId = g.Key.RegionId,
                    RegionName = g.Key.RegionName,
                    TotalQuantitySold = g.Sum(x => x.Quantity ?? 0),
                    TotalRevenue = g.Sum(x => (x.Price ?? 0) * (x.Quantity ?? 0)),
                    DishIds = g.Select(x => x.DishId).Distinct().ToList(),
                    OrderCount = g.Count(),
                    Source = "Cart"
                })
                .ToListAsync();
            var dishCountByRegion = await _context.Menus
                .GroupBy(d => new { d.RegionId, d.Region.RegionName })
                .Select(g => new
                {
                    RegionId = g.Key.RegionId,
                    RegionName = g.Key.RegionName,
                    TotalDishCount = g.Count()
                })
                .ToDictionaryAsync(x => x.RegionId, x => x.TotalDishCount);

            var combinedRegionRevenue = orderRegionRevenue.Concat(cartRegionRevenue)
                .GroupBy(x => new { x.RegionId, x.RegionName })
                .Select(g => new
                {
                    RegionId = g.Key.RegionId,
                    RegionName = g.Key.RegionName,
                    TotalQuantitySold = g.Sum(x => x.TotalQuantitySold),
                    TotalRevenue = g.Sum(x => x.TotalRevenue),
                    DishCount = dishCountByRegion.GetValueOrDefault(g.Key.RegionId, 0),
                    OrderedDishCount = g.SelectMany(x => x.DishIds).Distinct().Count(),
                    OrderCount = g.Sum(x => x.OrderCount),
                    OrderTableCount = g.Where(x => x.Source == "OrderTable").Sum(x => x.OrderCount),
                    CartCount = g.Where(x => x.Source == "Cart").Sum(x => x.OrderCount)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();

            return Ok(combinedRegionRevenue);
        }

    }
}
