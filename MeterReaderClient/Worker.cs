using Grpc.Net.Client;
using MeterReaderWeb.Services;

namespace MeterReaderClient
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IReadingFactory _readingFactory;
        private MeterReadingService.MeterReadingServiceClient? _meterReadingServiceClient = null;
        protected MeterReadingService.MeterReadingServiceClient MeterReadingServiceClient
        {
            get
            {
                if (_meterReadingServiceClient == null)
                {
                    var channel = GrpcChannel.ForAddress(_configuration.GetValue<string>("Service:ServerUrl")!);
                    _meterReadingServiceClient = new MeterReadingService.MeterReadingServiceClient(channel);
                }
                return _meterReadingServiceClient;
            }
        }
        public Worker(ILogger<Worker> logger, IConfiguration configuration, IReadingFactory readingFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _readingFactory = readingFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                var customerId = _configuration.GetValue<int>("Service:CustomerId");
                var pkt = new ReadingPacket()
                {
                    Successful = ReadingStatus.Success,
                    Notes = "This is a test packet"
                };

                for (int i = 0; i < 5; i++)
                {
                    pkt.Readings.Add(_readingFactory.Generate(customerId).Result);
                }

                await MeterReadingServiceClient.AddReadingAsync(pkt);
                await Task.Delay(_configuration.GetValue<int>("Service:DelayInterval"), stoppingToken);
            }
        }
    }
}