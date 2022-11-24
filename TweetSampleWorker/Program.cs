using TweetSampleWorker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        services.Configure<TweetSamplerConfiguration>(configuration.GetSection(nameof(TweetSamplerConfiguration)));

        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();

