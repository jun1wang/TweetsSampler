
using System.Text.Json;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Models.V2;
using System.Text.Json.Serialization;
using TweetSampleModel;
using Microsoft.Extensions.Logging;

namespace TweetProcessor;

public class Processor
{
    private readonly ILogger<Processor> _logger;

    private static Uri StreamV2Uri = new Uri("https://api.twitter.com/2/tweets/sample/stream?tweet.fields=created_at&expansions=author_id&user.fields=created_at");
    private static string ? Token;
    private static int TotalTweets = 0;
    private static SortedDictionary<string, int> HashTagCounts = new SortedDictionary<string, int>();

    public Processor(string token)
    {
        Token = token;
    }

    public async Task Run()
    {
        try
        {
            var credentials = new TwitterCredentials();
            credentials.BearerToken = Token;
            var client = new TwitterClient(credentials);

            var sampleStreamV2 = client.StreamsV2.CreateSampleStream();
            sampleStreamV2.TweetReceived += (sender, args) =>
            {
                ProcessTweet(args.Tweet);
                if (TotalTweets % 1000 == 0)
                {
                    ProcessTrendingHashTags();
                }
            };

            await sampleStreamV2.StartAsync();
        }
        catch (HttpRequestException e)
        {
            _logger?.LogError("\nException Caught!");
            _logger?.LogError("Message :{0} ", e.Message);
        }
    }

    private void PrintTweet(TweetV2 tweet)
    {
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve
        };

        var jsonString = JsonSerializer.Serialize(tweet, options);

        _logger?.LogInformation($"*** Tweet # {TotalTweets}, # of hashTags = {tweet.Entities.Hashtags?.Count()} ***");

        if (tweet.Entities != null && tweet.Entities.Hashtags != null)
        {
            foreach (var h in tweet.Entities.Hashtags)
            {
                _logger?.LogInformation(h.Tag);
            }
        }
        _logger?.LogInformation(jsonString); 
    }

    private void ProcessTweet(TweetV2 tweet)
    {
        ++TotalTweets;
        if (tweet.Entities != null && tweet.Entities.Hashtags != null)
        {
            foreach (var t in tweet.Entities.Hashtags)
            {
                if (HashTagCounts.ContainsKey(t.Tag))
                {
                    ++HashTagCounts[t.Tag];
                    _logger?.LogInformation($"Found existing tag {t.Tag}, count = {HashTagCounts[t.Tag]}");
                }
                else
                {
                    HashTagCounts.Add(t.Tag, 1);
                    _logger?.LogInformation($"New tag {t.Tag}, count = {HashTagCounts[t.Tag]}");
                }
            }
        }

    }

    private static void ProcessTrendingHashTags()
    {
        var sortedKVPairs = (
                        from kv in HashTagCounts
                        orderby kv.Value descending
                        select kv
                        ).Take(10);

        DisplayTrendingHashTags(sortedKVPairs);
        SaveTrendingHashTags(sortedKVPairs);
    }

    private static void SaveTrendingHashTags(IEnumerable<KeyValuePair<string, int>> sortedKVPairs)
    {
        string fileName = "../Data/TrendingHashTags.json";
        var trendingHashTags = new TrendingHashTags();

        int rank = 0;
        foreach (var p in sortedKVPairs)
        {
            var hc = new HashTag();
            hc.Rank = ++rank;
            hc.Tag = p.Key;
            hc.Tweets = p.Value;
            trendingHashTags.TopTags?.Add(hc);
            System.Console.WriteLine($"Top # {hc.Rank} {hc.Tag},tweets {hc.Tweets}, out of {TotalTweets} sampled tweets");
        }
        trendingHashTags.SampleSize = TotalTweets;

        var options = new JsonSerializerOptions { IncludeFields = true };
        string jsonString = JsonSerializer.Serialize(trendingHashTags, options);
        File.WriteAllText(fileName, jsonString);

        Console.WriteLine(File.ReadAllText(fileName));
    }

    private static void DisplayTrendingHashTags(IEnumerable<KeyValuePair<string, int>> sortedKVPairs)
    {
        System.Console.WriteLine($"**************************");
        int i = 0;
        foreach (var p in sortedKVPairs)
        {
            System.Console.WriteLine($"Top {++i} hashtag:{p.Key},tweets {p.Value}, out of {TotalTweets} sampled tweets");
        }
    }
}

