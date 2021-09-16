using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Tweetinvi;
using Newtonsoft.Json;
using NarfoxSparrow.Logging;
using System.IO;
using NarfoxSparrow.Models;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using Tweetinvi.Logic.QueryParameters;

namespace NarfoxSparrow
{
    class Program
    {
        const string ConfigPath = "config.json";
        const string AuthFile = "auth.json";

        static async Task Main(string[] args)
        {
            Config config;
            try
            {
                var configJson = File.ReadAllText(ConfigPath);
                config = JsonConvert.DeserializeObject<Config>(configJson);
                Log.Instance.Level = config.LogLevel;
                Log.Instance.Info($"Loaded config from {ConfigPath} and set LogLevel to {config.LogLevel}.");
            }
            catch(Exception e)
            {
                Log.Instance.Error($"Failed to load app configuration: {e.Message}");
                throw e;
            }

            TwitterCredentials userCredentials;
            if(!File.Exists(AuthFile))
            {
                Log.Instance.Info("Creating application client.");
                var appClient = new TwitterClient(config.ApiKey, config.ApiSecret);

                Log.Instance.Info("Attempting to authenticate user.");
                var authReq = await appClient.Auth.RequestAuthenticationUrlAsync();

                Log.Instance.Info("Launching browser.");
                Process.Start(new ProcessStartInfo(authReq.AuthorizationURL)
                {
                    UseShellExecute = true
                });

                var pin = GetUserInput("Enter pin provided by auth process and press enter:");
                userCredentials = await appClient.Auth
                    .RequestCredentialsFromVerifierCodeAsync(pin, authReq) as TwitterCredentials;

                // save credentials test
                var userJson = JsonConvert.SerializeObject(userCredentials);
                File.WriteAllText(AuthFile, userJson);
            }
            else
            {
                Log.Instance.Info("Loading saved credentials.");
                var authJson = File.ReadAllText(AuthFile);
                userCredentials = JsonConvert.DeserializeObject<TwitterCredentials>(authJson);
            }

            
            Log.Instance.Info("Creating user client...");
            var userClient = new TwitterClient(userCredentials);
            var user = await userClient.Users.GetAuthenticatedUserAsync();

            Log.Instance.Info($"Authenticated user: {user}...");

            // TODO: error handling!
            await PublishImageTweetAsync(userClient);

            Log.Instance.Info("Application successful.");
        }

        static async Task<ITweet> PublishImageTweetAsync(TwitterClient client)
        {
            var data = File.ReadAllBytes("./Content/wee-go-home.png");
            var upload = await client.Upload.UploadTweetImageAsync(data);
            await client.Upload.AddMediaMetadataAsync(new MediaMetadata(upload)
            {
                AltText = "Leaving the dunegon to go home.",
            });
            var tweet = await client.Tweets.PublishTweetAsync(
                new PublishTweetParameters("NarfoxSparrow likes pixelart.")
                {
                    Medias = { upload }
                    
                });
            Log.Instance.Info($"Image tweet published: {tweet}");
            return tweet;
        }

        static async Task<ITweet> PublishVideoTweetAsync(TwitterClient client)
        {
            var data = File.ReadAllBytes("./Content/wee-forest-fireflies.gif");
            var upload = await client.Upload.UploadTweetVideoAsync(data);

            // Await image processing
            await client.Upload.WaitForMediaProcessingToGetAllMetadataAsync(upload);

            var tweet = await client.Tweets.PublishTweetAsync(
                new PublishTweetParameters("NarfoxSparrow likes gifs.")
                {
                    Medias = { upload }
                });
            Log.Instance.Info($"Video tweet published: {tweet}");
            return tweet;
        }

        static string GetUserInput(string prompt)
        {
            Console.WriteLine(prompt);
            var input = Console.ReadLine();
            return input;
        }
    }
}
