namespace Infra.Domain.Options
{
    public class DatabaseOptions
    {
        public const string SectionName = "Database";
        public string? ConnectionString { get; set; }
    }
}
