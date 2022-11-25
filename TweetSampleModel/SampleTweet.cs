
using System.Text.Json;
using System.Text.Json.Serialization;
using Tweetinvi.Models.V2;

namespace TweetSampler.Model;

public static class SampleTweet 
{
    public static bool HasNoHashTags(this TweetV2 tweet)
    {
        return (tweet.Entities == null || tweet.Entities.Hashtags == null);
    }

    public static void Log(this TweetV2 tweet, NLog.Logger logger)
    {
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve
        };

        var jsonString = JsonSerializer.Serialize(tweet, options);

        logger.Info(jsonString);
    }
}
