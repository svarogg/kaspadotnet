using Google.Protobuf;
using Google.Protobuf.Collections;
using Kaspadontnet.Clients;
using Kaspawalletd;

namespace Kaspadotnet.Examples;

public partial class RpcExamples
{

    public async Task SendTransaction(string password)
    {
        using var kaspawalletClient = new KaspawalletdClient();
        var sendRequest = new SendRequest()
        {
            ToAddress = "kaspadev:qr76zfj027fqgwsnx95rlsf2tch45gfat9sls07c58qevfnp6s4mu0cmyjeq7",   
            Amount = 1000,  // Amount is in sompi, a.k.a. 0.00000001KAS
            Password = password
        };
        var sendResponse = await kaspawalletClient.Client.SendAsync(sendRequest);
        Console.WriteLine("Transactions sent:");
        Console.WriteLine(sendResponse.TransactionIDs);
    }
}