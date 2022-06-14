namespace ElectrumClient.Request
{
    internal class BlockChainBlockHeadersRequest : RequestBase
    {
        private static readonly string METHOD = "blockchain.block.headers";
        internal BlockChainBlockHeadersRequest(long startHeight, long count, long? checkPointHeight)
            : base(METHOD, startHeight, count, checkPointHeight ?? 0)
        {
        }
    }
}
