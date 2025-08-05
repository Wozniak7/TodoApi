using Microsoft.AspNetCore.Mvc;
using TodoApi.Data;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TodoContext _context;
        private readonly IConfiguration _config;

        public AuthController(TodoContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            if (_context.Users.Any(u => u.Username == user.Username))
                return BadRequest("Usuário já existe");

            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok("Usuário registrado");
        }

        [HttpPost("login")]
        public IActionResult Login(User login)
        {
            var user = _context.Users.FirstOrDefault(u =>
                u.Username == login.Username && u.Password == login.Password);

            if (user == null) return Unauthorized("Usuário ou senha inválidos");

            var token = TokenService.GenerateToken(user, _config["Jwt:Key"]);
            return Ok(new { token });
        }
    }
}
