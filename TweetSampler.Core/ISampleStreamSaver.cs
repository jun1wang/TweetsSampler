using Tweetinvi.Events.V2;
using Tweetinvi.Models.V2;

namespace TweetSampler.Core
{
    public interface ISampleStreamSaver
    {
        void SaveToFileAsJson(IEnumerable<KeyValuePair<string, int>> topHashTags);
        void SaveToDatabase(IEnumerable<KeyValuePair<string, int>> topHashTags);
    }
}