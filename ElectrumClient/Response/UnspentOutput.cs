using ElectrumClient.Converters;
using ElectrumClient.Hashing;
using NBitcoin;
using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    internal interface IUnspentOutputList : IAsyncResponseResult
    {
        public IList<IUnspentOutput> List { get; }
    }
    public interface IUnspentOutput
    {
        public long Height { get; }
        public Hash<BitSize256> TxHash { get; }
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
        internal Money _value;

        [JsonProperty("tx_hash")]
        internal string _txHash;

        public UnspentOutput()
        {
            _value = Money.Zero;
            _txHash = "";
        }

        

        [JsonProperty("height")]
        public long Height { get; set; }

        public Hash<BitSize256> TxHash { get { return HashFactory.Create256(_txHash); } }

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
