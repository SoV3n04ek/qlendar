using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using QlendarBackend.Qlendar.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QlendarBackend.Qlendar.API
{
    public class AuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<AuthResult> RegisterAsync(string email, string password, string? nickname = null)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new[] { "Email already exists" }
                };
            }

            var newUser = new User { Email = email, UserName = email, Nickname = nickname };
            var createdUser = await _userManager.CreateAsync(newUser, password);

            if (!createdUser.Succeeded)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = createdUser.Errors.Select(x => x.Description)
                };
            }

            return GenerateJwtToken(newUser);
        }

        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = ["Invalid credentials"]
                };
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordValid)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = ["Invalid credentials"]
                };
            }

            return GenerateJwtToken(user);
        }

        private AuthResult GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("nickname", user.Nickname ?? string.Empty)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(
                Convert.ToDouble(_configuration["Jwt:ExpireMinutes"]));

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new AuthResult
            {
                Success = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }
        
    }
}
