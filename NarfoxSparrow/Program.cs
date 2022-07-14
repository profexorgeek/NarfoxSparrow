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
        const string HistoryPath = "history.json";
        const int MaxUniquenessIterations = 10;

        static Config config;
        static List<TweetContentModel> history;

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

            // set this to true to test stuff without actually tweeting
            // TwitterService.Instance.TestTweetOnly = true;
            LogService.Instance.Info($"In test only mode? {TwitterService.Instance.TestTweetOnly}");

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
            // try to get tweet content whose image hasn't been tweeted in awhile
            var tweetContent = TweetContentService.Instance.GetRandomTweet(config.HashtagsPerTweet);
            var iterations = 0;

            try
            {
                while (iterations < MaxUniquenessIterations && history.Where(t => t.ImagePath == tweetContent.ImagePath).Any())
                {
                    // NOTE: within the while loop we suppress content reloads because we don't want to spam IO
                    tweetContent = TweetContentService.Instance.GetRandomTweet(config.HashtagsPerTweet, true);
                    iterations++;
                }
                var imgPath = Path.Combine(config.ContentPath, tweetContent.ImagePath);
                var tweet = await TwitterService.Instance.TryImageTweet(tweetContent.TweetText, imgPath, tweetContent.ImageAltText);

                // if we successfully tweeted, add the tweet to the history and prune it to the target length
                if (tweet != null)
                {
                    history.Add(tweetContent);
                    if (history.Count > config.MinimumTweetsBeforeRepeat)
                    {
                        history.RemoveAt(0);
                    }
                    FileService.Instance.SaveFile(history, HistoryPath);
                }

                return tweet;
            }
            catch(Exception ex)
            {
                LogService.Instance.Error($"Failed to post tweet: {ex.Message}\n{ex.StackTrace}\n{ex.InnerException}");
                return null;
            }
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

            if (File.Exists(HistoryPath))
            {
                history = FileService.Instance.LoadFile<List<TweetContentModel>>(HistoryPath);
            }
            else
            {
                history = new List<TweetContentModel>();
            }

            LogService.Instance.Level = config.LogLevel;
            TwitterService.Instance.Initialize(config.ApiKey, config.ApiSecret);
        }
    }
}
