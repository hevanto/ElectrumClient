namespace ElectrumClient.Request
{
    internal class MempoolGetFeeHistogramRequest : RequestBase
    {
        private static readonly string METHOD = "mempool.get_fee_histogram";
        internal MempoolGetFeeHistogramRequest()
            : base(METHOD)
        {
        }
    }
}
