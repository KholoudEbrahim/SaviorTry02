using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Persistence;
using Services;
using Shared.UserDTOs;
using Microsoft.Extensions.Logging;
namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly SaviorDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticationController> _logger;
        public AuthenticationController(SaviorDbContext context, IConfiguration configuration, ILogger<AuthenticationController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("signup")]
        public async Task<ActionResult> SignUp(RegisterDto dto)
        {
            if (dto == null)
                return BadRequest("User data is required.");

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingUser != null)
                return Conflict("Email is already registered.");

            var user = new User
            {
                Fname = dto.FirstName,
                Lname = dto.LastName,
                Email = dto.Email,
                Password = dto.Password,
                Phone = dto.Phone,
                ConfirmPassword = dto.ConfirmPassword,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(SignUp), new { id = user.Id }, user);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] Login login)
        {
            if (login == null)
                return BadRequest("Email and Password are required.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == login.Email);

            if (user == null || user.Password != login.Password)
                return Unauthorized("Invalid credentials.");

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                Message = "Login successful",
                Token = token
            });
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(72),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, [FromServices] EmailService emailService)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return NotFound("Email not found.");

            var code = new Random().Next(100000, 999999).ToString();

            var subject = "Password Reset Code";
            var body = $"Your password reset code is: {code}";

            await emailService.SendEmailAsync(request.Email, subject, body);

            user.ResetCode = code;
            user.ResetCodeExpiry = DateTime.UtcNow.AddMinutes(5);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok("A code has been sent to your email.");
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPassword model)
        {
            if (model.NewPassword != model.ConfirmNewPassword)
                return BadRequest("New password and confirm password do not match.");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.ResetCode != null && u.ResetCodeExpiry > DateTime.UtcNow);

            if (user == null)
                return NotFound("Invalid or expired reset code.");

            user.Password = model.NewPassword;
            user.ResetCode = null;
            user.ResetCodeExpiry = null;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok("Password has been successfully reset.");
        }

        [HttpPost("sync")]
        public async Task<IActionResult> SyncUser([FromBody] ExternalUserDto userDto)
        {
            if (userDto == null)
            {
                _logger.LogWarning("SyncUser called with null userDto");
                return BadRequest("User data is required.");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {

                    var existingUser = await _context.Users.FindAsync(userDto.Id);
                    if (existingUser != null)
                    {
                        _logger.LogWarning($"User with ID {userDto.Id} already exists");
                        return Conflict("User already exists");
                    }


                    await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Users ON");
                    _logger.LogInformation("IDENTITY_INSERT enabled for Users table");

                    var user = new User
                    {
                        Id = userDto.Id,
                        Fname = userDto.Fname,
                        Lname = userDto.Lname,
                        Email = userDto.Email,
                        Phone = userDto.Phone,
                        Password = userDto.Password
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"User with ID {userDto.Id} synced successfully");

               
                    await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Users OFF");
                    _logger.LogInformation("IDENTITY_INSERT disabled for Users table");

                    await transaction.CommitAsync();
                    return Ok(new { Message = "User synced successfully", UserId = userDto.Id });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, $"Error syncing user with ID {userDto?.Id}");
                    return StatusCode(500, new
                    {
                        Message = "Internal server error",
                        Details = ex.Message,
                        StackTrace = ex.StackTrace
                    });
                }
            }
        }
    }
}