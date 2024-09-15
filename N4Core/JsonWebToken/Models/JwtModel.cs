namespace N4Core.JsonWebToken.Models
{
    public class JwtModel
    {
        public string Token { get; set; } = null!;
        public DateTime Expiration { get; set; }
    }
}
