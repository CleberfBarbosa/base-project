namespace Infra.Domain
{
    public class ErrorDefault
    {
        /// <summary>
        /// Error description
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// Error message. <code>Exception.Message</code>
        /// </summary>
        public string? Detail { get; set; }
        /// <summary>
        /// Status code
        /// </summary>
        public int Status { get; set; }
        public string? Stacktrace { get; set; }
    }
}
