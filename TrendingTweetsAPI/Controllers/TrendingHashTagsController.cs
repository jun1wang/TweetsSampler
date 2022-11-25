using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TweetSampler.Model;

namespace TweetSampler.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TrendingHashTagsController : ControllerBase
{
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

