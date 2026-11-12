
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RaceDayAPI.Data;
using RaceDayAPI.Models;
using RaceDayAPI.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly RaceDayContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(
            RaceDayContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (request.Role != "Organiser" &&
                request.Role != "Participant")
            {
                return BadRequest("Invalid role.");
            }

            if (await _context.Users.AnyAsync(
                u => u.Email == request.Email))
            {
                return BadRequest("Email already exists.");
            }

            int nextUserID = (await _context.Users
                .MaxAsync(u => (int?)u.UserID) ?? 0) + 1;

            var user = new User
            {
                UserID = nextUserID,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                Role = request.Role,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Registration successful.");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(
                u => u.Email == request.Email);

            if (user == null)
            {
                return Unauthorized("User email was not found.");
            }

            bool passwordValid;

            if (user.PasswordHash.StartsWith("HASH_"))
            {
                passwordValid =
                    user.PasswordHash == "HASH_" + request.Password;

                if (passwordValid)
                {
                    user.PasswordHash = HashPassword(request.Password);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                passwordValid = VerifyPassword(
                    request.Password,
                    user.PasswordHash);
            }

            if (!passwordValid)
            {
                return Unauthorized("Password does not match.");
            }

            var token = GenerateToken(user);

            return Ok(new
            {
                token = token,
                userID = user.UserID,
                firstName = user.FirstName,
                lastName = user.LastName,
                email = user.Email,
                role = user.Role
            });
        }

        private string GenerateToken(User user)
        {
            var key = _configuration["Jwt:Key"]!;

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.UserID.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    user.Email),

                new Claim(
                    ClaimTypes.Role,
                    user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                100000,
                HashAlgorithmName.SHA256,
                32);

            return Convert.ToBase64String(salt) + "." +
                   Convert.ToBase64String(hash);
        }

        private static bool VerifyPassword(
            string password,
            string storedHash)
        {
            var parts = storedHash.Split('.');

            if (parts.Length != 2)
            {
                return false;
            }

            try
            {
                byte[] salt = Convert.FromBase64String(parts[0]);
                byte[] stored = Convert.FromBase64String(parts[1]);

                byte[] calculated = Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    salt,
                    100000,
                    HashAlgorithmName.SHA256,
                    32);

                return CryptographicOperations.FixedTimeEquals(
                    calculated,
                    stored);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}


