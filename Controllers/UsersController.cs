using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureApp.Model;
using SecureChat.Data;

namespace SecureApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require JWT for all actions
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            // Return only non-sensitive fields
            try
            {
                var users = await _context.Users
                    .Select(u => new
                    {
                        u.Id,
                        u.Username,
                        u.Email
                    })
                    .ToListAsync();
                Console.WriteLine("Fetched users: " + users.Count);
                return Ok(users);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching users: " + ex.Message);
                return StatusCode(500, "Internal server error");
                
            }
            
        }

        // GET: api/users/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new 
                {
                    u.Id,
                    u.Username,
                    u.Email
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // POST: api/users
        [HttpPost]
        [AllowAnonymous] // Allow registration without JWT
        public async Task<IActionResult> CreateUser(User user)
        {
            // Hash password before saving
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new 
            { 
                user.Id,
                user.Username,
                user.Email
            });
        }
    }
}
