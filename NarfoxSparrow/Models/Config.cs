using NarfoxSparrow.Services;

namespace NarfoxSparrow.Models
{
    public class Config
    {
        /// <summary>
        /// An API Key issued by developer.twitter.com
        /// </summary>
        public string ApiKey {get;set;}

        /// <summary>
        /// An API Key issued by developer.twitter.com
        /// </summary>
        public string ApiSecret { get; set; }

        /// <summary>
        /// The log level to run the application at.
        /// </summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Debug; 
    }
}
