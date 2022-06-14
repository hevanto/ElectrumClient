namespace ElectrumClient.Request
{
    internal class BlockChainScriptHashGetMempoolRequest : RequestBase
    {
        private static readonly string METHOD = "blockchain.scripthash.get_mempool";
        internal BlockChainScriptHashGetMempoolRequest(string scriptHash)
            : base(METHOD, scriptHash)
        {
        }
    }
}
