namespace Infra.Domain.Options
{
    public class JwtOptions
    {
        public const string SectionName = "JwtProperties";
        public string PrivateKey { get; set; }
        public string IssuerUrl { get; set; } = "";
        public int ExpireTime { get; set; }
    }
}
