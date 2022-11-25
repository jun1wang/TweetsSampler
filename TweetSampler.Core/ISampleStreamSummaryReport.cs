using Tweetinvi.Events.V2;
using Tweetinvi.Models.V2;

namespace TweetSampler.Core
{
    public interface ISampleStreamSummaryReport
    {
        void DisplayTrendingHashTags(IEnumerable<KeyValuePair<string, int>> trendingHashTags, int totalTweets);
    }
}