namespace Infra.Domain.Options
{
    public class JwtOptions
    {
        public const string SectionName = "Jwt";
        public string? PrivateKey { get; set; }
        public int ExpireTime { get; set; }
    }
}
