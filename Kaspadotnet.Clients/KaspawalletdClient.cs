using Grpc.Net.Client;
using Kaspawalletd;
using Protowire;

namespace Kaspadontnet.Clients;

public class KaspawalletdClient:IDisposable
{
    public kaspawalletd.kaspawalletdClient Client { get; private set; }
    
    private GrpcChannel channel;
    
    public KaspawalletdClient(string url = "http://localhost:8082")  
    {
        channel = GrpcChannel.ForAddress(url);
        Client = new kaspawalletd.kaspawalletdClient(channel);
        
    }

    public void Dispose()
    {
        channel.Dispose();
    }
}