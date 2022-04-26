using Grpc.Core;
using Kaspadontnet.Clients;
using Kaspawalletd;
using Protowire;

namespace Kaspadotnet.Examples;

public partial class RpcExamples
{
    public async Task SubscribeToUTXOUpdatesExample()
    {
        using var kaspawalletClient = new KaspawalletdClient();
        // Using port 16610 for devnet. Use 16210 for testnet, or don't pass a url for mainnet
        using var kaspadClient = new KaspadClient("http://localhost:16610");
        
        
        var showAddressesRequest = new ShowAddressesRequest{};
        var showAddressesResponse = await kaspawalletClient.Client.ShowAddressesAsync(showAddressesRequest);
        
        Console.WriteLine($"Got {showAddressesResponse.Address.Count} Addresses");

        using var messageStream = kaspadClient.Client.MessageStream();

        var notifyRequestMessage = new KaspadMessage
        {
            NotifyUtxosChangedRequest = new NotifyUtxosChangedRequestMessage
            {
                Addresses = {showAddressesResponse.Address}
            }
        };
        await messageStream.RequestStream.WriteAsync(notifyRequestMessage);
        await messageStream.ResponseStream.MoveNext();
        var response = messageStream.ResponseStream.Current;
        if (response.NotifyUtxosChangedResponse.Error != null)
        {
            Console.WriteLine($"Error when subscribing to notifications: {response.NotifyUtxosChangedResponse.Error}");
            return;
        }

        Console.WriteLine("Subsribed to notifications, awaiting messages...");
        while (true)
        {
            await messageStream.ResponseStream.MoveNext();
            var message = messageStream.ResponseStream.Current;

            if (message.PayloadCase != KaspadMessage.PayloadOneofCase.UtxosChangedNotification)
            {
                Console.WriteLine($"Skipping message of type {message.PayloadCase}");
            }
            Console.WriteLine(message.UtxosChangedNotification);
        }
    }
}