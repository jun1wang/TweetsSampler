using TweetSampler.Core;

namespace TweetSampler.Core.Tests;

public class SampleStreamProcessorTests
{

    [Fact]
    public void ThrowsExceptioWhenBearTokenIsEmpty()
    {
        var processor = new SampleStreamProcessorDefault("");
        Assert.Throws<ArgumentNullException>(() => processor);
    }
}
