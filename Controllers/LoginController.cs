using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VeterinariaSaep.Data;
using VeterinariaSaep.Models;
using VeterinariaSaep.Models.DTOs;

namespace VeterinariaSaep.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        public LoginController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost("register")]
        public async Task<ActionResult> CriarUser(Login login)
        {
            var novoUser = new Login
            {
                Email = login.Email,
                Senha = login.Senha,
                NomeUser = login.NomeUser
            };
            _context.logins.Add(novoUser);
            await _context.SaveChangesAsync();
            return Ok(novoUser);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            var user = await _context.logins.FirstOrDefaultAsync(u => u.Email == login.Email);

            if (user != null && user.Senha == login.Senha)
            {
                var token = CriarToken(user);
                return Ok(new
                {
                    NomeUser = user.NomeUser,
                    token
                });
            }
            return Unauthorized(new { Mensagem = "Credenciais inválidas" });
        }
        private string CriarToken(Login login)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, login.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("minha-chave-super-segura-com-32-caracteres"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: creds,
                expires: DateTime.Now.AddHours(2)
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}