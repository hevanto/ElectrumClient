namespace ElectrumClient.Request
{
    internal class BlockChainBlockHeaderRequest : RequestBase
    {
        private static readonly string METHOD = "blockchain.block.header";

        internal BlockChainBlockHeaderRequest(long height, long? checkPointHeight)
            : base(METHOD, height, checkPointHeight ?? 0)
        {
        }
    }
}
