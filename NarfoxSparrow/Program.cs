using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Tweetinvi;
using Newtonsoft.Json;
using NarfoxSparrow.Logging;

namespace NarfoxSparrow
{
    class Program
    {
        const string ConfigPath = "config.json";

        static async Task Main(string[] args)
        {
            var config = JsonConvert.DeserializeObject<Models.Config>(ConfigPath);
            Log.Instance.Level = config.LogLevel;
            Log.Instance.Info($"Loaded config from {ConfigPath} and set LogLevel to {config.LogLevel}.");


            Log.Instance.Info("Creating twitter client.");
            var appClient = new TwitterClient(config.ApiKey, config.ApiSecret);

            Log.Instance.Info("Attempting to authenticate user.");
            var authReq = await appClient.Auth.RequestAuthenticationUrlAsync();

            Log.Instance.Info("Launching browser.");
            Process.Start(new ProcessStartInfo(authReq.AuthorizationURL)
            {
                UseShellExecute = true
            });

            var pin = GetUserInput("Enter pin provided by auth process and press enter:");
            var userCredentials = await appClient.Auth
                .RequestCredentialsFromVerifierCodeAsync(pin, authReq);

            Log.Instance.Info("Creating twitter client...");
            var userClient = new TwitterClient(userCredentials);
            var user = await userClient.Users.GetAuthenticatedUserAsync();

            Log.Instance.Info($"Authenticated user: {user}...");

            var tweet = await userClient.Tweets.PublishTweetAsync("Tweets in the mirror are larger than they appear.");
            Log.Instance.Info("Tweet published: {tweet}");
        }

        static string GetUserInput(string prompt)
        {
            Console.WriteLine(prompt);
            var input = Console.ReadLine();
            return input;
        }
    }
}
