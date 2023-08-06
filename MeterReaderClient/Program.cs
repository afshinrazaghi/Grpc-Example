using MeterReaderClient;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddScoped<IReadingFactory,ReadingFactory>();
        services.AddHostedService<Worker>();
    })
    .Build();



host.Run();
