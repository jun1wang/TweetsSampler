using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TweetProcessor;

namespace TweetSampleWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    IOptions<TweetSamplerConfiguration> _options;

    public Worker(ILogger<Worker> logger, IOptions<TweetSamplerConfiguration> options)
    {
        _logger = logger;
        _options = options;
        
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            var processor = new Processor(_options.Value.TwitterBearToken);

            await processor.Run();
        }
    }
}

