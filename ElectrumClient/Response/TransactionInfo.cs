using ElectrumClient.Converters;
using NBitcoin;
using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    internal interface ITransactionInfoList : IAsyncResponseResult
    {
        public IList<ITransactionInfo> List { get; }
    }

    public interface ITransactionInfo : IAsyncResponseResult
    {
        public long Height { get; }
        public Hash<BitSize256> TxHash { get; }
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
                foreach (var item in Result)
                {
                    ((IAsyncResponseResult)item).SetNetwork(((IAsyncResponseResult)this).Network);
                    list.Add(item);
                }
                return list;
            }
        }
    }

    internal class TransactionInfo : ResponseBase, ITransactionInfo
    {
        [JsonProperty("fee")]
        [JsonConverter(typeof(MoneyConverterSats))]
        internal Money _fee;

        [JsonProperty("merkle")]
        internal List<string> _merkle;

        [JsonProperty("tx_hash")]
        internal string _txHash;

        public TransactionInfo()
        {
            _fee = Money.Zero;
            _merkle = new List<string>();
            _txHash = "";
            _network = Network.TestNet; // Safe default
        }


        [JsonProperty("height")]
        public long Height { get; set; }


        public Hash<BitSize256> TxHash { get { return HashFactory.Create256(_txHash); } }

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

    internal interface IMempoolTransationInfoList : IAsyncResponseResult
    {
        public IList<IMempoolTransactionInfo> List { get; }
    }

    public interface IMempoolTransactionInfo : IAsyncResponseResult
    {
        public long Height { get; }
        public Hash<BitSize256> TxHash { get; }
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
                foreach (var item in Result)
                {
                    ((IAsyncResponseResult)item).SetNetwork(((IAsyncResponseResult)this).Network);
                    list.Add(item);
                }
                return list;
            }
        }
    }

    internal class MempoolTransactionInfo : ResponseBase, IMempoolTransactionInfo
    {
        [JsonProperty("fee")]
        [JsonConverter(typeof(MoneyConverterSats))]
        internal Money _fee;

        [JsonProperty("tx_hash")]
        internal string _txHash;

        public MempoolTransactionInfo()
        {
            _fee = Money.Zero;
            _txHash = "";
            _network = Network.TestNet; // Safe default;
        }

        [JsonProperty("height")]
        public long Height { get; set; }

        
        public Hash<BitSize256> TxHash { get { return HashFactory.Create256(_txHash); } }

        public Money Fee { get { return _fee; } }
    }
}
