
using System.Text.Json;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Models.V2;
using System.Text.Json.Serialization;
using TweetSampler.Model;
using Tweetinvi.Events.V2;

namespace TweetSampler.Core;

public class SampleStreamProcessorDefault : SampleStreamProcessorBase
{
    public SampleStreamProcessorDefault(string token, int numTopHashTags = 10): base(token, numTopHashTags) { }

    protected override bool Ignorable(TweetV2 tweet)
    {
        return (SampleTweet.HasNoHashTags(tweet));
    }
}

