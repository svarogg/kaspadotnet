using Kaspadontnet.Clients;
using Kaspawalletd;

namespace Kaspadotnet.Examples;

public partial class RpcExamples
{
    public async Task NewAddressExample()
    {
        using var kaspawalletClient = new KaspawalletdClient();
        var newAddressRequest = new NewAddressRequest();
        var newAddressResponse = await kaspawalletClient.Client.NewAddressAsync(newAddressRequest);
        
        Console.WriteLine("New Address:");
        Console.WriteLine(newAddressResponse.Address);
    }
}