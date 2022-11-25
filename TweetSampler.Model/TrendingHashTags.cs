using System.Collections;

namespace TweetSampler.Model;

public class TrendingHashTags   
{
    public List<HashTag>? TopTags { get; set; }

    public TrendingHashTags()
    {
        TopTags = new List<HashTag>();
    }
}
