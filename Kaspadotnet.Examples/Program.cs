// See https://aka.ms/new-console-template for more information


using Grpc.Core;
using Grpc.Net.Client;
using Kaspadotnet.Examples;
using Protowire;
using Kaspawalletd;

var examples = new RpcExamples();
await examples.NewAddressExample();
await examples.GetBalanceExample();
await examples.GetUTXOsByAddressExample();
