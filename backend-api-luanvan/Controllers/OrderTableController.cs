using backend_api_luanvan.DataTransferObject;
using backend_api_luanvan.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_api_luanvan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderTableController : ControllerBase
    {
        private readonly Dbluanvan2Context _context;
        public OrderTableController(Dbluanvan2Context context)
        {
            _context = context;
        }
        // lấy tất cả đơn đặt bàn
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO_OrderTable>>> GetAllOrderTables()
        {
            return await _context.OrderTables
                .Select(m => new DTO_OrderTable
                {
                    OrderTableId = m.OrderTableId,
                    UserId = m.UserId,
                    StartingTime = m.StartingTime,
                    IsCancel = m.IsCancel,
                    TotalPrice = m.TotalPrice,
                    TotalDeposit = m.TotalDeposit,
                    OrderDate = m.OrderDate
                }).ToListAsync();
        }
        // lọc rồi sắp xếp theo thời gian đặt bàn , không phải là thời gian khách đến
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<DTO_OrderTable_Paymentstatus>>> GetOrderTablesByFilter(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string userId = null, 
            [FromQuery] DateTime? startingTime = null,
            [FromQuery] bool? isCancel = null )
        {
            var query = _context.OrderTables.AsQueryable();
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            if (fromDate.HasValue)
            {
                var fromDateLocal = TimeZoneInfo.ConvertTimeFromUtc(fromDate.Value, vietnamTimeZone);
                query = query.Where(m => m.OrderDate >= fromDateLocal);
            }
            if (toDate.HasValue)
            {
                var toDateLocal = TimeZoneInfo.ConvertTimeFromUtc(toDate.Value, vietnamTimeZone);
                query = query.Where(m => m.OrderDate <= toDateLocal);
            }

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(m => m.UserId == userId);
            }
            if (startingTime.HasValue)
            {
                query = query.Where(m => m.StartingTime >= startingTime.Value);
            }
            if (isCancel.HasValue)
            {
                query = query.Where(m => m.IsCancel == isCancel.Value);
            }
            var orderTables = await query
                .OrderByDescending(m => m.OrderDate)
                .Select(m => new DTO_OrderTable_Paymentstatus
                {
                    OrderTableId = m.OrderTableId,
                    UserId = m.UserId,
                    StartingTime = m.StartingTime,
                    IsCancel = m.IsCancel,
                    TotalPrice = m.TotalPrice,
                    TotalDeposit = m.TotalDeposit,
                    OrderDate = m.OrderDate,
                    IsPaid = _context.PaymentResults.Any(p => p.OrderTableId == m.OrderTableId && p.IsSuccess == true)
                }).ToListAsync();
            if (orderTables == null || orderTables.Count == 0)
                return NoContent();
            return Ok(orderTables);
        }
        // lấy tất cả đơn đặt bàn từ 2 tiếng trước trở về sau , không kèm thông tin thanh toán 
        [HttpGet("afterStartingTime2HoursAgo")]
        public async Task<ActionResult<IEnumerable<DTO_OrderTable>>> GetOrderTableAfterStartingTime3HoursAgo()
        {
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone).AddHours(-2);

            var orderTables = await _context.OrderTables
                .Where(m => m.StartingTime > currentTime && m.IsCancel == false)
                .Select(m => new DTO_OrderTable
                {
                    OrderTableId = m.OrderTableId,
                    UserId = m.UserId,
                    StartingTime = m.StartingTime,
                    IsCancel = m.IsCancel,
                    TotalPrice = m.TotalPrice,
                    TotalDeposit = m.TotalDeposit,
                    OrderDate = m.OrderDate
                }).ToListAsync();

            if (orderTables == null || orderTables.Count == 0)
                return NoContent();

            return Ok(orderTables);
        }
        // lấy tất cả đơn đặt bàn từ 2 phút trước trở về sau , không kèm thông tin thanh toán 
        //[HttpGet("afterStartingTime2MinutesAgo")]
        //public async Task<ActionResult<IEnumerable<DTO_OrderTable>>> GetOrderTableAfterStartingTime2MinutesAgo()
        //{
        //    var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        //    var currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone).AddMinutes(-2);

        //    var orderTables = await _context.OrderTables
        //        .Where(m => m.OrderDate > currentTime && m.IsCancel == false)
        //        .Select(m => new DTO_OrderTable
        //        {
        //            OrderTableId = m.OrderTableId,
        //            UserId = m.UserId,
        //            StartingTime = m.StartingTime,
        //            IsCancel = m.IsCancel,
        //            TotalPrice = m.TotalPrice,
        //            TotalDeposit = m.TotalDeposit,
        //            OrderDate = m.OrderDate
        //        }).ToListAsync();

        //    if (orderTables == null || orderTables.Count == 0)
        //        return NoContent();

        //    return Ok(orderTables);
        //}
        // lấy tất cả đơn đặt bàn từ 1 tiếng trước trở về sau có kèm thông tin thanh toán 
        // api này sử dụng trong trang quản lý đơn đặt bàn của nhân viên

        // cái này hiện đang dùng cho trang đặt bàn cho khách , để tạm đã cân nhắc nâng cấp và xóa sau
        [HttpGet("after15Minutes")]
        public async Task<ActionResult<IEnumerable<DTO_OrderTable_Paymentstatus>>> GetOrderTableAfterCurrentStartingTime()
        {
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone).AddMinutes(-15);

            var orderTables = await _context.OrderTables
                .Where(m => m.StartingTime > currentTime && m.IsCancel == false)
                .Select(m => new DTO_OrderTable_Paymentstatus
                {
                    OrderTableId = m.OrderTableId,
                    UserId = m.UserId,
                    StartingTime = m.StartingTime,
                    IsCancel = m.IsCancel,
                    TotalPrice = m.TotalPrice,
                    TotalDeposit = m.TotalDeposit,
                    OrderDate = m.OrderDate,
                    IsPaid = _context.PaymentResults.Any(p => p.OrderTableId == m.OrderTableId && p.IsSuccess == true)

                }).ToListAsync();

            if (orderTables == null || orderTables.Count == 0)
                return NoContent();

            return Ok(orderTables);
        }
        [HttpGet("afterCurrentStartingTimeNewerVer")]
        public async Task<ActionResult<IEnumerable<DTO_OrderTable_Paymentstatus_Food_payment_info>>> GetOrderTableAfterCurrentStartingTimeNewerVers()
        {
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone).AddHours(-1);

            var orderTables = await _context.OrderTables
                .Where(m => m.StartingTime > currentTime && m.IsCancel == false)
                .Select(m => new DTO_OrderTable_Paymentstatus_Food_payment_info
                {
                    OrderTableId = m.OrderTableId,
                    UserId = m.UserId,
                    StartingTime = m.StartingTime,
                    IsCancel = m.IsCancel,
                    TotalPrice = m.TotalPrice,
                    TotalDeposit = m.TotalDeposit,
                    OrderDate = m.OrderDate,
                    IsPaidDeposit = _context.PaymentResults.Any(p => p.OrderTableId == m.OrderTableId && p.IsSuccess == true && p.Amount == m.TotalDeposit),
                    IsPaidTotalPrice = _context.PaymentResults.Any(p => p.OrderTableId == m.OrderTableId && p.IsSuccess == true && p.Amount == m.TotalPrice),

                }).ToListAsync();

            if (orderTables == null || orderTables.Count == 0)
                return NoContent();

            return Ok(orderTables);
        }

        // api trả về đơn đặt bàn kèm trạng thái thanh toán 
        // hiện đang dùng cho trang quản lý đơn đặt bàn của nhân viên 
        // sửa ngày 07/23/2025 2 thuộc tính thông tin thanh toán 
        [HttpGet("paymentstatus")]
        public async Task<ActionResult<IEnumerable<DTO_OrderTable_Paymentstatus_Food_payment_info>>> GetOrderTablesWithPaymentStatus()
        {
            var orderTables = await _context.OrderTables
                .Select(m => new DTO_OrderTable_Paymentstatus_Food_payment_info
                {
                    OrderTableId = m.OrderTableId,
                    UserId = m.UserId,
                    StartingTime = m.StartingTime,
                    IsCancel = m.IsCancel,
                    TotalPrice = m.TotalPrice,
                    TotalDeposit = m.TotalDeposit,
                    OrderDate = m.OrderDate,
                    IsPaidDeposit = _context.PaymentResults.Any(p => p.OrderTableId == m.OrderTableId && p.IsSuccess == true && p.Amount == m.TotalDeposit),
                    IsPaidTotalPrice = _context.PaymentResults.Any(p => p.OrderTableId == m.OrderTableId && p.IsSuccess == true && p.Amount == m.TotalPrice)
                }).ToListAsync();
            if (orderTables == null || orderTables.Count == 0)
                return NotFound();
            return Ok(orderTables);
        }

        // this function is without payment status
        // hiện tại không còn dùng nữa vì đã có api trả về đơn đặt bàn kèm trạng thái thanh toán                                 XXXXX LƯU Ý !
        // outdated
        [HttpGet("{userid}")]
        public async Task<ActionResult<DTO_OrderTable>> GetOrderTableByUserID(string userid)
        {
            var orderTable = await _context.OrderTables
                .Where(m => m.UserId == userid)
                .Select(m => new DTO_OrderTable
                {
                    OrderTableId = m.OrderTableId,
                    UserId = m.UserId,
                    StartingTime = m.StartingTime,
                    IsCancel = m.IsCancel,
                    TotalPrice = m.TotalPrice,
                    TotalDeposit = m.TotalDeposit,
                    OrderDate = m.OrderDate
                }).ToListAsync();
            if (orderTable == null || orderTable.Count == 0)
                return Ok();
            return Ok(orderTable);
        }

        // lấy thông tin đơn đặt bàn theo id có cả trạng thái thanh toán , cái này dùng để cho việc sửa chức năng đặt bàn sau khi báo cáo xong luận văn 
        // tạm thời đình chỉ api này , có api dưới cải tiến ok hơn 
        // outdated
        [HttpGet("includepaymentstatus/{userid}")]
        public async Task<ActionResult<DTO_OrderTable_Paymentstatus>> GetOrderTableByUserIDHaveStatus(string userid)
        {
            var orderTable = await _context.OrderTables
                .Where(m => m.UserId == userid)
                .Select(m => new DTO_OrderTable_Paymentstatus
                {
                    OrderTableId = m.OrderTableId,
                    UserId = m.UserId,
                    StartingTime = m.StartingTime,
                    IsCancel = m.IsCancel,
                    TotalPrice = m.TotalPrice,
                    TotalDeposit = m.TotalDeposit,
                    OrderDate = m.OrderDate,
                    IsPaid = _context.PaymentResults.Any(p => p.OrderTableId == m.OrderTableId && p.IsSuccess == true)
                }).ToListAsync();
            if (orderTable == null || orderTable.Count == 0)
                return Ok();
            return Ok(orderTable);
        }


        // hiện tại api này dùng cho trang DanhSachDatBan của khách hàng , đang thử nghiệm
        [HttpGet("includepaymentstatusnewvers/{userid}")]
        public async Task<ActionResult<DTO_OrderTable_Paymentstatus_Food_payment_info>> GetOrderTableByUserIDHaveStatusNewVers(string userid)
        {
            var orderTable = await _context.OrderTables
                .Where(m => m.UserId == userid)
                .Select(m => new DTO_OrderTable_Paymentstatus_Food_payment_info
                {
                    OrderTableId = m.OrderTableId,
                    UserId = m.UserId,
                    StartingTime = m.StartingTime,
                    IsCancel = m.IsCancel,
                    TotalPrice = m.TotalPrice,
                    TotalDeposit = m.TotalDeposit,
                    OrderDate = m.OrderDate,
                    IsPaidDeposit = _context.PaymentResults.Any(p => p.OrderTableId == m.OrderTableId && p.IsSuccess == true && p.Amount == m.TotalDeposit),
                    IsPaidTotalPrice = _context.PaymentResults.Any(p => p.OrderTableId == m.OrderTableId && p.IsSuccess == true && p.Amount == m.TotalPrice)
                }).ToListAsync();
            if (orderTable == null || orderTable.Count == 0)
                return Ok();
            return Ok(orderTable);
        }

        // tạo 1 đơn đặt bàn mới 
        [HttpPost]
        public async Task<ActionResult<DTO_OrderTable>> CreateOrderTable([FromBody] DTO_OrderTable dto)
        {
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var orderTable = new OrderTable
            {
                UserId = dto.UserId,
                StartingTime = dto.StartingTime,
                IsCancel = dto.IsCancel,
                TotalPrice = dto.TotalPrice ?? 0,
                TotalDeposit = dto.TotalDeposit ?? 0,
                OrderDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone)
            };
            _context.OrderTables.Add(orderTable);
            await _context.SaveChangesAsync();
            dto.OrderTableId = orderTable.OrderTableId;
            dto.OrderDate = orderTable.OrderDate;
            dto.TotalPrice = orderTable.TotalPrice;
            dto.TotalDeposit = orderTable.TotalDeposit;
            return CreatedAtAction(nameof(GetOrderTableByUserID), new { userid = dto.UserId }, dto);
        }
        // sửa thông tin của 1 đơn đặt bàn , nhưng THỪA
        // api này chưa được sử dụng 
        [HttpPut("{id}")]
        public async Task<ActionResult<DTO_OrderTable>> UpdateOrderTable(long id, [FromBody] DTO_OrderTable dto)
        {
            if (id != dto.OrderTableId)
                return BadRequest();

            var orderTable = await _context.OrderTables.FindAsync(id);
            if (orderTable == null)
                return NotFound();

            //orderTable.UserId = dto.UserId;
            orderTable.StartingTime = dto.StartingTime;
            orderTable.TotalPrice = dto.TotalPrice;
            orderTable.TotalDeposit = dto.TotalDeposit;
            _context.Entry(orderTable).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // api tính tổng giá trị của đơn đặt bàn , tổng giá trị là tổng giá trị của tất cả các món ăn trong đơn đặt bàn đó , sau đó thay thế vào giá trị cũ
        // api này chỉ call 1 lần trong việc thêm món ăn vào đơn đặt bàn cho khách , chưa sử dụng ở nơi nào khác 
        // thêm ngày 07/24/2025
        [HttpPut("totalprice/calculate/{id}")]
        public async Task<ActionResult<DTO_OrderTable>> UpdateOrderTableTotalPriceAndCalculate(long id)
        {
            var orderTable = await _context.OrderTables.FindAsync(id);
            if (orderTable == null)
                return NotFound();
            var totalPrice = await _context.OrderFoodDetails
                .Where(od => od.OrderTableId == id)
                .SumAsync(od => od.Price * od.Quantity);
            orderTable.TotalPrice = totalPrice;
            _context.Entry(orderTable).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // thay đổi trạng thái của 1 đơn đặt bàn
        [HttpPut("state/{id}")]
        public async Task<ActionResult<DTO_OrderTable>> UpdateOrderTableByState(long id, [FromBody] DTO_OrderTable dto)
        {
            var orderTable = await _context.OrderTables.FindAsync(id);
            if (orderTable == null)
                return NotFound();

            //orderTable.UserId = dto.UserId;
            orderTable.IsCancel = dto.IsCancel;
            _context.Entry(orderTable).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // từ đây trở xuống là api phục vụ cho việc thống kê đơn đặt bàn của 1 user
        // lấy số lượng đơn đặt bàn của 1 user
        [HttpGet("user/count/{id}")]
        public async Task<ActionResult<int>> GetOrderTableCountByUserId(string id)
        {
            var count = await _context.OrderTables
                .Where(m => m.UserId == id)
                .CountAsync();
            if (count == 0)
                return NotFound();
            return Ok(count);
        }
        // lấy số lượng đơn đặt bàn đã thanh toán của 1 user
        [HttpGet("user/paid/count/{id}")]
        public async Task<ActionResult<int>> GetpaidOrderTableCountByUserId(string id)
        {
            var count = await _context.OrderTables
                .Where(ot => ot.UserId == id)
                .Where(ot => _context.PaymentResults
                    .Any(p => p.OrderTableId == ot.OrderTableId && p.IsSuccess == true))
                .CountAsync();

            return Ok(count);
        }
        // lấy số lượng đơn đặt bàn chưa thanh toán của 1 user
        [HttpGet("user/unpaid/count/{id}")]
        public async Task<ActionResult<int>> GetUnPaidOrderTableCountByUserId(string id)
        {
            var count = await _context.OrderTables
                .Where(ot => ot.UserId == id)
                .Where(ot => !_context.PaymentResults
                    .Any(p => p.OrderTableId == ot.OrderTableId && p.IsSuccess == true))
                .CountAsync();

            return Ok(count);
        }
        // lấy thông tin đơn đặt bàn đã thanh toán của 1 user
        [HttpGet("user/paid/{userid}")]
        public async Task<ActionResult<IEnumerable<DTO_OrderTable>>> GetPaidOrderTableByUserId(string userid)
        {
            var orderTables = await _context.OrderTables
                .Where(ot => ot.UserId == userid)
                .Where(ot => _context.PaymentResults
                    .Any(p => p.OrderTableId == ot.OrderTableId && p.IsSuccess == true))
                .Select(m => new DTO_OrderTable
                {
                    OrderTableId = m.OrderTableId,
                    UserId = m.UserId,
                    StartingTime = m.StartingTime,
                    IsCancel = m.IsCancel,
                    TotalPrice = m.TotalPrice,
                    TotalDeposit = m.TotalDeposit,
                    OrderDate = m.OrderDate
                }).ToListAsync();
            if (orderTables == null || orderTables.Count == 0)
                return NotFound();
            return Ok(orderTables);
        }
        // lấy thông tin đơn đặt bàn chưa thanh toán của 1 user
        [HttpGet("user/unpaid/{userid}")]
        public async Task<ActionResult<IEnumerable<DTO_OrderTable>>> GetUnPaidOrderTableByUserId(string userid)
        {
            var orderTables = await _context.OrderTables
                .Where(ot => ot.UserId == userid)
                .Where(ot => !_context.PaymentResults
                    .Any(p => p.OrderTableId == ot.OrderTableId && p.IsSuccess == true))
                .Select(m => new DTO_OrderTable
                {
                    OrderTableId = m.OrderTableId,
                    UserId = m.UserId,
                    StartingTime = m.StartingTime,
                    IsCancel = m.IsCancel,
                    TotalPrice = m.TotalPrice,
                    TotalDeposit = m.TotalDeposit,
                    OrderDate = m.OrderDate
                }).ToListAsync();
            if (orderTables == null || orderTables.Count == 0)
                return NotFound();
            return Ok(orderTables);
        }
        // lấy số lượng đơn đặt bàn đã hủy của 1 user
        [HttpGet("canceled/{id}")]
        public async Task<ActionResult<int>> GetOrderTableCanceledByUserId(string id)
        {
            var count = await _context.OrderTables
                .Where(m => m.UserId == id && m.IsCancel == true)
                .CountAsync();
            if (count == 0)
                return NoContent();
            return Ok(count);
        }
        // lấy số lượng đơn đặt bàn đã hủy
        [HttpGet("canceled")]
        public async Task<ActionResult<int>> GetAllOrderTableCanceled()
        {
            var count = await _context.OrderTables
                .Where(m => m.IsCancel == true)
                .CountAsync();
            if (count == 0)
                return NotFound();
            return Ok(count);
        }

        // api lấy ra bàn ( tableid ) có tổng doanh thu cao nhất , doanh thu cao nhất là tổng doanh thu của tất cả các đơn đặt bàn có bàn đó , 
        [HttpGet("highest-revenue")]
        public async Task<ActionResult<DTO_OrderTable>> GetOrderTableWithHighestRevenue()
        {
            var orderTable = await _context.OrderTables
                .Select(ot => new
                {
                    OrderTable = ot,
                    TotalRevenue = _context.PaymentResults
                        .Where(pr => pr.OrderTableId == ot.OrderTableId && pr.IsSuccess == true)
                        .Sum(pr => pr.Amount) ?? 0
                })
                .OrderByDescending(x => x.TotalRevenue)
                .FirstOrDefaultAsync();
            if (orderTable == null)
                return NotFound();
            return Ok(new DTO_OrderTable
            {
                OrderTableId = orderTable.OrderTable.OrderTableId,
                UserId = orderTable.OrderTable.UserId,
                StartingTime = orderTable.OrderTable.StartingTime,
                IsCancel = orderTable.OrderTable.IsCancel,
                TotalPrice = orderTable.OrderTable.TotalPrice,
                TotalDeposit = orderTable.OrderTable.TotalDeposit,
                OrderDate = orderTable.OrderTable.OrderDate
            });
        }

        // lấy ra bàn có doanh thu cao nhất , doanh thu cao nhất là tổng doanh thu của tất cả các đơn đặt bàn có bàn đó
        [HttpGet("highest-revenue-table")]
        public async Task<IActionResult> GetHighestRevenueTable()
        {
            try
            {

                var tableRevenueStats = await _context.Tables
                    .Select(table => new
                    {
                        TableId = table.TableId,
                        Capacity = table.Capacity,
                        Description = table.Description,
                        RegionId = table.RegionId,
                        RegionName = table.Region.RegionName,
                        TotalRevenue = table.OrderTablesDetails
                            .Where(otd => otd.OrderTable != null &&
                                         otd.OrderTable.TotalPrice != null &&
                                         otd.OrderTable.IsCancel != true &&
                                         otd.OrderTable.PaymentResults.Any(pr => pr.IsSuccess == true))
                            .Sum(otd => otd.OrderTable.TotalPrice ?? 0),
                        OrderCount = table.OrderTablesDetails
                            .Where(otd => otd.OrderTable != null &&
                                         otd.OrderTable.IsCancel != true &&
                                         otd.OrderTable.PaymentResults.Any(pr => pr.IsSuccess == true))
                            .Count()
                    })
                    .OrderByDescending(x => x.TotalRevenue)
                    //.Take(10)
                    .ToListAsync();
                //.ToListAsync();
                //.FirstOrDefaultAsync();

                if (tableRevenueStats == null)
                {
                    return NoContent();
                }

                return Ok(tableRevenueStats);
            }
            catch (Exception ex)
            {
                return NoContent();
            }
        }
    }
}
