using backend_api_luanvan.DataTransferObject;
using backend_api_luanvan.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_api_luanvan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Dbluanvan2Context _context;
        public UserController(Dbluanvan2Context context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DTO_User>> GetUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("this user is not exists");
            }
            return new DTO_User
            {
                UserId = user.UserId,
                UPassword = user.UPassword,
                CustomerName = user.CustomerName,
                RolesId = user.RolesId,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Address = user.Address,
                CreateAt = user.CreateAt
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<DTO_User>> Login([FromBody] DTO_User loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.UserId) || string.IsNullOrEmpty(loginDto.UPassword))
            {
                return BadRequest("UserId and UPassword are required.");
            }

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == loginDto.UserId && u.UPassword == loginDto.UPassword);

            if (existingUser == null)
            {
                return Unauthorized("UserId or UPassword is invalid.");
            }

            var dtoUser = new DTO_User
            {
                //UserId = existingUser.UserId,
                //UPassword = null, // không trả về password!
                //CustomerName = existingUser.CustomerName,
                RolesId = existingUser.RolesId,
                //PhoneNumber = existingUser.PhoneNumber,
                //Email = existingUser.Email,
                //Address = existingUser.Address,
                //CreateAt = existingUser.CreateAt
            };
            return Ok(dtoUser);
        }


        // sign up
        [HttpPost("signup/admin")]
        public async Task<ActionResult<DTO_User>> CreateUserAdmin([FromBody] DTO_User user)
        {
            if (user == null)
                return BadRequest("User data is null.");

            if (string.IsNullOrEmpty(user.UserId) || string.IsNullOrEmpty(user.UPassword)
                || string.IsNullOrEmpty(user.PhoneNumber) || string.IsNullOrEmpty(user.Email))
            {
                return BadRequest("UserId, UPassword, PhoneNumber, Email are required.");
            }

            if (await _context.Users.AnyAsync(u => u.UserId == user.UserId))
                return Conflict("UserId already exists.");

            if (await _context.Users.AnyAsync(u => u.PhoneNumber == user.PhoneNumber))
                return Conflict("PhoneNumber already exists.");

            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                return Conflict("Email already exists.");
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var newUser = new User
            {
                UserId = user.UserId,
                UPassword = user.UPassword,
                CustomerName = user.CustomerName,
                RolesId = 1,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Address = user.Address,
                CreateAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone)
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // map lại cái dữ liệu vào dto để trả về đúng dữ liệu mong muốn
            var userDto = new DTO_User
            {
                UserId = newUser.UserId,
                UPassword = newUser.UPassword,
                CustomerName = newUser.CustomerName,
                RolesId = newUser.RolesId,
                PhoneNumber = newUser.PhoneNumber,
                Email = newUser.Email,
                Address = newUser.Address,
                CreateAt = newUser.CreateAt
            };

            return CreatedAtAction(nameof(GetUser), new { id = userDto.UserId }, userDto);
        }


        [HttpPost("signup")]
        public async Task<ActionResult<DTO_User>> CreateUser([FromBody] DTO_User user)
        {
            if (user == null)
                return BadRequest("User data is null.");

            if (string.IsNullOrEmpty(user.UserId) || string.IsNullOrEmpty(user.UPassword)
                || string.IsNullOrEmpty(user.PhoneNumber) || string.IsNullOrEmpty(user.Email))
            {
                return BadRequest("UserId, UPassword, PhoneNumber, Email are required.");
            }

            if (await _context.Users.AnyAsync(u => u.UserId == user.UserId))
                return Conflict("UserId already exists.");

            if (await _context.Users.AnyAsync(u => u.PhoneNumber == user.PhoneNumber))
                return Conflict("PhoneNumber already exists.");

            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                return Conflict("Email already exists.");
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var newUser = new User
            {
                UserId = user.UserId,
                UPassword = user.UPassword,
                CustomerName = user.CustomerName,
                RolesId = 0,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Address = user.Address,
                CreateAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone)
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // map lại cái dữ liệu vào dto để trả về đúng dữ liệu mong muốn
            var userDto = new DTO_User
            {
                UserId = newUser.UserId,
                UPassword = newUser.UPassword,
                CustomerName = newUser.CustomerName,
                RolesId = newUser.RolesId,
                PhoneNumber = newUser.PhoneNumber,
                Email = newUser.Email,
                Address = newUser.Address,
                CreateAt = newUser.CreateAt
            };

            return CreatedAtAction(nameof(GetUser), new { id = userDto.UserId }, userDto);
        }



        [HttpPost("signup/guest")]
        public async Task<ActionResult<DTO_User_Guest>> CreateUserGuest([FromBody] DTO_User_Guest user)
        {
            if (user == null)
                return BadRequest("User data is null.");

            if (string.IsNullOrEmpty(user.UserId))
            {
                return BadRequest("UserId is required.");
            }
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            var newUser = new User
            {
                UserId = user.UserId,
                UPassword = "",
                CustomerName = user.CustomerName,
                RolesId = 0,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email ?? "",
                Address = user.Address ?? "",
                CreateAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone)
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();


            // map lại cái dữ liệu vào dto để trả về đúng dữ liệu mong muốn
            var userDto = new DTO_User_Guest
            {
                UserId = newUser.UserId,
                CustomerName = newUser.CustomerName,
                RolesId = newUser.RolesId,
                PhoneNumber = newUser.PhoneNumber,
                CreateAt = newUser.CreateAt
            };

            return CreatedAtAction(nameof(GetUser), new { id = userDto.UserId }, userDto);
        }

        [HttpPut("modify/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] DTO_UpdateUser user)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound("User not found.");
            }

            // Kiểm tra trùng (trừ chính bản thân user)
            if (await _context.Users.AnyAsync(u => u.UserId != id && u.PhoneNumber == user.PhoneNumber))
                return Conflict("PhoneNumber already exists.");

            if (await _context.Users.AnyAsync(u => u.UserId != id && u.Email == user.Email))
                return Conflict("Email already exists.");

            // Cập nhật
            existingUser.UPassword = user.UPassword;
            existingUser.CustomerName = user.CustomerName;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.Email = user.Email;
            existingUser.Address = user.Address;

            await _context.SaveChangesAsync();

            var userUpdateDTO = new DTO_UpdateUser
            {
                UPassword = existingUser.UPassword,
                CustomerName = existingUser.CustomerName,
                PhoneNumber = existingUser.PhoneNumber,
                Email = existingUser.Email,
                Address = existingUser.Address,
            };

            return Ok(userUpdateDTO);
        }
    }
}