using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using pasto_backend.Data;
using pasto_backend.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace pasto_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public UserController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        private string HashPassword(string password, byte[] salt)
        {
            // Derive a key from the password and salt using PBKDF2
            string hashed = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));

            return hashed;
        }

        private string GetLoggedInUserEmail()
        {
            // Retrieve the email from the User.Identity.Name property
            return User.Identity.Name;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            // Check if the provided email or username already exists
            bool emailExists = _context.Users.Any(u => u.Email == user.Email);
            bool usernameExists = _context.Users.Any(u => u.Username == user.Username);

            if (emailExists)
            {
                return BadRequest(new { response = "Email already exists" });
            }

            if (usernameExists)
            {
                return BadRequest(new { response = "Username already exists" });
            }

            // Generate a random salt
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Hash the user's password using the generated salt
            string hashedPassword = HashPassword(user.PasswordHash, salt);
            user.PasswordHash = hashedPassword;
            user.Salt = Convert.ToBase64String(salt);

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { response = "User registered successfully" });
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            // Authenticate the user based on the provided email
            var user = _context.Users.SingleOrDefault(u => u.Email == loginModel.Email);
            if (user == null)
            {
                // Return an unauthorized response if user doesn't exist
                return Unauthorized();
            }

            // Extract the stored salt and hash the provided password using the same salt
            var storedSalt = Convert.FromBase64String(user.Salt);
            var computedHash = HashPassword(loginModel.Password, storedSalt);

            // Compare the computed hash with the stored hashed password
            if (computedHash == user.PasswordHash)
            {
                // Generate a JWT token for the authenticated user
                var token = GenerateJwtToken(user.Email);
                return Ok(new { token });
            }
            else
            {
                // Return an unauthorized response if password verification fails
                return Unauthorized();
            }
        }

        private string GenerateJwtToken(string userEmail)
        {
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, userEmail)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpGet("check-email")]
        public IActionResult CheckEmailExists(string email)
        {
            bool emailExists = _context.Users.Any(u => u.Email == email);
            return Ok(new { isNewUser = !emailExists });
        }

        [HttpPost("change-password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordModel changePasswordModel)
        {
            // Get the logged-in user's email from the token
            string userEmail = GetLoggedInUserEmail();

            // Check if the email from the token matches the user's email
            if (userEmail != changePasswordModel.Email)
            {
                return Unauthorized(new { response = "Unauthorized" });
            }

            // Find the user by email
            User user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
            {
                return NotFound(new { response = "User not found" });
            }

            // Rest of your code for changing password...

            // Add a return statement for the successful password change
            return Ok(new { response = "Password changed successfully" });
        }

        [HttpDelete("delete-account")]
        public IActionResult DeleteAccount([FromBody] User userToDelete)
        {
            // Get the logged-in user's email from the token
            string userEmail = GetLoggedInUserEmail();

            // Check if the email from the token matches the user's email
            if (userEmail != userToDelete.Email)
            {
                return Unauthorized(new { response = "Unauthorized" });
            }

            var user = _context.Users.SingleOrDefault(u => u.Email == userEmail);
            if (user == null)
            {
                return NotFound(new { response = "User not found" });
            }

            // Rest of your code for deleting the account...

            // Delete user account and associated data
            _context.Users.Remove(user);
            // ... delete associated data ...

            _context.SaveChanges();

            return Ok(new { response = "Account deleted successfully" });
        }


    }
}
