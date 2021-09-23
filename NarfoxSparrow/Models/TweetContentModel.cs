using System;

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
        /// The image that should appear in a tweet. This 
        /// expected to also be a unique ID as a single
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// The text that should appear in a tweet
        /// </summary>
        public string TweetText { get; set; }

        /// <summary>
        /// The alt text for an image
        /// </summary>
        public string ImageAltText { get; set; }
        
        /// <summary>
        /// Stores the time and date this tweet content
        /// was created. Intended to be used for historic
        /// log that the app can use to avoid duplicate tweets
        /// </summary>
        public DateTime TweetTime { get; set; }
    }
}
