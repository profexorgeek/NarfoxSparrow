using System;
using System.Collections.Generic;
using System.Text;

namespace NarfoxSparrow.Models
{
    public class ProjectImage
    {
        /// <summary>
        /// The relative path on disk. This will
        /// be appended to the root path defined
        /// in the config file to resolve the full
        /// image path.
        /// 
        /// This is expected to be unique and may be
        /// used to avoid duplicate content tweets.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// An image description that will be used
        /// in the body of the tweet
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Description of the image for the visually 
        /// impaired. This will be used as the alt
        /// text for the image in the tweet
        /// </summary>
        public string AltText { get; set; }

        /// <summary>
        /// The id of the project this image belongs to
        /// </summary>
        public string ProjectId { get; set; }
    }
}
