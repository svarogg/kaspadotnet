using Kaspadontnet.Clients;
using Kaspawalletd;

namespace Kaspadotnet.Examples;

public partial class RpcExamples
{
    public async Task GetBalanceExample()
    {
        using var kaspawalletClient = new KaspawalletdClient();
        var getBalanceRequest = new GetBalanceRequest{};
        var getBalanceResponse = await kaspawalletClient.Client.GetBalanceAsync(getBalanceRequest);
        
        
        Console.WriteLine("Balance:");
        Console.WriteLine(getBalanceResponse);
    }
}