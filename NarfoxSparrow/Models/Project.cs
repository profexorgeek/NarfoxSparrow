using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NarfoxSparrow.Models
{
    public class Project
    {
        List<string> hashtags;

        /// <summary>
        /// The project Id, which is used as the master
        /// reference for a project.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The project name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A URL link to the project's store page.
        /// </summary>
        public string StoreLink { get; set; }

        /// <summary>
        /// A space-separated list of twitter hashtags
        /// that will be chosen randomly for tweets.
        /// </summary>
        public string HashtagList { get; set; }
        
        /// <summary>
        /// The project's tweet weight. This determines how
        /// frequently it is randomly chosen to be the focus
        /// of a tweet.
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// A list of hashtags
        /// </summary>
        [JsonIgnore]
        public List<string> Hashtags
        {
            get
            {
                if(hashtags == null)
                {
                    hashtags = new List<string>();
                    foreach(var tag in HashtagList.Split(" "))
                    {
                        hashtags.Add($"#{tag.Trim()}");
                    }
                }

                return hashtags;
            }
        }
    }
}
