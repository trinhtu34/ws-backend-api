using backend_api_luanvan.DataTransferObject;
using backend_api_luanvan.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace backend_api_luanvan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactFormController : ControllerBase
    {
        private readonly Dbluanvan2Context _context;
        public ContactFormController(Dbluanvan2Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO_ContactForm>>> GetAllContactForms()
        {
            return await _context.ContactForms
                .Select(m => new DTO_ContactForm
                {
                    ContactId = m.ContactId,
                    UserId = m.UserId,
                    Content = m.Content,
                    CreateAt = m.CreateAt
                }).ToListAsync();
        }

        [HttpGet("contactid/{contactid}")]
        public async Task<ActionResult<IEnumerable<DTO_ContactForm>>> GetAllContactFormsById(string contactid)
        {
            var contactForms = await _context.ContactForms
                .Where(m => m.ContactId.ToString() == contactid)
                .Select(m => new DTO_ContactForm
                {
                    ContactId = m.ContactId,
                    UserId = m.UserId,
                    Content = m.Content,
                    CreateAt = m.CreateAt
                }).ToListAsync();
            if (contactForms == null || !contactForms.Any())
            {
                return NotFound("No contact forms found with the specified ID.");
            }
            return Ok(contactForms);
        }

        [HttpGet("{userid}")]
        public async Task<ActionResult<IEnumerable<DTO_ContactForm>>> GetAllContactFormsByUserId(string userid)
        {
            var contactForms = await _context.ContactForms
                .Where(m => m.UserId == userid)
                .Select(m => new DTO_ContactForm
                {
                    ContactId = m.ContactId,
                    UserId = m.UserId,
                    Content = m.Content,
                    CreateAt = m.CreateAt
                }).ToListAsync();
            if (contactForms == null || !contactForms.Any())
            {
                return NotFound("No contact forms found for the specified user.");
            }
            return Ok(contactForms);
        }
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<DTO_ContactForm>>> GetContactFormWithFilters(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string? userId = null,
            [FromQuery] string? contentSearch = null,
            [FromQuery] int pageSize = 50,
            [FromQuery] int pageNumber = 1)
        {
            try
            {
                var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

                // Giả sử bạn có DbSet<ContactForm> trong _context
                var query = _context.ContactForms.AsQueryable();

                // Lọc theo ngày tạo (fromDate)
                if (fromDate.HasValue)
                {
                    var fromUtc = TimeZoneInfo.ConvertTimeToUtc(fromDate.Value, vietnamTimeZone);
                    query = query.Where(c => c.CreateAt >= fromUtc);
                }

                // Lọc theo ngày tạo (toDate)
                if (toDate.HasValue)
                {
                    var toUtc = TimeZoneInfo.ConvertTimeToUtc(toDate.Value.AddDays(1).AddSeconds(-1), vietnamTimeZone);
                    query = query.Where(c => c.CreateAt <= toUtc);
                }

                // Lọc theo UserId
                if (!string.IsNullOrEmpty(userId))
                {
                    query = query.Where(c => c.UserId.Contains(userId));
                }

                // Lọc theo nội dung (search trong Content)
                if (!string.IsNullOrEmpty(contentSearch))
                {
                    query = query.Where(c => c.Content.Contains(contentSearch));
                }

                // Đếm tổng số records
                var totalRecords = await query.CountAsync();

                // Lấy dữ liệu với phân trang
                var results = await query
                    .OrderByDescending(c => c.CreateAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c => new DTO_ContactForm
                    {
                        ContactId = c.ContactId,
                        UserId = c.UserId,
                        Content = c.Content,
                        CreateAt = c.CreateAt
                    })
                    .ToListAsync();

                // Thêm thông tin phân trang vào header
                Response.Headers.Add("X-Total-Count", totalRecords.ToString());
                Response.Headers.Add("X-Page-Number", pageNumber.ToString());
                Response.Headers.Add("X-Page-Size", pageSize.ToString());
                Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)totalRecords / pageSize)).ToString());

                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }

        [HttpGet("filter/sub")]
        public async Task<ActionResult<IEnumerable<DTO_ContactForm>>> GetContactFormWithFilters(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string? userId = null,
            [FromQuery] string? contentSearch = null,
            [FromQuery] string? searchType = "contains", // contains, exact, startswith, endswith
            [FromQuery] bool? caseSensitive = false,
            [FromQuery] string? sortBy = "createAt", // createAt, userId, content
            [FromQuery] string? sortOrder = "desc", // asc, desc
            [FromQuery] int pageSize = 50,
            [FromQuery] int pageNumber = 1)
        {
            try
            {
                var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

                // Giả sử bạn có DbSet<ContactForm> trong _context
                var query = _context.ContactForms.AsQueryable();

                // Lọc theo ngày tạo (fromDate)
                if (fromDate.HasValue)
                {
                    var fromUtc = TimeZoneInfo.ConvertTimeToUtc(fromDate.Value, vietnamTimeZone);
                    query = query.Where(c => c.CreateAt >= fromUtc);
                }

                // Lọc theo ngày tạo (toDate)
                if (toDate.HasValue)
                {
                    var toUtc = TimeZoneInfo.ConvertTimeToUtc(toDate.Value.AddDays(1).AddSeconds(-1), vietnamTimeZone);
                    query = query.Where(c => c.CreateAt <= toUtc);
                }

                // Lọc theo UserId với tùy chọn tìm kiếm
                if (!string.IsNullOrEmpty(userId))
                {
                    query = ApplyStringFilter(query, c => c.UserId, userId, searchType, caseSensitive);
                }

                // Lọc theo nội dung với tùy chọn tìm kiếm
                if (!string.IsNullOrEmpty(contentSearch))
                {
                    query = ApplyStringFilter(query, c => c.Content, contentSearch, searchType, caseSensitive);
                }

                // Đếm tổng số records
                var totalRecords = await query.CountAsync();

                // Áp dụng sắp xếp
                query = ApplySorting(query, sortBy, sortOrder);

                // Lấy dữ liệu với phân trang
                var results = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c => new DTO_ContactForm
                    {
                        ContactId = c.ContactId,
                        UserId = c.UserId,
                        Content = c.Content,
                        CreateAt = c.CreateAt
                    })
                    .ToListAsync();

                // Thêm thông tin phân trang vào header
                Response.Headers.Add("X-Total-Count", totalRecords.ToString());
                Response.Headers.Add("X-Page-Number", pageNumber.ToString());
                Response.Headers.Add("X-Page-Size", pageSize.ToString());
                Response.Headers.Add("X-Total-Pages", ((int)Math.Ceiling((double)totalRecords / pageSize)).ToString());

                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }

        // Helper method để áp dụng các loại tìm kiếm string
        private IQueryable<ContactForm> ApplyStringFilter(IQueryable<ContactForm> query,
            Expression<Func<ContactForm, string>> propertySelector,
            string searchValue,
            string searchType,
            bool? caseSensitive)
        {
            if (string.IsNullOrEmpty(searchValue)) return query;

            // Xử lý case sensitive
            var searchTerm = caseSensitive == true ? searchValue : searchValue.ToLower();

            switch (searchType?.ToLower())
            {
                case "exact":
                    return caseSensitive == true
                        ? query.Where(c => propertySelector.Compile()(c) == searchTerm)
                        : query.Where(c => propertySelector.Compile()(c).ToLower() == searchTerm);

                case "startswith":
                    return caseSensitive == true
                        ? query.Where(c => propertySelector.Compile()(c).StartsWith(searchTerm))
                        : query.Where(c => propertySelector.Compile()(c).ToLower().StartsWith(searchTerm));

                case "endswith":
                    return caseSensitive == true
                        ? query.Where(c => propertySelector.Compile()(c).EndsWith(searchTerm))
                        : query.Where(c => propertySelector.Compile()(c).ToLower().EndsWith(searchTerm));

                case "contains":
                default:
                    return caseSensitive == true
                        ? query.Where(c => propertySelector.Compile()(c).Contains(searchTerm))
                        : query.Where(c => propertySelector.Compile()(c).ToLower().Contains(searchTerm));
            }
        }

        // Helper method để áp dụng sắp xếp
        private IQueryable<ContactForm> ApplySorting(IQueryable<ContactForm> query, string sortBy, string sortOrder)
        {
            var isDescending = sortOrder?.ToLower() == "desc";

            switch (sortBy?.ToLower())
            {
                case "userid":
                    return isDescending ? query.OrderByDescending(c => c.UserId) : query.OrderBy(c => c.UserId);

                case "content":
                    return isDescending ? query.OrderByDescending(c => c.Content) : query.OrderBy(c => c.Content);

                case "createat":
                default:
                    return isDescending ? query.OrderByDescending(c => c.CreateAt) : query.OrderBy(c => c.CreateAt);
            }
        }
        [HttpPost]
        public async Task<ActionResult<DTO_ContactForm>> CreateContactForm([FromBody] DTO_ContactForm dto)
        {
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            var contactForm = new ContactForm
            {
                UserId = dto.UserId,
                Content = dto.Content,
                CreateAt = currentTime
            };
            _context.ContactForms.Add(contactForm);
            await _context.SaveChangesAsync();
            dto.ContactId = contactForm.ContactId;
            dto.CreateAt = contactForm.CreateAt;
            return NoContent();
        }
    }
}
