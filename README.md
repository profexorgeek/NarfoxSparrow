# NarfoxSparrow

This bot has the specific purpose of tweeting gamedev images from one or more projects on a 
random time interval. It is a simple console app intended to just use data to tweet without
any sophisticated UI. It's primarily built for indie gamedevs to automate their Twitter content.

You must sign up for your own Twitter developer account to use this application.

## Why NarfoxSparrow

As a mostly-solo indie game developer, I struggle to do all of the things required to build 
and ship a game and have any time and energy left for marketing. I often take screenshots or record
gifs while working on the game, and even share it in our gamedev discord channel. But I rarely
get around to posting that on Twitter. So... I wrote this bot to regularly tweet my gamedev images
so that fans of our games see a steady stream of content.

It's called NarfoxSparrow because Narfox Studios is my game company and it tweets regularly... like a sparrow.

## Application lifecyle

This is what the application does:

1. Upon startup it will immediately load `Config/config.json`
1. The application will check for an `auth.json` file with saved user credentials
1. If no auth file is found, it will launch pin-based authentication on Twitter
1. Once authenticated, credentials will be saved for the future. Delete the `auth.json` file to force new auth.
1. The application will give the user a chance to tweet immediately
1. The application will tweet images and text continually at random intervals based on the config settings
1. The application keeps a history and checks tweets against the history to try to avoid duplicate content in too short a time frame

**Note:** that the application does _not_ currently guard against duplicate posts. When I built this I had roughly 150
gamedev images in a folder. I figured if the bot tweets 3x per day it would be roughly 50 days between duplicate 
content. Additionally, with 150 images there is a one in 22,500 chance of the same image being posted twice in a row. 
These were acceptable parameters for our Twitter account.

## App Config (Config/config.json)

The config file specifies the global application configuration. Configuration must be specified 
in `Config/config.json`. This file is **loaded once** when the application launches. See the 
'Config/example_config.json` file for reference. These properties are available for configuration:

- ApiKey: your API key, issued by [Twitter](https://developer.twitter.com)
- ApiSecret: your API secret, issued by [Twitter](https://developer.twitter.com)
- LogLevel: the logging verbosity, defaults to `Debug`
- ContentPath: the root path for your content, this will be prepended to image filenames
- MinimumTweetsBeforeRepeat: the history length, the app will try to avoid tweeting duplicate content more frequently than this number
- MinimumHoursBetweenTweets: the minimum hours between tweets, can be a decimal
- MaximumHoursBetweenTweets: the maximum hours between tweets, can be a decimal
- HashtagsPerTweet: the number of hashtags that are randomly appended to each tweet. This number should be smaller than the number of hashtags specified in your project config!

## Project Config (Config/projects.json)

A Project is typically a game. Projects must be specified in `Config/projects.json`. This
file is **reloaded for every tweet** so that new entries are automatically picked up. See the
`Config/example_projects.json` file for reference. These are the properties that you can specify 
for each project:

- An Id: used to associate images with the project
- A Name: currently not used for anything
- A StoreLink: currently not used for anything
- A HashtagList: used to tag project images
- A Weight: used to determine how frequently this project is tweeted about relative to other projects

## Image Config (Config/images.json)

An image is a jpg, png, or gif file that shows off some game feature or dev progress. Images must
be specified in `Config/images.json`. This file is **reloaded for every tweet** so that new entries
are automatically picked up. See the `example_images.json` file for reference. These are the properties
that you can specify for each image:

- FilePath: the **relative** filepath for the image. The `ContentPath` specified in the application configuration will be prepended.
- Caption: the image caption, this will be the main body of the tweet
- AltText: an alt text description that will be supplied to Twitter for the visually impaired
- ProjectId: the project this image belongs to. This id must exist in the `projects.json` file.

## Questions or Comments?

Reach out to [@profexorgeek on Twitter](https://twitter.com/profexorgeek)
