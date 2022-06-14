namespace ElectrumClient.Request
{
    internal class BlockChainTransactionGetRequest : RequestBase
    {
        private static readonly string METHOD = "blockchain.transaction.get";
        internal BlockChainTransactionGetRequest(string txHash, bool verbose = false)
            : base(METHOD, txHash, verbose)
        {
        }
    }
}
