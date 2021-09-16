using NarfoxSparrow.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Logic.QueryParameters;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace NarfoxSparrow.Services
{
    public class TwitterService
    {
        const string AuthSavePath = "auth.json";

        static TwitterService instance;

        bool initialized = false;
        string apiKey;
        string apiSecret;
        TwitterClient userClient;
        IAuthenticatedUser user;


        public static TwitterService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TwitterService();
                }
                return instance;
            }
        }

        private TwitterService() { }

        public void Initialize(string apiKey, string apiSecret)
        {
            this.apiKey = apiKey;
            this.apiSecret = apiSecret;
            initialized = true;
        }

        /// <summary>
        /// Attempts to authenticate a user by checking if a saved
        /// authentication token exists and attempts to use existing.
        /// Otherwise it will use PIN authentication to authenticate
        /// the app to post. If successful, it will establish a Twitter
        /// client and cache the authenticated user data.
        /// </summary>
        /// <returns>bool value indicating success!</returns>
        public async Task<bool> TryAuthenticateUserAsync()
        {
            CheckThrowInitializeError();

            TwitterCredentials twitterCredentials;

            // we don't have an auth file, we need to
            // use pin auth for the user
            if (!File.Exists(AuthSavePath))
            {
                LogService.Instance.Info("No user auth found, requesting PIN authentication.");

                // attempt to pin authenticate
                try
                {
                    var appClient = new TwitterClient(apiKey, apiSecret);
                    var authReq = await appClient.Auth.RequestAuthenticationUrlAsync();
                    Process.Start(new ProcessStartInfo(authReq.AuthorizationURL)
                    {
                        UseShellExecute = true
                    });

                    var pin = InputService.Instance.GetUserInput("Enter PIN provided by Twitter:");
                    twitterCredentials = await appClient.Auth
                        .RequestCredentialsFromVerifierCodeAsync(pin, authReq) as TwitterCredentials;
                }
                catch (Exception e)
                {
                    LogService.Instance.Error($"Error authenticating user: {e.Message}");
                    twitterCredentials = null;
                }

                // Attempt to save the credentials to disk.
                try
                {
                    if (twitterCredentials != null)
                    {
                        FileService.Instance.SaveFile(twitterCredentials, AuthSavePath);
                    }
                }
                catch (Exception e)
                {
                    LogService.Instance.Error($"Error saving auth credentials: {e.Message}");
                }

            }

            // attempt to load credentials from disk
            else
            {
                try
                {
                    LogService.Instance.Info("Loading saved credentials.");
                    twitterCredentials = FileService.Instance.LoadFile<TwitterCredentials>(AuthSavePath);
                }
                catch (Exception e)
                {
                    LogService.Instance.Error($"Error loading auth credentials: {e.Message}");
                    twitterCredentials = null;
                }
            }

            // create a twitter client and fetch the user
            if (twitterCredentials != null)
            {
                userClient = new TwitterClient(twitterCredentials);
                user = await userClient.Users.GetAuthenticatedUserAsync();
                LogService.Instance.Info($"Authenticated user: @{user.ScreenName}");
                return true;
            }
            else
            {
                LogService.Instance.Error("Failed to created authenticated Twitter client.");
                return false;
            }

        }

        /// <summary>
        /// Publishes a standard text tweet to the authenticated
        /// user's timeline.
        /// </summary>
        /// <param name="text">The text to tweet.</param>
        /// <returns>The completed ITweet instance</returns>
        public async Task<ITweet> TryTextTweet(string text)
        {
            CheckThrowInitAndAuthError();
            var tweet = await userClient.Tweets.PublishTweetAsync(text);
            LogService.Instance.Info($"Tweet published: {tweet}");
            return tweet;
        }

        /// <summary>
        /// Publishes an image tweet to the authenticated
        /// user's timeline.
        /// </summary>
        /// <param name="text">The tweet text content</param>
        /// <param name="imagePath">The image filepath</param>
        /// <param name="imageAlt">Optional: The image alt text</param>
        /// <returns></returns>
        public async Task<ITweet> TryImageTweet(string text, string imagePath, string imageAlt = null)
        {
            CheckThrowInitAndAuthError();

            var imgExt = Path.GetExtension(imagePath);
            var mediaCategory = MediaCategory.Image;
            switch(imgExt.ToLower())
            {
                case ".gif":
                    mediaCategory = MediaCategory.Gif;
                    break;
                case ".jpg":
                    mediaCategory = MediaCategory.Image;
                    break;
                case ".png":
                    mediaCategory = MediaCategory.Image;
                    break;
                default:
                    throw new Exception($"Unknown media type: {imgExt}");
            }

            var data = File.ReadAllBytes(imagePath);
            LogService.Instance.Info($"Uploading image from path: {imagePath}...");
            var upload = await userClient.Upload.UploadTweetImageAsync(
                new UploadTweetImageParameters(data)
                {
                    // 1mb chunk size
                    MaxChunkSize = (1024 * 1024),
                    MediaCategory = mediaCategory,
                    WaitForTwitterProcessing = true,
                });
            ITweet tweet = null;

            LogService.Instance.Info($"Image upload complete.");

            if (upload != null)
            {
                // add alt text
                if (string.IsNullOrEmpty(imageAlt))
                {
                    await userClient.Upload.AddMediaMetadataAsync(new MediaMetadata(upload)
                    {
                        AltText = imageAlt,
                    });
                }

                tweet = await userClient.Tweets.PublishTweetAsync(
                    new PublishTweetParameters(text)
                    {
                        MediaIds = { upload.Id.Value }

                    });
                LogService.Instance.Info($"Image tweet published: {tweet}");
            }
            
            
            return tweet;
        }

        /// <summary>
        /// Throws an error if the service is not initialized.
        /// Intended to be called in any method that requires
        /// the service to be Intialized - usually every public
        /// method.
        /// </summary>
        void CheckThrowInitializeError()
        {
            if (!initialized)
            {
                var msg = "You must Initialize this service before using it!";
                LogService.Instance.Error(msg);
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// Throws an error if the service is has not been initialized and
        /// authenticated successfully
        /// </summary>
        void CheckThrowInitAndAuthError()
        {
            CheckThrowInitializeError();

            if (userClient == null || user == null)
            {
                var msg = $"You must call {nameof(TryAuthenticateUserAsync)} successfully before proceeding!";
                LogService.Instance.Error(msg);
                throw new Exception(msg);
            }
        }
    }
}
