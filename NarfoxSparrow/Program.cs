using NarfoxSparrow.Models;
using NarfoxSparrow.Services;
using NarfoxSparrow.Utilities;
using System;
using System.IO;
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
