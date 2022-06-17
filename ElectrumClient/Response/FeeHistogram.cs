using ElectrumClient.Converters;
using NBitcoin;
using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    internal interface IFeeHistogram : IAsyncResponseResult
    {
        public IList<IFeeHistogramPoint> List { get; }
    }
    public interface IFeeHistogramPoint
    {
        public Money Fee { get; }
        public long VSize { get; }
    }

    internal class FeeHistogram : ResponseBase, IFeeHistogram
    {
        public FeeHistogram()
        {
            Result = new List<FeeHistogramPoint>();
        }

        [JsonProperty("result")]
        [JsonConverter(typeof(FeeHistogramConverter))]
        internal List<FeeHistogramPoint> Result { get; set; }

        public IList<IFeeHistogramPoint> List
        {
            get
            {
                var res = new List<IFeeHistogramPoint>();
                foreach (var item in Result) res.Add(item);
                return res;
            } 
        }
    }

    internal class FeeHistogramPoint : IFeeHistogramPoint
    {
        private long _fee;

        public FeeHistogramPoint() : this(0, 0)
        {
        }

        public FeeHistogramPoint(long fee, long vsize)
        {
            _fee = fee;
            VSize = vsize;
        }

        public Money Fee { get { return new Money(_fee); } }
        public long VSize { get; }
    }
}
