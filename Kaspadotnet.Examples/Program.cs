// See https://aka.ms/new-console-template for more information


using Grpc.Core;
using Grpc.Net.Client;
using Kaspadotnet.Examples;
using Protowire;
using Kaspawalletd;

var examples = new RpcExamples();

Console.WriteLine("Please enter password");
var password = Console.ReadLine();

await examples.NewAddressExample();
await examples.GetBalanceExample();
await examples.GetUTXOsByAddressExample();
if (password != null)
{
    await examples.SendTransaction(password);
}

// Commented out, because the following methods block.
// await examples.SubscribeToUTXOUpdatesExample();
// await examples.FollowTransactionsExample();