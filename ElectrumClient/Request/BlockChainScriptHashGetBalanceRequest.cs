namespace ElectrumClient.Request
{
    internal class BlockChainScriptHashGetBalanceRequest : RequestBase
    {
        private static readonly string METHOD = "blockchain.scripthash.get_balance";
        internal BlockChainScriptHashGetBalanceRequest(string scriptHash)
            : base(METHOD, scriptHash)
        {
        }
    }
}
