using NarfoxSparrow.Models;
using NarfoxSparrow.Services;
using NarfoxSparrow.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi.Models;

namespace NarfoxSparrow
{
    class Program
    {
        const string ConfigPath = "Config/config.json";

        static Config config;

        static async Task Main(string[] args)
        {
            Initialize();

            await TwitterService.Instance.TryAuthenticateUserAsync();

            bool tweetNow = false;
            try
            {
                tweetNow = Convert.ToBoolean(InputService.Instance.GetUserInput("Tweet immediately (true or false)?"));
            }
            catch (Exception e)
            {
                LogService.Instance.Warn("Bad input received, resorting to default: no immediate tweet.");
            }

            // TestForDuplicates();

            if (tweetNow)
            {
                await PostRandomTweet();
            }

            Random rand = new Random();
            while (true)
            {
                var millisecondsToSleep = GetRandomSleepMilliseconds();
                var nextTweetTime = DateTime.Now.AddMilliseconds(millisecondsToSleep);
                LogService.Instance.Info($"Next tweet will be: {nextTweetTime.ToString()}");
                await Task.Delay(millisecondsToSleep);
                await PostRandomTweet();
            }
        }

        /// <summary>
        /// Brute force test randomization to make sure selection of
        /// tweet content is randomly distributed
        /// </summary>
        /// <param name="numberOfTweets">The number of tweet contents to randomly generate</param>
        static void TestForDuplicates(int numberOfTweets = 1000)
        {
            // get a ton of tweets
            List<TweetContentModel> tweetContent = new List<TweetContentModel>();
            for(var i = 0; i < numberOfTweets; i++)
            {
                tweetContent.Add(TweetContentService.Instance.GetRandomTweet());
            }

            // count duplicates
            var dupeCounts = new Dictionary<string, int>();
            var totalDupes = 0;
            for(var i = tweetContent.Count - 1; i > -1; i--)
            {
                var t1 = tweetContent[i];

                if(dupeCounts.ContainsKey(t1.ImagePath))
                {
                    continue;
                }
                else
                {
                    var count = tweetContent.Count(t => t.ImagePath == t1.ImagePath);
                    dupeCounts.Add(t1.ImagePath, count);
                }
            }

            LogService.Instance.Info("\tPath\t\t\tCount");
            foreach(var kvp in dupeCounts)
            {
                LogService.Instance.Info($"\t{kvp.Key}\t\t\t{kvp.Value}");
            }
        }

        static int GetRandomSleepMilliseconds()
        {
            float hoursToNextTweet = Extensions.RNG.InRange(config.MinimumHoursBetweenTweets, config.MaximumHoursBetweenTweets);

            // hours * 60min * 60sec * 1000milli
            float milliseconds = hoursToNextTweet * 60 * 60 * 1000;
            int milliRounded = (int)Math.Floor(milliseconds);
            return milliRounded;
        }

        static async Task<ITweet> PostRandomTweet()
        {
            var tweetContent = TweetContentService.Instance.GetRandomTweet(config.HashtagsPerTweet);
            var imgPath = Path.Combine(config.ContentPath, tweetContent.ImagePath);
            var tweet = await TwitterService.Instance.TryImageTweet(tweetContent.TweetText, imgPath, tweetContent.ImageAltText);

            return tweet;
        }

        static void Initialize()
        {
            try
            {
                config = FileService.Instance.LoadFile<Config>(ConfigPath);
            }
            catch (Exception e)
            {
                // catch so we can log the error
                LogService.Instance.Error($"Failed to load app configuration: {e.Message}");

                // rethrow because the app can't keep going
                throw e;
            }

            LogService.Instance.Level = config.LogLevel;
            TwitterService.Instance.Initialize(config.ApiKey, config.ApiSecret);
        }
    }
}
