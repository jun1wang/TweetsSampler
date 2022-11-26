
using System.Text.Json;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Models.V2;
using System.Text.Json.Serialization;
using TweetSampler.Model;
using Tweetinvi.Events.V2;

namespace TweetSampler.Core;

public class SampleStreamProcessorWithFilter : SampleStreamProcessorBase
{
    public SampleStreamProcessorWithFilter(string token, string lang, int numTopHashTags = 10)
        : base(token, numTopHashTags)
    {
        Language = lang;
    }


    protected override bool Ignorable(TweetV2 tweet)
    {
        return (SampleTweet.HasNoHashTags(tweet) || (Language == "English" && SampleTweet.InEnglish(tweet) == false));
    }

    #region Fileds

    public string? Language { get; set; }

    #endregion
}

