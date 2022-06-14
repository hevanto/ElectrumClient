namespace ElectrumClient.Request
{
    internal class BlockChainTransactionGetMerkleRequest : RequestBase
    {
        private static readonly string METHOD = "blockchain.transaction.get_merkle";
        internal BlockChainTransactionGetMerkleRequest(string txHash, long height)
            : base(METHOD, txHash, height)
        {
        }
    }
}
