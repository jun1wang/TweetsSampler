using TweetSampler.Core;

namespace TweetSampler.Core.Tests;

public class SampleStreamProcessorTests
{

    [Fact]
    public void ThrowsExceptioWhenBearTokenIsEmpty ()
    {
        var processor = new SampleStreamProcessor("");
        Assert.Throws<ArgumentNullException>(() => processor.Token);
    }
}
