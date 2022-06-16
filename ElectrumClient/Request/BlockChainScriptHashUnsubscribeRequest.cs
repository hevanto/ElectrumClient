namespace ElectrumClient.Request
{
    internal class BlockChainScriptHashUnsubscribeRequest : RequestBase
    {
        private static readonly string METHOD = "blockchain.scripthash.unsubscribe";
        internal BlockChainScriptHashUnsubscribeRequest(IHash scriptHash)
            : base(METHOD, scriptHash.ToString())
        {
        }
    }
}
