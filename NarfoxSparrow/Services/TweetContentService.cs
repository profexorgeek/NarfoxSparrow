using NarfoxSparrow.Models;
using NarfoxSparrow.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NarfoxSparrow.Services
{
    public class TweetContentService
    {
        public const string ProjectsPath = "Config/projects.json";
        public const string ImagesPath = "Config/images.json";
        public const int MaxLoopIterations = 10;
        public const int MaxTweetChars = 280;

        static TweetContentService instance;

        public static TweetContentService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TweetContentService();
                }
                return instance;
            }
        }

        private TweetContentService() { }

        /// <summary>
        /// Gets random content based on the json data
        /// available to the app.
        /// 
        /// NOTE: loads JSON data upon every request.
        /// This ensures that the app always uses fresh
        /// data to compose a tweet.
        /// </summary>
        /// <returns>A TweetContentModel ready for tweeting</returns>
        public TweetContentModel GetRandomTweet(int numberOfHashtags = 3)
        {
            // load content: we do this every time to make sure data is fresh!
            var tweetContent = new TweetContentModel();
            var sb = new StringBuilder();
            var projects = FileService.Instance.LoadFile<List<Project>>(ProjectsPath);
            var images = FileService.Instance.LoadFile<List<ProjectImage>>(ImagesPath);
            var weightedList = CreatedWeightedList(projects);
            var chosenProjectId = weightedList.Random();
            var chosenProject = projects.Where(p => p.Id == chosenProjectId).FirstOrDefault();
            var chosenImage = images.Where(i => i.ProjectId == chosenProjectId).Random();

            // set main tweet content
            sb.AppendLine(chosenImage.Caption);

            // get N random hashtags
            int tries = 0;
            var hashtags = new List<string>();
            while (hashtags.Count < numberOfHashtags && tries < MaxLoopIterations)
            {
                var newTag = chosenProject.Hashtags.Random();
                if(!hashtags.Contains(newTag))
                {
                    hashtags.Add(newTag);
                }
                tries++;
            }
            var hashtagLine = string.Join(" ", hashtags);

            // append hashtags if we have enough characters
            if (sb.Length + hashtagLine.Length < MaxTweetChars)
            {
                sb.AppendLine(hashtagLine);
            }

            // build the tweet
            tweetContent.ImagePath = chosenImage.FilePath;
            tweetContent.TweetText = sb.ToString();
            tweetContent.ImageAltText = chosenImage.AltText;

            return tweetContent;
        }

        /// <summary>
        /// Populates a List with a number of projectIds based
        /// on the project weight. This allows randomly selecting
        /// from the list and getting a weighted random value
        /// </summary>
        /// <param name="projects">A list of projects</param>
        /// <returns>A list of project Ids where a project Id is 
        /// repeated based on the project weight</returns>
        public List<string> CreatedWeightedList(List<Project> projects)
        {
            List<string> weightedList = new List<string>();
            foreach (var proj in projects)
            {
                for (var i = 0; i < proj.Weight; i++)
                {
                    weightedList.Add(proj.Id);
                }
            }
            return weightedList;
        }
    }
}
