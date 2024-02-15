namespace Infra.Domain.Options
{
    public class DatabaseOptions
    {
        public const string SectionName = "DatabaseProperties";
        public string? ConnectionString { get; set; }
    }
}
