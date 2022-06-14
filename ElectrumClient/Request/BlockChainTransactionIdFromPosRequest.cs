namespace ElectrumClient.Request
{
    internal class BlockChainTransactionIdFromPosRequest : RequestBase
    {
        private static readonly string METHOD = "blockchain.transaction.id_from_pos";
        internal BlockChainTransactionIdFromPosRequest(long height, long txPos, bool merkle = false)
            : base(METHOD, height, txPos, merkle)
        {
        }
    }
}
