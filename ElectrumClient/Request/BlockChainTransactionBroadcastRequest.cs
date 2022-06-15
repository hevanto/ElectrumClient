using ElectrumClient.Hashing;

namespace ElectrumClient.Request
{
    internal class BlockChainTransactionBroadcastRequest : RequestBase
    {
        private static readonly string METHOD = "blockchain.transaction.broadcast";
        internal BlockChainTransactionBroadcastRequest(IHex rawTx)
            : base(METHOD, rawTx.ToString())
        {
        }
    }
}
