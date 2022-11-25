
using System.Text.Json;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Models.V2;
using System.Text.Json.Serialization;
using TweetSampler.Model;
using Tweetinvi.Events.V2;

namespace TweetSampler.Core;

public class SampleStreamSummaryReport : ISampleStreamSummaryReport
{

    public void DisplayTrendingHashTags(IEnumerable<KeyValuePair<string, int>> trendingHashTags, int totalTweets)
    {
        System.Console.WriteLine($"\n==============================================================================\n");
        System.Console.WriteLine($"Total sampled tweets: {totalTweets}\n");
        System.Console.WriteLine($"Top 10 Hashtags:\n");
        int i = 0;
        foreach (var p in trendingHashTags)
        {
            System.Console.WriteLine(string.Format("  {0, 3}: #{1, -50} {2, 8} tweets.", ++i, p.Key?.Trim(), p.Value));
        }
    }
}

