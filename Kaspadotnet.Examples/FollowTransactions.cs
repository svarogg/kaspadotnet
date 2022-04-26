using System.Text.Json;
using Grpc.Core;
using Kaspadontnet.Clients;
using Protowire;

namespace Kaspadotnet.Examples;

public partial class RpcExamples
{
    private class TransactionWithData
    {
        internal RpcTransaction transaction;
        internal string? acceptingBlockHash;

        public TransactionWithData(RpcTransaction transaction)
        {
            this.transaction = transaction;
        }
    }

    private class FollowTransactionsState
    {
        internal readonly Dictionary<string, TransactionWithData> TransactionsById;
        internal readonly Dictionary<string, List<TransactionWithData>> TransactionsByAcceptingBlockHash;

        public FollowTransactionsState()
        {
            TransactionsById = new Dictionary<string, TransactionWithData>();
            TransactionsByAcceptingBlockHash = new Dictionary<string, List<TransactionWithData>>();
        }
    }

    public async Task FollowTransactionsExample()
    {
        var messageStream = await SetupFollowTransactionsExample();
        // You might want to instantiate the state with some data already.
        // You might want to call GetVirtualSelectedParentChainFromBlock with includeAcceptedTransactionIds = true
        // And startHash = the last chain block you processed. 
        // and handle the response similar to how we handle VirtualSelectedParentChainChangedNotification
        var state = new FollowTransactionsState();

        while (true)
        {
            await messageStream.ResponseStream.MoveNext();
            var message = messageStream.ResponseStream.Current;

            Console.WriteLine($"Got message of type {message.PayloadCase}:\n{message}");
            switch (message.PayloadCase)
            {
                // It is important to always first handle the BlockAdded notifications, since blocks might include
                // unknown transactions that would be mentioned in the VirtualSelectedParentChainChanged notification.
                case KaspadMessage.PayloadOneofCase.BlockAddedNotification: 
                    handleBlockAddedNotification(message.BlockAddedNotification, state);
                    break;

                case KaspadMessage.PayloadOneofCase.VirtualSelectedParentChainChangedNotification:
                    handleVirtualSelectedParentChainChangedNotification(
                        message.VirtualSelectedParentChainChangedNotification, state);
                    break;
                default:
                    Console.WriteLine($"Skipping message of type {message.PayloadCase}");
                    break;
            }
            Console.WriteLine($"State after message:\n Transactions: {JsonSerializer.Serialize(state.TransactionsById)}");
        }
    }

    private void handleBlockAddedNotification(BlockAddedNotificationMessage blockAddedNotification,
        FollowTransactionsState state)
    {
        foreach (var transaction in blockAddedNotification.Block.Transactions)
        {
            // At this stage you might want to filter out transactions that are not relevant to you
            
            var transactionId = transaction.VerboseData.TransactionId;
            state.TransactionsById[transactionId] = new TransactionWithData(transaction);
        }
    }

    private void handleVirtualSelectedParentChainChangedNotification(
        VirtualSelectedParentChainChangedNotificationMessage virtualSelectedParentChainChangedNotification, 
        FollowTransactionsState state)
    {
        var removedChainBlockHashes = virtualSelectedParentChainChangedNotification.RemovedChainBlockHashes;
        foreach (var removedChainBlockHash in removedChainBlockHashes)
        {
            // removedChainBlockHashes would be non-empty during re-orgs.
            // Shallow re-orgs are happen all the time in a DAG @ 1 block / second, so this is very importasnt
            // to "unaccept" all transactions that were accepted by blocks that were re-orged out.
            // Note that most transactions will later be re-accepted by another block.

            foreach (var transaction in state.TransactionsByAcceptingBlockHash[removedChainBlockHash])
            {
                transaction.acceptingBlockHash = null;
            }
            state.TransactionsByAcceptingBlockHash.Remove(removedChainBlockHash);
        }

        var acceptedTransactionIds = virtualSelectedParentChainChangedNotification.AcceptedTransactionIds;
        foreach (var acceptedTransactionIdsWithBlock in acceptedTransactionIds)
        {
            var acceptingBlockHash = acceptedTransactionIdsWithBlock.AcceptingBlockHash;
            state.TransactionsByAcceptingBlockHash[acceptingBlockHash] = new List<TransactionWithData>();
            foreach (var acceptedTransactionId in acceptedTransactionIdsWithBlock.AcceptedTransactionIds_)
            {
                if (!state.TransactionsById.ContainsKey(acceptedTransactionId)) 
                { 
                    // This might happen if transactions were filtered-out, or if transaction was included in a block
                    // deeper than where we started processing
                    continue;
                } 
                var transaction = state.TransactionsById[acceptedTransactionId];
                transaction.acceptingBlockHash = acceptingBlockHash;
                state.TransactionsByAcceptingBlockHash[acceptingBlockHash].Add(transaction);
            }
        }
    }

    private static async Task<AsyncDuplexStreamingCall<KaspadMessage, KaspadMessage>> SetupFollowTransactionsExample()
    {
        // Using port 16610 for devnet. Use 16210 for testnet, or don't pass a url for mainnet
        var kaspadClient = new KaspadClient("http://localhost:16610");
        var messageStream = kaspadClient.Client.MessageStream();

        var notifyBlockAddedRequestMessage = new KaspadMessage
        {
            NotifyBlockAddedRequest = new NotifyBlockAddedRequestMessage(),
        };
        await messageStream.RequestStream.WriteAsync(notifyBlockAddedRequestMessage);
        await messageStream.ResponseStream.MoveNext();
        var response = messageStream.ResponseStream.Current;
        if (response.NotifyBlockAddedResponse.Error != null)
        {
            throw new Exception(
                $"Error when subscribing to blockAdded notifications: {response.NotifyUtxosChangedResponse.Error}");
        }

        var notifyVirtualSelectedParentChainChangedRequest = new KaspadMessage
        {
            NotifyVirtualSelectedParentChainChangedRequest = new NotifyVirtualSelectedParentChainChangedRequestMessage
            {
                IncludeAcceptedTransactionIds = true
            },
        };
        do
        {
            await messageStream.RequestStream.WriteAsync(notifyVirtualSelectedParentChainChangedRequest);
            await messageStream.ResponseStream.MoveNext();
            response = messageStream.ResponseStream.Current;
        } while (response.PayloadCase != // At this point we might already get BlockAdded notifications, so skip those 
                 KaspadMessage.PayloadOneofCase.NotifyVirtualSelectedParentChainChangedResponse);

        if (response.NotifyVirtualSelectedParentChainChangedResponse.Error != null)
        {
            throw new Exception(
                $"Error when subscribing to blockAdded notifications: {response.NotifyUtxosChangedResponse.Error}");
            
        }
        
        Console.WriteLine("Subscribed to notifications, awaiting messages...");

        return messageStream;
    }
}