using ElectrumClient.Converters;
using NBitcoin;
using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    public interface IFee : IAsyncResponseResult
    {
        public long TargetConfirmation { get; }
        public Money Amount { get;  }
    }

    internal class Fee : ResponseBase, IFee
    {
        public Fee() : this(0L)
        {
        }

        public Fee(long targetConfirmation)
        {
            Result = Money.Zero;
            TargetConfirmation = targetConfirmation;
        }

        [JsonProperty("result")]
        [JsonConverter(typeof(MoneyConverterBTC))]
        internal Money Result { get; set; }
       
        public long TargetConfirmation { get; set; }
        public Money Amount { get { return Result; } }
    }
}
