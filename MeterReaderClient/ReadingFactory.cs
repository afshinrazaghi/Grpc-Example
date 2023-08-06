using Google.Protobuf.WellKnownTypes;
using MeterReaderWeb.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterReaderClient
{
    public class ReadingFactory: IReadingFactory
    {
        private readonly ILogger<ReadingFactory> _logger;

        public ReadingFactory(ILogger<ReadingFactory> logger)
        {
            _logger = logger;
        }

        public Task<ReadingMessage> Generate(int customerId)    
        {
            var readingMessage = new ReadingMessage()
            {
                CustomerId = customerId,
                ReadingTime = Timestamp.FromDateTime(DateTime.UtcNow),
                ReadingValue = new Random().Next(10000)
            };

            return Task.FromResult(readingMessage); 
        }
    }
}
