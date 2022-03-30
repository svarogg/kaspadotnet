// See https://aka.ms/new-console-template for more information


using Grpc.Core;
using Grpc.Net.Client;
using Protowire;
using Kaspawalletd;

async Task kaspadExample()
{
    using var kaspadChannel = GrpcChannel.ForAddress("http://localhost:16610");
    var kaspadClient = new RPC.RPCClient(kaspadChannel);
    using var messageStream = kaspadClient.MessageStream();
    var request = new GetBlockDagInfoRequestMessage();
    var message = new KaspadMessage {GetBlockDagInfoRequest = request};
    await messageStream.RequestStream.WriteAsync(message);
    await messageStream.ResponseStream.MoveNext();
    Console.WriteLine(messageStream.ResponseStream.Current);
}

async Task kaspawalletExample()
{
    using var kaspawalletChannel = GrpcChannel.ForAddress("http://localhost:8082");
    var kaspawalletClient = new kaspawalletd.kaspawalletdClient(kaspawalletChannel);
    var getBalanceRequest = new GetBalanceRequest();
    var getBalanceResponse = await kaspawalletClient.GetBalanceAsync(getBalanceRequest);
    Console.WriteLine(getBalanceResponse);
}

await kaspadExample();
await kaspawalletExample();