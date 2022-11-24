using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TweetSampleModel;

namespace TrendingTweetsAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class TrendingHashTagsController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<TrendingHashTagsController> _logger;

    public TrendingHashTagsController(ILogger<TrendingHashTagsController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<HashTag> Get()
    {

        string fileName = "../Data/TrendingHashTags.json";
        string jsonString = System.IO.File.ReadAllText(fileName);
        var options = new JsonSerializerOptions { IncludeFields = true };
        var trendingHashTags = JsonSerializer.Deserialize<TrendingHashTags>(jsonString, options)!;
        List<HashTag> list = trendingHashTags.TopTags!;
        return list.ToArray();
    }
}

