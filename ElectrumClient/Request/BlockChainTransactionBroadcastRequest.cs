namespace ElectrumClient.Request
{
    internal class BlockChainTransactionBroadcastRequest : RequestBase
    {
        private static readonly string METHOD = "blockchain.transaction.broadcast";
        internal BlockChainTransactionBroadcastRequest(IHexString rawTx)
            : base(METHOD, rawTx.ToString())
        {
        }
    }
}
