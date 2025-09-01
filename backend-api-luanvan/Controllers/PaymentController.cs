using backend_api_luanvan.DataTransferObject;
using backend_api_luanvan.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace backend_api_luanvan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly Dbluanvan2Context _context;
        public PaymentController(Dbluanvan2Context context)
        {
            _context = context;
        }
        // api dùng để lấy ra thông tin thanh toán và thống kê doanh thu theo tuần , tháng , quý , năm 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO_PaymentResultInfoTotal>>> GetAllPaymentResultsWithFilter(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int? paymentMethod = null,
            [FromQuery] bool? filterBySuccess = null
        )
        {
            try
            {
                var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var query = _context.PaymentResults.AsQueryable();
                if (fromDate.HasValue)
                {
                    var fromUtc = TimeZoneInfo.ConvertTimeToUtc(fromDate.Value, vietnamTimeZone);
                    query = query.Where(p => p.Timestamp >= fromUtc);
                }
                if (toDate.HasValue)
                {
                    var toUtc = TimeZoneInfo.ConvertTimeToUtc(toDate.Value.AddDays(1).AddSeconds(-1), vietnamTimeZone);
                    query = query.Where(p => p.Timestamp <= toUtc);
                }
                if (filterBySuccess.HasValue)
                {
                    query = query.Where(p => p.IsSuccess == filterBySuccess.Value);
                }   
                if (paymentMethod.HasValue)
                {
                    query = query.Where(p => p.PaymentMethod.Contains(paymentMethod.ToString()));
                }
                var results = await query
                    .Select(pr => new DTO_PaymentResultInfoTotal
                    {
                        PaymentResultId = pr.PaymentResultId,
                        OrderTableId = pr.OrderTableId,
                        CartId = pr.CartId,
                        Amount = pr.Amount,
                        IsSuccess = pr.IsSuccess,
                        Description = pr.Description,
                        Timestamp = pr.Timestamp,
                        PaymentMethod = pr.PaymentMethod,
                        BankCode = pr.BankCode,
                        ResponseDescription = pr.ResponseDescription,
                        TransactionStatusDescription = pr.TransactionStatusDescription
                    }).ToListAsync();
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }
        //[HttpGet("paymenthistory/ordertable")]
        //public async Task<ActionResult<IEnumerable<DTO_Payment_ShowHistoryPayment_OrderTable>>> GetPaymentHistoryOrderTable()
        //{
        //    var paymentResults = await _context.PaymentResults
        //        .Where(pr => pr.OrderTableId != null)
        //        .Select(pr => new DTO_Payment_ShowHistoryPayment_OrderTable
        //        {
        //            PaymentResultId = pr.PaymentResultId,
        //            OrderTableId = pr.OrderTableId,
        //            Amount = pr.Amount,
        //            IsSuccess = pr.IsSuccess,
        //            Timestamp = pr.Timestamp,
        //            PaymentMethod = pr.PaymentMethod,
        //            BankCode = pr.BankCode,
        //            ResponseDescription = pr.ResponseDescription,
        //            TransactionStatusDescription = pr.TransactionStatusDescription
        //        }).ToListAsync();
        //    if (paymentResults == null || !paymentResults.Any())
        //        return NotFound();
        //    return paymentResults;
        //}

        [HttpGet("paymenthistory/ordertable/filter")]
        public async Task<ActionResult<IEnumerable<DTO_Payment_ShowHistoryPayment_OrderTable>>> GetOrdertablePaymentHistoryWithFilters(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] bool? filterBySuccess = null,
            [FromQuery] string? paymentMethod = null,
            [FromQuery] string? bankCode = null,
            [FromQuery] int pageSize = 50,
            [FromQuery] int pageNumber = 1)
        {
            try
            {
                var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

                var query = _context.PaymentResults.Where(p => p.OrderTableId != null);

                if (fromDate.HasValue)
                {
                    var fromUtc = TimeZoneInfo.ConvertTimeToUtc(fromDate.Value, vietnamTimeZone);
                    query = query.Where(p => p.Timestamp >= fromUtc);
                }

                if (toDate.HasValue)
                {
                    var toUtc = TimeZoneInfo.ConvertTimeToUtc(toDate.Value.AddDays(1).AddSeconds(-1), vietnamTimeZone);
                    query = query.Where(p => p.Timestamp <= toUtc);
                }

                if (filterBySuccess.HasValue)
                {
                    query = query.Where(p => p.IsSuccess == filterBySuccess.Value);
                }

                if (!string.IsNullOrEmpty(paymentMethod))
                {
                    query = query.Where(p => p.PaymentMethod.Contains(paymentMethod));
                }
                if (!string.IsNullOrEmpty(bankCode))
                {
                    query = query.Where(p => p.BankCode.Contains(bankCode));
                }
                var totalRecords = await query.CountAsync();
                var results = await query
                    .OrderByDescending(p => p.Timestamp)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new DTO_Payment_ShowHistoryPayment_OrderTable
                    {
                        PaymentResultId = p.PaymentResultId,
                        OrderTableId = p.OrderTableId,
                        Amount = p.Amount,
                        IsSuccess = p.IsSuccess ?? false,
                        Timestamp = p.Timestamp ?? DateTime.MinValue,
                        PaymentMethod = p.PaymentMethod,
                        BankCode = p.BankCode,
                        ResponseDescription = p.ResponseDescription,
                        TransactionStatusDescription = p.TransactionStatusDescription
                    })
                    .ToListAsync();

                // phân trang , nhưng đang lỗi để làm sau 
                Response.Headers.Add("X-Total-Count", totalRecords.ToString());
                Response.Headers.Add("X-Page-Number", pageNumber.ToString());
                Response.Headers.Add("X-Page-Size", pageSize.ToString());

                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }
        //[HttpGet("paymenthistory/cart")]
        //public async Task<ActionResult<IEnumerable<DTO_Payment_ShowHistoryPayment_Cart>>> GetPaymentHistoryCart()
        //{
        //    var paymentResults = await _context.PaymentResults
        //        .Where(pr => pr.CartId != null)
        //        .Select(pr => new DTO_Payment_ShowHistoryPayment_Cart
        //        {
        //            PaymentResultId = pr.PaymentResultId,
        //            CartId = pr.CartId,
        //            Amount = pr.Amount,
        //            IsSuccess = pr.IsSuccess,
        //            Timestamp = pr.Timestamp,
        //            PaymentMethod = pr.PaymentMethod,
        //            BankCode = pr.BankCode,
        //            ResponseDescription = pr.ResponseDescription,
        //            TransactionStatusDescription = pr.TransactionStatusDescription
        //        }).ToListAsync();
        //    if (paymentResults == null || !paymentResults.Any())
        //        return NotFound();
        //    return paymentResults;
        //}

        [HttpGet("paymenthistory/cart/filter")]
        public async Task<ActionResult<IEnumerable<DTO_Payment_ShowHistoryPayment_Cart>>> GetPaymentHistoryWithFilters(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] bool? filterBySuccess = null,
            [FromQuery] string? paymentMethod = null,
            [FromQuery] string? bankCode = null,
            [FromQuery] int pageSize = 50,
            [FromQuery] int pageNumber = 1)
        {
            try
            {
                var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

                var query = _context.PaymentResults.Where(p => p.CartId != null);

                if (fromDate.HasValue)
                {
                    var fromUtc = TimeZoneInfo.ConvertTimeToUtc(fromDate.Value, vietnamTimeZone);
                    query = query.Where(p => p.Timestamp >= fromUtc);
                }

                if (toDate.HasValue)
                {
                    var toUtc = TimeZoneInfo.ConvertTimeToUtc(toDate.Value.AddDays(1).AddSeconds(-1), vietnamTimeZone);
                    query = query.Where(p => p.Timestamp <= toUtc);
                }

                if (filterBySuccess.HasValue)
                {
                    query = query.Where(p => p.IsSuccess == filterBySuccess.Value);
                }

                if (!string.IsNullOrEmpty(paymentMethod))
                {
                    query = query.Where(p => p.PaymentMethod.Contains(paymentMethod));
                }
                if (!string.IsNullOrEmpty(bankCode))
                {
                    query = query.Where(p => p.BankCode.Contains(bankCode));
                }
                var totalRecords = await query.CountAsync();
                var results = await query
                    .OrderByDescending(p => p.Timestamp)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new DTO_Payment_ShowHistoryPayment_Cart
                    {
                        PaymentResultId = p.PaymentResultId,
                        CartId = p.CartId,
                        Amount = p.Amount,
                        IsSuccess = p.IsSuccess ?? false,
                        Timestamp = p.Timestamp ?? DateTime.MinValue,
                        PaymentMethod = p.PaymentMethod,
                        BankCode = p.BankCode,
                        ResponseDescription = p.ResponseDescription,
                        TransactionStatusDescription = p.TransactionStatusDescription
                    })
                    .ToListAsync();

                // phân trang , nhưng đang lỗi để làm sau 
                Response.Headers.Add("X-Total-Count", totalRecords.ToString());
                Response.Headers.Add("X-Page-Number", pageNumber.ToString());
                Response.Headers.Add("X-Page-Size", pageSize.ToString());

                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }

        // api thống kê số liệu theo filter
        [HttpGet("paymenthistory/cart/statistics")]
        public async Task<ActionResult<object>> GetPaymentStatistics(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] bool? filterBySuccess = null)
        {
            try
            {
                var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

                var query = _context.PaymentResults.Where(p => p.CartId != null);

                if (fromDate.HasValue)
                {
                    var fromUtc = TimeZoneInfo.ConvertTimeToUtc(fromDate.Value, vietnamTimeZone);
                    query = query.Where(p => p.Timestamp >= fromUtc);
                }

                if (toDate.HasValue)
                {
                    var toUtc = TimeZoneInfo.ConvertTimeToUtc(toDate.Value.AddDays(1).AddSeconds(-1), vietnamTimeZone);
                    query = query.Where(p => p.Timestamp <= toUtc);
                }

                if (filterBySuccess.HasValue)
                {
                    query = query.Where(p => p.IsSuccess == filterBySuccess.Value);
                }

                var statistics = await query
                    .GroupBy(p => 1)
                    .Select(g => new
                    {
                        TotalTransactions = g.Count(),
                        TotalAmount = g.Sum(p => p.Amount ?? 0),
                        SuccessCount = g.Count(p => p.IsSuccess == true),
                        FailCount = g.Count(p => p.IsSuccess == false),
                        SuccessRate = g.Count() > 0 ? (double)g.Count(p => p.IsSuccess == true) / g.Count() * 100 : 0,
                        SuccessAmount = g.Where(p => p.IsSuccess == true).Sum(p => p.Amount ?? 0),
                        PaymentMethods = g.GroupBy(p => p.PaymentMethod)
                            .Select(pm => new { Method = pm.Key, Count = pm.Count() })
                            .OrderByDescending(pm => pm.Count),
                        BankCodes = g.GroupBy(p => p.BankCode)
                            .Select(bc => new { Bank = bc.Key, Count = bc.Count() })
                            .OrderByDescending(bc => bc.Count)
                    })
                    .FirstOrDefaultAsync();

                if (statistics == null)
                {
                    return Ok(new
                    {
                        TotalTransactions = 0,
                        TotalAmount = 0,
                        SuccessCount = 0,
                        FailCount = 0,
                        SuccessRate = 0,
                        SuccessAmount = 0,
                        PaymentMethods = new List<object>(),
                        BankCodes = new List<object>()
                    });
                }

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }

        [HttpGet("ordertable/{id}")]
        public async Task<ActionResult<IEnumerable<DTO_Payment>>> GetPaymentResultsByOrderTableId(long id)
        {
            var paymentResults = await _context.PaymentResults
                .Where(pr => pr.OrderTableId == id)
                .Select(pr => new DTO_Payment
                {
                    PaymentResultId = pr.PaymentResultId,
                    OrderTableId = pr.OrderTableId,
                    CartId = pr.CartId,
                    Amount = pr.Amount,
                    PaymentId = pr.PaymentId,
                    IsSuccess = pr.IsSuccess,
                    Description = pr.Description,
                    Timestamp = pr.Timestamp,
                    VnpayTransactionId = pr.VnpayTransactionId,
                    PaymentMethod = pr.PaymentMethod,
                    BankCode = pr.BankCode,
                    BankTransactionId = pr.BankTransactionId,
                    ResponseDescription = pr.ResponseDescription,
                    TransactionStatusDescription = pr.TransactionStatusDescription
                }).ToListAsync();
            if (paymentResults == null || !paymentResults.Any())
                return NotFound();

            return paymentResults;
        }
        [HttpGet("ordertable/juststatus/{id}")]
        public async Task<ActionResult<bool>> GetPaymentJustStatusByOrderTableId(long id)
        {
            var isPaid = await _context.PaymentResults
                .AnyAsync(p => p.OrderTableId == id && p.IsSuccess == true);

            return Ok(isPaid); // true nếu đã thanh toán, false nếu chưa
        }


        [HttpGet("ordertable/status/{id}")]
        public async Task<ActionResult<IEnumerable<DTO_PaymentStatusOrderTable>>> GetPaymentStatusByOrderTableId(long id)
        {
            var paymentStatuses = await _context.PaymentResults
                .Where(pr => pr.OrderTableId == id)
                .Select(pr => new DTO_PaymentStatusOrderTable
                {
                    OrderTableId = pr.OrderTableId,
                    IsSuccess = pr.IsSuccess
                }).ToListAsync();

            if (paymentStatuses == null || !paymentStatuses.Any())
                return NotFound();

            return paymentStatuses;
        }
        [HttpGet("cart/status/{id}")]
        public async Task<ActionResult<IEnumerable<DTO_PaymentStatusCart>>> GetPaymentStatusByCartId(long id)
        {
            var paymentStatuses = await _context.PaymentResults
                .Where(pr => pr.CartId == id)
                .Select(pr => new DTO_PaymentStatusCart
                {
                    CartId = pr.CartId,
                    IsSuccess = pr.IsSuccess
                }).ToListAsync();

            if (paymentStatuses == null || !paymentStatuses.Any())
                return NotFound();

            return paymentStatuses;
        }
        [HttpPost]
        public async Task<ActionResult<DTO_Payment>> CreatePaymentResult(DTO_Payment paymentDto)
        {
            var paymentResult = new PaymentResult
            {
                OrderTableId = paymentDto.OrderTableId,
                CartId = paymentDto.CartId,
                Amount = paymentDto.Amount,
                PaymentId = paymentDto.PaymentId,
                IsSuccess = paymentDto.IsSuccess,
                Description = paymentDto.Description,
                Timestamp = paymentDto.Timestamp,
                VnpayTransactionId = paymentDto.VnpayTransactionId,
                PaymentMethod = paymentDto.PaymentMethod,
                BankCode = paymentDto.BankCode,
                BankTransactionId = paymentDto.BankTransactionId,
                ResponseDescription = paymentDto.ResponseDescription,
                TransactionStatusDescription = paymentDto.TransactionStatusDescription
            };
            _context.PaymentResults.Add(paymentResult);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("admin/ordertable/{ordertableid}")]
        public async Task<ActionResult<DTO_Payment_OrderTable>> CreatePaymentResultForOrderTable(long ordertableid, DTO_Payment_OrderTable paymentDto)
        {
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            var paymentid = DateTime.Now.Ticks;
            var orderTable = await _context.OrderTables.FindAsync(ordertableid);
            if (orderTable == null)
            {
                return NotFound("Order table not found.");
            }
            var paymentResult = new PaymentResult
            {
                OrderTableId = ordertableid,
                Amount = paymentDto.Amount,
                PaymentId = paymentid,
                IsSuccess = true,
                Description = $"Thanh toán tiền mặt cho đơn đặt bàn : {ordertableid}",
                Timestamp = currentTime,
                PaymentMethod = "Cash"
            };
            _context.PaymentResults.Add(paymentResult);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("admin/cart/{cartid}")]
        public async Task<ActionResult<DTO_Payment_Cart>> CreatePaymentResultForCart(long cartid, DTO_Payment_Cart paymentDto)
        {
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            var paymentid = DateTime.Now.Ticks;
            var cart = await _context.Carts.FindAsync(cartid);
            if (cart == null)
            {
                return NotFound("Cart not found.");
            }
            var paymentResult = new PaymentResult
            {
                CartId = cartid,
                Amount = paymentDto.Amount,
                PaymentId = paymentid,
                IsSuccess = true,
                Description = $"Thanh toán tiền mặt cho đơn hàng : {cartid}",
                Timestamp = currentTime,
                PaymentMethod = "Cash"
            };
            _context.PaymentResults.Add(paymentResult);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
