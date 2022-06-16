namespace ElectrumClient.Request
{
    internal class BlockChainScriptHashListUnspent : RequestBase
    {
        private static readonly string METHOD = "blockchain.scripthash.listunspent";
        internal BlockChainScriptHashListUnspent(IHash scriptHash)
            : base(METHOD, scriptHash.ToString())
        {
        }
    }
}
