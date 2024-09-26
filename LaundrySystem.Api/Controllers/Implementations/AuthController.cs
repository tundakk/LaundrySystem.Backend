namespace LaundrySystem.API.Controllers
{
    using LaundrySystem.Domain.Model.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Controller for handling authentication-related actions.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly string _jwtSecretKey;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="configuration">The configuration.</param>
        public AuthController(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
            _jwtSecretKey = _configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT secret is not configured.");
            _jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured.");
            _jwtAudience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured.");
        }

        /// <summary>
        /// Authenticates a user and generates a JWT token.
        /// </summary>
        /// <param name="model">The login details.</param>
        /// <returns>An <see cref="IActionResult"/> containing the JWT token and expiration.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            var roles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecretKey));
            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            // Generate a refresh token
            var refreshToken = Guid.NewGuid().ToString();

            // Store refresh token in AspNetUserTokens table
            await _userManager.SetAuthenticationTokenAsync(user, _jwtIssuer, "RefreshToken", refreshToken);

            return Ok(new
            {
                token = jwtToken,
                refreshToken,
                expiration = token.ValidTo
            });
        }

        /// <summary>
        /// Refreshes the JWT token using the provided refresh token.
        /// </summary>
        /// <param name="model">The token refresh details.</param>
        /// <returns>An <see cref="IActionResult"/> containing the new JWT token and refresh token.</returns>
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRefreshModel model)
        {
            var principal = GetPrincipalFromExpiredToken(model.Token);
            var username = principal?.Identity?.Name;
            if (username == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return Unauthorized();
            }

            // Get the stored refresh token
            var storedRefreshToken = await _userManager.GetAuthenticationTokenAsync(user, _jwtIssuer, "RefreshToken");

            if (storedRefreshToken != model.RefreshToken)
            {
                return Unauthorized(new { message = "Invalid refresh token." });
            }
            var claims = principal?.Claims?.ToList();
            if (claims == null)
            {
                return Unauthorized(new { message = "Invalid token claims." });
            }
            var newJwtToken = GenerateJwtToken(claims);
            var newRefreshToken = Guid.NewGuid().ToString();

            // Store the new refresh token
            await _userManager.SetAuthenticationTokenAsync(user, _jwtIssuer, "RefreshToken", newRefreshToken);

            return Ok(new
            {
                token = newJwtToken,
                refreshToken = newRefreshToken
            });
        }

        /// <summary>
        /// Generates a new JWT token.
        /// </summary>
        /// <param name="claims">The claims to include in the token.</param>
        /// <returns>The generated JWT token.</returns>
        private string GenerateJwtToken(List<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecretKey));
            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                expires: DateTime.Now.AddHours(3),
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Gets the principal from an expired token.
        /// </summary>
        /// <param name="token">The expired token.</param>
        /// <returns>The claims principal.</returns>
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecretKey)),
                ValidateLifetime = false, // Important: we are checking an expired token
                ValidIssuer = _jwtIssuer,
                ValidAudience = _jwtAudience,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}