using NarfoxSparrow.Models;
using NarfoxSparrow.Services;
using System;
using System.Threading.Tasks;

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

            await TwitterService.Instance.TryImageTweet(
                "Gif tweet test",
                "./Content/wee-forest-fireflies.gif",
                "Fireflies in the forest");
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
