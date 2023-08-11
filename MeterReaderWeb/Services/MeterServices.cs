using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MeterReaderLib;
using MeterReaderLib.Models;
using MeterReaderWeb.Data;
using MeterReaderWeb.Data.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MeterReaderWeb.Services
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MeterServices : MeterReadingService.MeterReadingServiceBase
    {
        private readonly ILogger<MeterServices> _logger;
        private readonly IReadingRepository _repository;
        private readonly JwtTokenValidationService _jwtValidationService;

        public MeterServices(ILogger<MeterServices> logger, IReadingRepository repository, JwtTokenValidationService jwtValidationService)
        {
            _logger = logger;
            _repository = repository;
            _jwtValidationService = jwtValidationService;
        }

        public override async Task<Empty> SendDiagnostics(IAsyncStreamReader<ReadingMessage> requestStream, ServerCallContext context)
        {
            var taskRun = Task.Run(async () =>
            {
                await foreach (var reading in requestStream.ReadAllAsync())
                {
                    _logger.LogInformation("Current Read Value From Stream is " + reading.ReadingValue);
                }
            });

            await taskRun;
            return new Empty();
        }

        [AllowAnonymous]
        public override async Task<TokenResponse> CreateToken(TokenRequest request, ServerCallContext context)
        {
            var credentialModel = new CredentialModel
            {
                UserName = request.UserName,
                Passcode = request.Password
            };

            var res = await _jwtValidationService.GenerateTokenModelAsync(credentialModel);
            if (res.Success)
            {
                return new TokenResponse
                {
                    Success = true,
                    Token = res.Token,
                    Expiration = Timestamp.FromDateTime(res.Expiration),
                };
            }
            return new TokenResponse
            {
                Success = false
            };
        }

        public override async Task<StatusMessage> AddReading(ReadingPacket request, ServerCallContext context)
        {
            var result = new StatusMessage
            {
                Success = ReadingStatus.Failure
            };

            if (request.Successful == ReadingStatus.Success)
            {
                _logger.LogInformation("AddReading Method Called " + JsonConvert.SerializeObject(request));

                try
                {
                    foreach (var r in request.Readings)
                    {
                        // save reading to database
                        var reading = new MeterReading()
                        {
                            Value = r.ReadingValue,
                            ReadingDate = r.ReadingTime.ToDateTime(),
                            CustomerId = r.CustomerId
                        };

                        _repository.AddEntity(reading);
                    }

                    if (await _repository.SaveAllAsync())
                    {
                        result.Success = ReadingStatus.Success;
                    }
                }
                catch (Exception e)
                {
                    result.Message = "Exception thrown during process";
                    _logger.LogError($"Exception thrown during saving of readings: {e}");
                }
            }


            return result;
        }
    }
}
