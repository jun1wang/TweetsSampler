
using System.Text.Json;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Models.V2;
using System.Text.Json.Serialization;
using TweetSampler.Model;
using Tweetinvi.Events.V2;

namespace TweetSampler.Core;

public abstract class SampleStreamProcessorBase
{
    public SampleStreamProcessorBase(string token, int numTopHashTags = 10)
    {
        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentNullException("Beare token cannot be empty!");
        }
        Token = token;
        NumTopHashTags = numTopHashTags;
        CreateTwitterClient();
    }

    private void CreateTwitterClient()
    {
        var credentials = new TwitterCredentials
        {
            BearerToken = Token
        };

        Client = new TwitterClient(credentials); 
    }

    public async Task Run()
    {
        try
        {
            var sampleStreamV2 = Client?.StreamsV2.CreateSampleStream()!;

            sampleStreamV2.TweetReceived += (sender, args) =>
            {
                ProcessSampleTweet(args);
            };

            await sampleStreamV2.StartAsync();
        }
        catch (HttpRequestException e)
        {
            _logger.Error("\nException Caught! :{0} ", e.Message);
        }
    }

    protected void ProcessSampleTweet(TweetV2ReceivedEventArgs args)
    {
        var tagDetected = DetectHashTags(args.Tweet) ;

        if (!tagDetected) return;

        if (TotalTweets % 100 == 0)
        {
            var topHashTags = GetTrendingHashTags();

            var reporter = new SampleStreamSummaryReport();
            reporter.DisplayTrendingHashTags(topHashTags, TotalTweets);

            var saver = new SampleStreamSaver();
            saver.SaveToFileAsJson(topHashTags);
        }
    }

    private bool DetectHashTags(TweetV2 tweet)
    {
        ++TotalTweets;

        if (Ignorable(tweet))
            return false;

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

        return true;
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
    

    protected abstract bool Ignorable(TweetV2 tweet);


    #region Fileds

    protected TwitterClient? Client { get; set; }
    protected int NumTopHashTags { get; set; }
    protected int TotalTweets { get; set; }

    protected SortedDictionary<string, int> HashTagCounts = new SortedDictionary<string, int>();
    protected static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    private string? Token { get; set; }
    private readonly Uri StreamV2Uri = new Uri("https://api.twitter.com/2/tweets/sample/stream?tweet.fields=created_at&expansions=author_id&user.fields=created_at");

    #endregion
}

