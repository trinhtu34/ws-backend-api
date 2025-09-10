using backend_api_luanvan.Models;
using backend_backend_api_luanvan.DataTransferObject;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend_backend_api_luanvan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly Dbluanvan2Context _context;
        public RoleController(Dbluanvan2Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTO_Roles>>> GetRole()
        {
            return Ok(_context.Roles.ToList());
        }

    }
}
