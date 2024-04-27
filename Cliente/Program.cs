using Grpc.Core;
using Grpc.Net.Client;
using System.Media;
using System.Net;
using static AudioService;

static async Task<MemoryStream> descargaStreamAssync(AudioServiceClient stub, string nombre_archivo)
{
    using var call = stub.downloadAudio(new DownloadFileRequest
    {
        Nombre = nombre_archivo
    });

    Console.WriteLine($"Recibiendo el archivo {nombre_archivo}...");
    var writeStream = new MemoryStream();
    await foreach (var message in call.ResponseStream.ReadAllAsync())
    {
        if (message.Data.Length != null)
        {
            var bytes = message.Data.Memory;
            Console.Write(".");
            await writeStream.WriteAsync(bytes);
        }
    }

    Console.WriteLine("\n Recepcion de datos correcta.\n\n");
    return writeStream;
}

static void playStream(MemoryStream stream, string nombre_archivo)
{
    if (stream != null)
    {
        Console.WriteLine($"Reproduciendo el archivo {nombre_archivo}...");
        SoundPlayer player = new(stream);
        player.Stream?.Seek(0, SeekOrigin.Begin);
        player.Play();

    }
}

//establecer el servidor grpc
using var channel = GrpcChannel.ForAddress("http://localhost:8080");
//creaa el canal de comunicacion
AudioServiceClient stub = new(channel);

string nombre_archivo = "sample.wav";
//descarga el stream
MemoryStream stream = await descargaStreamAssync(stub, nombre_archivo);
//reproduce el stream
playStream(stream, nombre_archivo);

Console.WriteLine("Presione cualquier  tecla para terminar el programa...");
stream.Close();
Console.WriteLine("Apagando...");
channel.ShutdownAsync().Wait();