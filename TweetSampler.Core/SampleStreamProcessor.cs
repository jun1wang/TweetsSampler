
using System.Text.Json;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Models.V2;
using System.Text.Json.Serialization;
using TweetSampler.Model;
using Tweetinvi.Events.V2;

namespace TweetSampler.Core;

public class SampleStreamProcessor : ISampleStreamProcessor
{
    public SampleStreamProcessor(string token, int numTopHashTags = 10)
    {
        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentNullException("Beare token cannot be empty!");
        }
        Token = token;
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
                ProcessSampleTweet(args);
            };

            await sampleStreamV2.StartAsync();
        }
        catch (HttpRequestException e)
        {
            _logger.Error("\nException Caught!");
            _logger.Error("Message :{0} ", e.Message);
        }
    }

    private void ProcessSampleTweet(TweetV2ReceivedEventArgs args)
    {
        DetectHashTags(args.Tweet);

        if (TotalTweets % 100 == 0)
        {
            var topHashTags = GetTrendingHashTags();

            var reporter = new SampleStreamSummaryReport();
            reporter.DisplayTrendingHashTags(topHashTags, TotalTweets);

            var saver = new SampleStreamSaver();
            saver.SaveToFileAsJson(topHashTags);
        }
    }

    private void DetectHashTags(TweetV2 tweet)
    {
        ++TotalTweets;

        if (SampleTweet.HasNoHashTags(tweet))
            return; // ignore (no hashtag)

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

    #region Fileds

    public string? Token { get; set; }
    private int NumTopHashTags { get; set; }
    private int TotalTweets { get; set; }

    private SortedDictionary<string, int> HashTagCounts = new SortedDictionary<string, int>();
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
    private readonly Uri StreamV2Uri = new Uri("https://api.twitter.com/2/tweets/sample/stream?tweet.fields=created_at&expansions=author_id&user.fields=created_at");

    #endregion
}

