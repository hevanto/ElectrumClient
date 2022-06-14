using ElectrumClient.Converters;
using NBitcoin;
using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    internal interface IUnspentOutputList
    {
        public IList<IUnspentOutput> List { get; }
    }
    public interface IUnspentOutput
    {
        public long Height { get; }
        public string TxHash { get; }
        public long TxPos { get; }
        public Money Value { get; }
    }

    internal class UnspentOutputList : ResponseBase, IUnspentOutputList
    {
        public UnspentOutputList()
        {
            Result = new List<UnspentOutput>();
        }

        [JsonProperty("result")]
        internal List<UnspentOutput> Result { get; set; }

        public IList<IUnspentOutput> List
        {
            get
            {
                var list = new List<IUnspentOutput>();
                foreach (var item in Result) list.Add(item);
                return list;
            }
        }
    }

    internal class UnspentOutput : IUnspentOutput
    {
        [JsonProperty("value")]
        [JsonConverter(typeof(MoneyConverterSats))]
        public Money _value;

        public UnspentOutput()
        {
            _value = Money.Zero;
            TxHash = "";
        }

        

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("tx_hash")]
        public string TxHash { get; set; }

        [JsonProperty("tx_pos")]
        public long TxPos { get; set; }

        public Money Value {
            get { return _value; }
            set
            {
                _value = value;
            }
        }
    }
}
