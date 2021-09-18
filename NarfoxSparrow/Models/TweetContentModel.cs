namespace NarfoxSparrow.Models
{
    /// <summary>
    /// This model represents content that could appear
    /// in a tweet. It is intended to be the output
    /// of a random content service that can author
    /// tweets to be automatically posted
    /// </summary>
    public class TweetContentModel
    {
        /// <summary>
        /// The text that should appear in a tweet
        /// </summary>
        public string TweetText { get; set; }

        /// <summary>
        /// The image that should appear in a tweet
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// The alt text for an image
        /// </summary>
        public string ImageAltText { get; set; }
    }
}
