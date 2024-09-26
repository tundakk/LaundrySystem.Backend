namespace LaundrySystem.Domain.Model.Models
{
    public class TokenRefreshModel
    {
        /// <summary>
        /// The current expired or near-expiry JWT token.
        /// </summary>
        public required string Token { get; set; }

        /// <summary>
        /// The refresh token provided during login or previous refresh request.
        /// </summary>
        public required string RefreshToken { get; set; }
    }
}
