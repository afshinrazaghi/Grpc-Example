using Grpc.Core;
using Grpc.Net.Client;
using MeterReaderWeb.Services;

namespace MeterReaderClient
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly ReadingFactory _readingFactory;
        private MeterReadingService.MeterReadingServiceClient? _meterReadingServiceClient = null;
        private string _token = string.Empty;
        private DateTime _expiration = DateTime.MinValue;
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
        public Worker(ILogger<Worker> logger, IConfiguration configuration, ReadingFactory readingFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _readingFactory = readingFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var customerId = _configuration.GetValue<int>("Service:CustomerId");
            //int counter = 0;

            if (!NeedLogin() || await GenerateToken())
            {
                var headers = new Metadata();
                headers.Add("Authorization", $"Bearer {_token}");
                while (!stoppingToken.IsCancellationRequested)
                {
                    //counter++;
                    //if (counter % 10 == 0)
                    //{
                    //    Console.WriteLine("Send Diagnostics");
                    //    var stream = MeterReadingServiceClient.SendDiagnostics();
                    //    for (var x = 0; x < 5; x++)
                    //    {
                    //        var reading = await _readingFactory.Generate(customerId);
                    //        await stream.RequestStream.WriteAsync(reading);
                    //    }

                    //    await stream.RequestStream.CompleteAsync();
                    //}

                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    var pkt = new ReadingPacket()
                    {
                        Successful = ReadingStatus.Success,
                        Notes = "This is a test packet"
                    };

                    for (int i = 0; i < 5; i++)
                    {
                        pkt.Readings.Add(await _readingFactory.Generate(customerId));
                    }

                    await MeterReadingServiceClient.AddReadingAsync(pkt, headers: headers);
                    await Task.Delay(_configuration.GetValue<int>("Service:DelayInterval"), stoppingToken);
                }
            }
        }

        private bool NeedLogin() => string.IsNullOrEmpty(_token) || _expiration < DateTime.UtcNow;
        private async Task<bool> GenerateToken()
        {
            string userName = _configuration.GetValue<string>("Service:UserName")!;
            string password = _configuration.GetValue<string>("Service:Password")!;
            var res = await MeterReadingServiceClient.CreateTokenAsync(new TokenRequest
            {
                UserName = userName,
                Password = password
            });
            if (res.Success)
            {
                _token = res.Token;
                _expiration = res.Expiration.ToDateTime();
                return true;
            }
            _token = string.Empty;
            _expiration = DateTime.MinValue;
            return false;
        }
    }
}