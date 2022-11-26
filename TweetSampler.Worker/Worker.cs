using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tweetinvi.Exceptions;
using TweetSampler.Core;

namespace TweetSampler.Worker;

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
            try
            {
                var bearerToken = _options.Value.TwitterBearToken!;
                var language = _options.Value.Langage!;

                SampleStreamProcessorBase processor;

                processor = (language == "English") ?
                        new SampleStreamProcessorWithFilter(bearerToken, language) :
                        new SampleStreamProcessorDefault(bearerToken);

                await processor.Run();
            }
            catch (TwitterInvalidCredentialsException e)
            {
                _logger.LogError($"Twitter credentials provided is invalid: {e.ToString()}");
            }
            catch (TwitterException e)
            {
                _logger.LogError($"Twitter internal error: {e.ToString()}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Twitter internal error: {e.ToString()}");
            }
        }
    }
}

