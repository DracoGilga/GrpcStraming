using Grpc.Core;
using Google.Protobuf;

public class AudioServicer : AudioService.AudioServiceBase 
{
    private const int ChunkSize = 1024;

    public override async Task downloadAudio (DownloadFileRequest request, 
        IServerStreamWriter<DataChunkResponse> responseStream, 
        ServerCallContext context)
    {
        var buffer = new byte[ChunkSize];
        await using var fileStream = 
            File.OpenRead($"recursos\\{request.Nombre}");
        var numBytesRead = 0;

        Console.WriteLine($"\n\nEnviado el archivo: {request.Nombre}");

        try
        {
            while ((numBytesRead = await fileStream.ReadAsync(buffer)) > 0)
            {
                await responseStream.WriteAsync(new DataChunkResponse
                {
                    Data = UnsafeByteOperations
                        .UnsafeWrap(buffer.AsMemory(0,numBytesRead))
                });
                Console.Write(".");
            }
        }
        catch{}
    }
}