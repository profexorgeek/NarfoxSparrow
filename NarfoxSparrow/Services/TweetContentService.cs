using NarfoxSparrow.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NarfoxSparrow.Services
{
    public class TweetContentService
    {
        public const string ProjectsPath = "Config/projects.json";
        public const string ImagesPath = "Config/images.json";
        Random rand;

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

        private TweetContentService()
        {
            rand = new Random();
        }

        /// <summary>
        /// Gets random content based on the json data
        /// available to the app.
        /// 
        /// NOTE: loads JSON data upon every request.
        /// This ensures that the app always uses fresh
        /// data to compose a tweet.
        /// </summary>
        /// <returns>A TweetContentModel ready for tweeting</returns>
        public TweetContentModel GetRandomTweet()
        {
            var tweetContent = new TweetContentModel();

            var projects = FileService.Instance.LoadFile<List<Project>>(ProjectsPath);
            var images = FileService.Instance.LoadFile<List<ProjectImage>>(ImagesPath);
            var weightedList = CreatedWeightedList(projects);

            // choose a random project
            var i = rand.Next(0, weightedList.Count);
            var chosenProjectId = weightedList[i];


            return tweetContent;
        }

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
