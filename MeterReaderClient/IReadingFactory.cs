using MeterReaderWeb.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeterReaderClient
{
    public interface IReadingFactory
    {
        Task<ReadingMessage> Generate(int customerId);
    }
}
