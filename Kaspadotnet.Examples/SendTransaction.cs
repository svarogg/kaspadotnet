using Google.Protobuf;
using Kaspadontnet.Clients;
using Kaspawalletd;

namespace Kaspadotnet.Examples;

public partial class RpcExamples
{
    private const char hexTransactionsSeparator = '_';
    public async Task CreateUnsignedTransactionExample()
    {
        using var kaspawalletClient = new KaspawalletdClient();
        var createUnsignedTransactionRequest = new CreateUnsignedTransactionsRequest
        {
            Address = "kaspadev:qr76zfj027fqgwsnx95rlsf2tch45gfat9sls07c58qevfnp6s4mu0cmyjeq7",   
            Amount = 1000,  // Amount is in sompi, a.k.a. 0.00000001KAS
        };
        var createUnsignedTransactionResponse = await kaspawalletClient.Client.CreateUnsignedTransactionsAsync(
            createUnsignedTransactionRequest);
        
        // Now we need to encode the transactions in a way that `kaspawallet sign can read`
        // Note that if auto-compound is required - there will be multiple transactions, we need to separate them with a `_`
        var hexTransactions = 
            from transaction in createUnsignedTransactionResponse.UnsignedTransactions
            select Convert.ToHexString(transaction.ToByteArray());
        var hexTransactionsString = String.Join(hexTransactionsSeparator, hexTransactions);

        Console.WriteLine("Unsigned Transaction:");
        Console.WriteLine(hexTransactionsString);
        // Now you need to manually use `kaspawallet [--devnet/--testnet] sign --transaction [hexTransactionsString]`
    }

    public async Task BroadcastTransaction()
    {
        var signedTransactionsString = // This is the output of `kaspawallet sign ...`
            "0a9e01122a0a260a220a20069cd8159cc695ae91b40dd20e1e024eb2232456b1a257de660992f70dd7e4321005" +
            "20011a2908e80712240a2220fda1264f5792043a1331683fc12a5e2f5a213d5961f83fd8a1c1962661d42bbeac" +
            "1a2d088892e198cb5e12240a22201bfe9976a6eb3cd19acb5f00aa3439c346791b6b7312a12d5d4b843a4c3446" +
            "bbac2a160a14000000000000000000000000000000000000000012ef01122d0880e8e198cb5e12240a2220497a" +
            "8b47521f6f356ed77f3b94116af9bce1b4ae9fd59c6f576cef9cb9d3bd6bac180122b4010a6f6b647562354163" +
            "695561377346734c6245416d3733503463346f394372735263546177415a7166546271597767547932557a4567" +
            "41796a6b67714b4251683558447852727548545078634757743341384e557a525648624e324a5537775061706a" +
            "426771744572505a4d6f666235351241aba5f3b1f7ceb84087a4a2288ef9202d8dc3b78b6d2c0231506d204aa7" +
            "e327a9ec0cff5868e0fcd9c50f10984e3da8e21a85f643f0bd04c2a5cf9458c1397979012a056d2f302f33";
        var signedTransactions = signedTransactionsString.Split(hexTransactionsSeparator);
        var signedTransactionsBytes =
            from signedTransaction in signedTransactions
            select Convert.FromHexString(signedTransaction);

        using var kaspawalletClient = new KaspawalletdClient();
        foreach (var signedTransactionBytes in signedTransactionsBytes)
        {
            var broadcastRequest = new BroadcastRequest
            {
                Transaction = ByteString.CopyFrom(signedTransactionBytes)
            };
            var broadcastResponse = await kaspawalletClient.Client.BroadcastAsync(broadcastRequest);
            Console.WriteLine("Transaction sent:");
            Console.WriteLine(broadcastResponse.TxID);
        }

    }
}