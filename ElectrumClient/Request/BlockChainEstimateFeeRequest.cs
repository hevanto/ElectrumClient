namespace ElectrumClient.Request
{
    internal class BlockChainEstimateFeeRequest : RequestBase
    {
        private static readonly string METHOD = "blockchain.estimatefee";
        internal BlockChainEstimateFeeRequest(long confirmationTarget)
            : base(METHOD, confirmationTarget)
        {
        }
    }
}
