namespace BuildingBlocks.Web
{
    public class JwtOptions
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string SigningKey { get; set; }
        public TimeSpan ExpiresIn { get; set; }
    }
}
