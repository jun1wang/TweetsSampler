
using System.Text.Json;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Models.V2;
using System.Text.Json.Serialization;
using TweetSampler.Model;
using Tweetinvi.Events.V2;

namespace TweetSampler.Core;

public class SampleStreamSaver : ISampleStreamSaver
{

    public void SaveToFileAsJson(IEnumerable<KeyValuePair<string, int>> topHashTags)
    {
        string fileName = "../Data/TrendingHashTags.json";
        var trendingHashTags = new TrendingHashTags();

        int rank = 0;
        foreach (var p in topHashTags)
        {
            var hc = new HashTag
            {
                Rank = ++rank,
                Tag = p.Key,
                Tweets = p.Value
            };
            trendingHashTags.TopTags?.Add(hc);
        }

        var options = new JsonSerializerOptions { IncludeFields = true };
        string jsonString = JsonSerializer.Serialize(trendingHashTags, options);
        File.WriteAllText(fileName, jsonString);
    }

    public void SaveToDatabase(IEnumerable<KeyValuePair<string, int>> topHashTags)
    {
        throw new Exception("Not yet implemented");
    }

}

