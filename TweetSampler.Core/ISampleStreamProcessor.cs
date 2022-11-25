using Tweetinvi.Events.V2;
using Tweetinvi.Models.V2;

namespace TweetSampler.Core
{
    public interface ISampleStreamProcessor
    {
        string? Token { get; set; }
        Task Run();
    }
}