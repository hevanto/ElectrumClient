namespace ElectrumClient.Request
{
    internal class BlockChainHeadersSubscribeRequest : RequestBase
    {
        private static readonly string METHOD = "blockchain.headers.subscribe";
        internal BlockChainHeadersSubscribeRequest()
            : base(METHOD)
        {
        }
    }
}
