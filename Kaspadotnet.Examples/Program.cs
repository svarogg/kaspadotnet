// See https://aka.ms/new-console-template for more information


using Grpc.Core;
using Grpc.Net.Client;
using Kaspadotnet.Examples;
using Protowire;
using Kaspawalletd;

Console.WriteLine("Please enter password");
var password = Console.ReadLine();


var examples = new RpcExamples();
await examples.NewAddressExample();
await examples.GetBalanceExample();
await examples.GetUTXOsByAddressExample();
if (password != null)
{
    await examples.SendTransaction(password);
}
