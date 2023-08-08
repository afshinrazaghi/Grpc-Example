import grpc;
import MeterReader_pb2 as MeterReader
import MeterReader_pb2_grpc as MeterReaderService
from google.protobuf.timestamp_pb2 import Timestamp

import enums_pb2 as Enums

def main():
    print ("call grpc service from python client")

    with open("localhost.pem",'rb') as file:
        cert = file.read()

    credentials = grpc.ssl_channel_credentials(cert)
    channel = grpc.secure_channel("localhost:5001",credentials)
    stub = MeterReaderService.MeterReadingServiceStub(channel)
    request = MeterReader.ReadingPacket(successful = Enums.ReadingStatus.SUCCESS)
    now = Timestamp()
    now.GetCurrentTime()
    reading = MeterReader.ReadingMessage(customerId = 1, readingValue = 1,  readingTime = now)

    request.readings.append(reading)
    result = stub.AddReading(request)
    if(result.success == Enums.ReadingStatus.SUCCESS):
        print("success")
    else:
        print ("failure")
    
main()

