using Google.Protobuf;
using Google.Protobuf.Collections;
using Kaspadontnet.Clients;
using Kaspawalletd;

namespace Kaspadotnet.Examples;

public partial class RpcExamples
{
    public async Task SendTransactionStepByStepExample(string password)
    {
        var unsignedTransactions = await createUnsignedTransactions();
        var signedTransactions = await signTransactions(unsignedTransactions, password);
        await broadcastTransactions(signedTransactions);
    }
    private async Task<RepeatedField<ByteString>> createUnsignedTransactions()
    {
        using var kaspawalletClient = new KaspawalletdClient();
        var createUnsignedTransactionRequest = new CreateUnsignedTransactionsRequest
        {
            Address = "kaspadev:qr76zfj027fqgwsnx95rlsf2tch45gfat9sls07c58qevfnp6s4mu0cmyjeq7",   
            Amount = 1000,  // Amount is in sompi, a.k.a. 0.00000001KAS
        };
        var createUnsignedTransactionResponse = await kaspawalletClient.Client.CreateUnsignedTransactionsAsync(
            createUnsignedTransactionRequest);

        return createUnsignedTransactionResponse.UnsignedTransactions;
    }

    private async Task<RepeatedField<ByteString>> signTransactions(RepeatedField<ByteString> unsignedTransactions, string password)
    {
        using var kaspawalletClient = new KaspawalletdClient();

        var signRequest = new SignRequest
        {
            Password = password
        };
        signRequest.UnsignedTransactions.Add(unsignedTransactions);

        var signResponse = await kaspawalletClient.Client.SignAsync(signRequest);
        return signResponse.SignedTransactions;
    }

    private async Task broadcastTransactions(RepeatedField<ByteString> signedTransactions )
    {

        using var kaspawalletClient = new KaspawalletdClient();
        
        var broadcastRequest = new BroadcastRequest();
        broadcastRequest.Transactions.Add(signedTransactions);
        
        var broadcastResponse = await kaspawalletClient.Client.BroadcastAsync(broadcastRequest);
        Console.WriteLine("Transactions sent:");
        Console.WriteLine(broadcastResponse.TxIDs);
    }
}