namespace ElectrumClient.Request
{
    internal class BlockChainScriptHashGetHistoryRequest : RequestBase
    {
        private static readonly string METHOD = "blockchain.scripthash.get_history";
        internal BlockChainScriptHashGetHistoryRequest(string scriptHash)
            : base(METHOD, scriptHash)
        {
        }
    }
}
