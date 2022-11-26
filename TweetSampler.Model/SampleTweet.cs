
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Tweetinvi.Models.V2;

namespace TweetSampler.Model;

public static class SampleTweet 
{
    public static bool HasNoHashTags(this TweetV2 tweet)
    {
        return (tweet == null || tweet.Entities == null || tweet.Entities.Hashtags == null);
    }

    public static bool InEnglish(this TweetV2 tweet)
    {
        return (tweet == null || tweet.Lang == Languge.English);
    }

    public static bool TagInEnglish(this TweetV2 tweet)
    {
        if (InEnglish(tweet) == false)
            return false;

        if (tweet.HasNoHashTags())
            return false;

        foreach (var h in tweet.Entities.Hashtags)
        {
            if (!Regex.IsMatch(h.Tag, "^[a-zA-Z0-9]*$"))
                return false;
        }

        return true;
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
