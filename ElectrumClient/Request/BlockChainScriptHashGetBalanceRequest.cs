using ElectrumClient.Hashing;

namespace ElectrumClient.Request
{
    internal class BlockChainScriptHashGetBalanceRequest : RequestBase
    {
        private static readonly string METHOD = "blockchain.scripthash.get_balance";
        internal BlockChainScriptHashGetBalanceRequest(IHash scriptHash)
            : base(METHOD, scriptHash.ToScriptHashString())
        {
        }
    }
}
