using NarfoxSparrow.Services;

namespace NarfoxSparrow.Models
{
    /// <summary>
    /// This data model represents the application configuration
    /// and is intended to be populated from JSON during
    /// application startup
    /// </summary>
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

        /// <summary>
        /// The root path to look for images. This will be prepended
        /// to image file paths supplied in JSON data
        /// </summary>
        public string ContentPath { get; set; }

        /// <summary>
        /// The minimum number of hours the app will wait
        /// between tweets.
        /// </summary>
        public float MinimumHoursBetweenTweets { get; set; }

        /// <summary>
        /// The maximum number of hours the app will wait
        /// between tweets.
        /// </summary>
        public float MaximumHoursBetweenTweets { get; set; }

        /// <summary>
        /// How many hashtags each tweet should have. The app
        /// should randomly select from available hashtags and
        /// ensure that it doesn't use the same one multiple times
        /// in a single tweet. The number of hashtags available
        /// should be larger than this number!
        /// </summary>
        public int HashtagsPerTweet { get; set; }
    }
}
