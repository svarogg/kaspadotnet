using Grpc.Net.Client;
using Kaspawalletd;
using Protowire;

namespace Kaspadontnet.Clients;

public class KaspadClient : RPC.RPCClient, IDisposable
{
    public RPC.RPCClient Client { get; private set; }
    
    private GrpcChannel channel;
    
    public KaspadClient(string url = "http://localhost:16110")  
    {
        channel = GrpcChannel.ForAddress(url);
        Client = new RPC.RPCClient(channel);
    }

    public void Dispose()
    {
        channel.Dispose();
    }
}

public static class Clients
{
    static kaspawalletd.kaspawalletdClient KaspawalletdClient(string url = "http://localhost:8082")
    {
        using var kaspawalletChannel = GrpcChannel.ForAddress(url);
        return  new kaspawalletd.kaspawalletdClient(kaspawalletChannel);
    }
}