// See https://aka.ms/new-console-template for more information


using Grpc.Core;
using Grpc.Net.Client;
using Kaspadotnet.Examples;
using Protowire;
using Kaspawalletd;

//async Task kaspadExample()
//{
//    using var messageStream = kaspadClient.MessageStream();
//    var request = new GetBlockDagInfoRequestMessage();
//    var message = new KaspadMessage {GetBlockDagInfoRequest = request};
//    await messageStream.RequestStream.WriteAsync(message);
//    await messageStream.ResponseStream.MoveNext();
//    Console.WriteLine(messageStream.ResponseStream.Current);
//}

var examples = new RPCExamples();
await examples.NewAddressExample();
