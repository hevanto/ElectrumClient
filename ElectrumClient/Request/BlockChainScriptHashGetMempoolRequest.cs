using ElectrumClient.Hashing;

namespace ElectrumClient.Request
{
    internal class BlockChainScriptHashGetMempoolRequest : RequestBase
    {
        private static readonly string METHOD = "blockchain.scripthash.get_mempool";
        internal BlockChainScriptHashGetMempoolRequest(IHash scriptHash)
            : base(METHOD, scriptHash.ToScriptHashString())
        {
        }
    }
}
