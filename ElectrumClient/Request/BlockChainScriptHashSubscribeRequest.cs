using ElectrumClient.Hashing;

namespace ElectrumClient.Request
{
    internal class BlockChainScriptHashSubscribeRequest : RequestBase
    {
        private static readonly string METHOD = "blockchain.scripthash.subscribe";
        internal BlockChainScriptHashSubscribeRequest(IHash scriptHash)
            : base (METHOD, scriptHash.ToScriptHashString())
        {
        }
    }
}
