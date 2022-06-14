namespace ElectrumClient.Request
{
    internal class BlockChainRelayFeeRequest : RequestBase
    {
        private static readonly string METHOD = "blockchain.relayfee";
        internal BlockChainRelayFeeRequest()
            : base(METHOD)
        {
        }
    }
}
