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

        List<Project> projects;
        List<ProjectImage> images;
        List<string> weightedList;
        bool contentLoadedOnce = false;

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
        public TweetContentModel GetRandomTweet(int numberOfHashtags = 3, bool suppressContentReload = false)
        {
            if (contentLoadedOnce == false || suppressContentReload == false)
            {
                LoadContent();
            }
            var chosenProjectId = weightedList.Random();
            var chosenProject = projects.Where(p => p.Id == chosenProjectId).FirstOrDefault();
            var projectImages = images.Where(i => i.ProjectId == chosenProjectId);
            var chosenImage = projectImages.Random();
            var imageIndex = images.IndexOf(chosenImage);

            var sb = new StringBuilder();
            sb.AppendLine(chosenImage.Caption);

            // get N random hashtags
            int tries = 0;
            var hashtags = new List<string>();
            while (hashtags.Count < numberOfHashtags && tries < MaxLoopIterations)
            {
                var newTag = chosenProject.Hashtags.Random();
                if (!hashtags.Contains(newTag))
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
            var tweetContent = new TweetContentModel();
            tweetContent.ImagePath = chosenImage.FilePath;
            tweetContent.TweetText = sb.ToString();
            tweetContent.ImageAltText = chosenImage.AltText;
            tweetContent.TweetTime = DateTime.Now;

            LogService.Instance.Debug($"Got tweet content: {tweetContent.TweetText} with image {tweetContent.ImagePath}");

            return tweetContent;
        }

        /// <summary>
        /// Loads or reloads projects and images and regenerates weighted list
        /// </summary>
        void LoadContent()
        {
            projects = FileService.Instance.LoadFile<List<Project>>(ProjectsPath);
            images = FileService.Instance.LoadFile<List<ProjectImage>>(ImagesPath);

            weightedList = new List<string>();
            foreach (var proj in projects)
            {
                for (var i = 0; i < proj.Weight; i++)
                {
                    weightedList.Add(proj.Id);
                }
            }

            contentLoadedOnce = true;
        }
    }
}
