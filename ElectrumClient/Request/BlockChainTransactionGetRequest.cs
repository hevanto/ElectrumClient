using ElectrumClient.Hashing;

namespace ElectrumClient.Request
{
    internal class BlockChainTransactionGetRequest : RequestBase
    {
        private static readonly string METHOD = "blockchain.transaction.get";
        internal BlockChainTransactionGetRequest(IHash txHash, bool verbose = false)
            : base(METHOD, txHash.ToString(), verbose)
        {
        }
    }
}
