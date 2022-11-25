using Tweetinvi.Models.V2;
using TweetSampler.Model;

namespace TweetSampler.Model.Tests;

public class SampleTweetTests
{
    [Fact]
    public void EmptyTweetHasNoHashTags()
    {
        TweetV2 tweet = new TweetV2();
        Assert.True(SampleTweet.HasNoHashTags(tweet));
    }

    [Fact]
    public void TweetHasHashTags()
    {
        TweetV2 tweet = new TweetV2();
        tweet.Entities = new TweetEntitiesV2();
        tweet.Entities.Hashtags = new HashtagV2[]
        {
            new HashtagV2 {
                Start = 99,
                End = 116,
                Tag = "JFKassassination"
            }
        };
        Assert.False(SampleTweet.HasNoHashTags(tweet));
    }
}
