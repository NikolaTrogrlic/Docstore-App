using Backend.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend.Services
{
    public class UserService(IConfiguration configuration)
    {
        //Ovo bi inače napravio u SQL / Entity Framework-u ali za potrebe ovog primjera postojati će mockani useri u appsettings file-u.
        private readonly List<User> _users = configuration.GetSection("MockUsers").Get<List<User>>() ?? [];
        private readonly IConfiguration _config = configuration;

        public User? GetAuthenticatedUser(HttpContext context)
        {
            var username = context.User.Identity?.Name;
            var role = context.User.FindFirst(ClaimTypes.Role)?.Value;

            return _users.Find(x => x.Username == username && x.Role.ToString() == role);
        }

        public string AuthenticateUser(string username, string password)
        {
            var userIndex = _users.FindIndex(x => x.Username == username && x.Password == password);

            if (userIndex != -1)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, _users[userIndex].Username),
                    new(ClaimTypes.Role, _users[userIndex].Role.ToString())
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var result = double.TryParse(_config["Jwt:ExpireMinutes"], out double duration);

                if (result)
                {
                    var token = new JwtSecurityToken(
                        issuer: _config["Jwt:Issuer"],
                        audience: _config["Jwt:Audience"],
                        claims: claims,
                        expires: DateTime.UtcNow.AddMinutes(duration),
                        signingCredentials: creds);

                    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                    return tokenString;
                }
                else
                {
                    return "ERROR: Couldn't parse max login time";
                }
            }
            else
            {
                return "ERROR: User with that username or password not found";
            }
        }

        public bool IsUser(HttpContext context)
        {
            var user = context.User.Identity?.Name;
            return user != null;
        }

        public string? GetContextUsername(HttpContext context)
        {
            return context.User.Identity?.Name;
        }

        public bool IsAdmin(HttpContext context)
        {
            var role = context.User.FindFirst(ClaimTypes.Role)?.Value;
            return role == "Admin";
        }
    }
}
