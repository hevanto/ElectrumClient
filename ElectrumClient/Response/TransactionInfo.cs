using ElectrumClient.Converters;
using NBitcoin;
using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    internal interface ITransactionInfoList
    {
        public IList<ITransactionInfo> List { get; }
    }

    public interface ITransactionInfo
    {
        public long Height { get; }
        public string TxHash { get; }
        public Money Fee { get; }
        public IList<string> Merkle { get; }
    }

    internal class TransactionInfoList : ResponseBase, ITransactionInfoList
    {
        public TransactionInfoList()
        {
            Result = new List<TransactionInfo>();
        }

        [JsonProperty("result")]
        internal List<TransactionInfo> Result { get; set; }

        public IList<ITransactionInfo> List
        {
            get
            {
                var list = new List<ITransactionInfo>();
                foreach (var item in Result) list.Add(item);
                return list;
            }
        }
    }
    
    internal class TransactionInfo : ITransactionInfo
    {
        [JsonProperty("fee")]
        [JsonConverter(typeof(MoneyConverterSats))]
        private Money _fee;

        [JsonProperty("merkle")]
        private List<string> _merkle;

        public TransactionInfo()
        {
            _fee = Money.Zero;
            _merkle = new List<string>();
            TxHash = "";
        }


        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("tx_hash")]
        public string TxHash { get; set; }

        public Money Fee {
            get { return _fee; }
            set
            {
                _fee = value;
            }
        }

        public IList<string> Merkle {
            get { return _merkle; }
            set
            {
                _merkle = new List<string>();
                foreach (var item in value) _merkle.Add(item);
            }
        }
    }

    internal interface IMempoolTransationInfoList
    {
        public IList<IMempoolTransactionInfo> List { get; }
    }

    public interface IMempoolTransactionInfo
    {
        public long Height { get; }
        public string TxHash { get; }
        public Money Fee { get; }
    }

    internal class MempoolTransactionInfoList : ResponseBase, IMempoolTransationInfoList
    {
        public MempoolTransactionInfoList()
        {
            Result = new List<MempoolTransactionInfo>();
        }

        [JsonProperty("result")]
        internal List<MempoolTransactionInfo> Result { get; set; }

        public IList<IMempoolTransactionInfo> List
        {
            get
            {
                var list = new List<IMempoolTransactionInfo>();
                foreach (var item in Result) list.Add(item);
                return list;
            }
        }
    }

    internal class MempoolTransactionInfo : ResponseBase, IMempoolTransactionInfo
    {
        [JsonProperty("fee")]
        [JsonConverter(typeof(MoneyConverterSats))]
        private Money _fee;

        public MempoolTransactionInfo()
        {
            _fee = Money.Zero;
            TxHash = "";
        }


        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("tx_hash")]
        public string TxHash { get; set; }

        public Money Fee { get { return _fee; } }
    }
}
