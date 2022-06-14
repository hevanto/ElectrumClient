namespace ElectrumClient.Request
{
    internal class BlockChainScriptHashListUnspent : RequestBase
    {
        private static readonly string METHOD = "blockchain.scripthash.listunspent";
        internal BlockChainScriptHashListUnspent(string scriptHash)
            : base(METHOD, scriptHash)
        {
        }
    }
}
