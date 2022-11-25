
using System.Text.Json;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Models.V2;
using System.Text.Json.Serialization;
using TweetSampler.Model;

namespace TweetSampler.Core;

public class SampleStreamProcessor
{
    public SampleStreamProcessor(string token, int numTopHashTags = 10)
    {
        Token = token ?? throw new ArgumentNullException("Beare token cannot be null");
        NumTopHashTags = numTopHashTags;
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
                DetectHashTags(args.Tweet);

                if (TotalTweets % 100 == 0)
                {
                    var topHashTags = GetTrendingHashTags();
                    DisplayTrendingHashTags(topHashTags);
                    SaveTrendingHashTags(topHashTags);
                }
            };

            await sampleStreamV2.StartAsync();
        }
        catch (HttpRequestException e)
        {
            _logger.Error("\nException Caught!");
            _logger.Error("Message :{0} ", e.Message);
        }
    }


    private void DetectHashTags(TweetV2 tweet)
    {
        ++TotalTweets;

        if (SampleTweet.HasNoHashTags(tweet))
            return;

        foreach (var t in tweet.Entities.Hashtags)
        {
            if (HashTagCounts.ContainsKey(t.Tag))
            {
                ++HashTagCounts[t.Tag];
                _logger.Info($"Found existing tag {t.Tag}, count = {HashTagCounts[t.Tag]}");
            }
            else
            {
                HashTagCounts.Add(t.Tag, 1);
                _logger.Info($"New tag {t.Tag}, count = {HashTagCounts[t.Tag]}");
            }
        }

        SampleTweet.Log(tweet, _logger);
    }

    private IEnumerable<KeyValuePair<string, int>> GetTrendingHashTags()
    {
        var topHashTags = (
                        from kv in HashTagCounts
                        orderby kv.Value descending
                        select kv
                        ).Take(NumTopHashTags);

        return topHashTags;
    }

    private void SaveTrendingHashTags(IEnumerable<KeyValuePair<string, int>> sortedKVPairs)
    {
        string fileName = "../Data/TrendingHashTags.json";
        var trendingHashTags = new TrendingHashTags();

        int rank = 0;
        foreach (var p in sortedKVPairs)
        {
            var hc = new HashTag {
                Rank = ++rank,
                Tag = p.Key,
                Tweets = p.Value
            };
            trendingHashTags.TopTags?.Add(hc);
        }

        trendingHashTags.SampleSize = TotalTweets;

        var options = new JsonSerializerOptions { IncludeFields = true };
        string jsonString = JsonSerializer.Serialize(trendingHashTags, options);
        File.WriteAllText(fileName, jsonString);
    }

    private void DisplayTrendingHashTags(IEnumerable<KeyValuePair<string, int>> trendingHashTags)
    {
        System.Console.WriteLine($"\n==============================================================================\n");
        System.Console.WriteLine($"Total sampled tweets: {TotalTweets}\n");
        System.Console.WriteLine($"Top 10 Hashtags:\n");
        int i = 0;
        foreach (var p in trendingHashTags)
        {
            System.Console.WriteLine(string.Format("  {0, 3}: #{1, -50} {2, 8} tweets.", ++i, p.Key?.Trim(), p.Value));
        }
    }

    #region Fileds

    public string? Token { get; set; }
    private int NumTopHashTags { get; set; }
    private int TotalTweets  { get; set; }

    private SortedDictionary<string, int> HashTagCounts = new SortedDictionary<string, int>();
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
    private readonly Uri StreamV2Uri = new Uri("https://api.twitter.com/2/tweets/sample/stream?tweet.fields=created_at&expansions=author_id&user.fields=created_at");

    #endregion
}

