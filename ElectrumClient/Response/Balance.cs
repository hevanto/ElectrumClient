using ElectrumClient.Converters;
using NBitcoin;
using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    public interface IBalance
    {
        public Money Confirmed { get; }
        public Money Unconfirmed { get; }
    }

    internal class Balance : ResponseBase, IBalance
    {
        public Balance()
        {
            Result = new BalanceResult();
        }

        [JsonProperty("result")]
        internal BalanceResult Result { get; set; }

        public Money Confirmed { get { return Result.Confirmed; } }
        public Money Unconfirmed { get { return Result.Unconfirmed; } }

        internal class BalanceResult
        {
            internal BalanceResult()
            {
                Confirmed = Money.Zero;
                Unconfirmed = Money.Zero;
            }

            [JsonProperty("confirmed")]
            [JsonConverter(typeof(MoneyConverterSats))]
            public Money Confirmed { get; set; }

            [JsonProperty("unconfirmed")]
            [JsonConverter(typeof(MoneyConverterSats))]
            public Money Unconfirmed { get; set; }
        }
    }
}
