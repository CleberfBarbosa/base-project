namespace Infra.Domain.Options
{
    public class SwaggerOptions
    {
        public const string SectionName = "SwaggerProperties";
        public string Name { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public string DescriptionAuth { get; set; }
    }
}
