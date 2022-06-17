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
    public interface IUnspentOutput : IAsyncResponseResult
    {
        public long Height { get; }
        public Hash<BitSize256> TxHash { get; }
        public long TxPos { get; }
        public Money Value { get; }

        public ICoin GetCoin(Client client);
        public long GetConfirmations(Client client);
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
                foreach (var item in Result)
                {
                    ((IAsyncResponseResult)item).SetNetwork(((IAsyncResponseResult)this).Network);
                    list.Add(item);
                }
                return list;
            }
        }
    }

    internal class UnspentOutput : ResponseBase, IUnspentOutput
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
            _network = Network.TestNet; // Safe default
        }

        private ITransaction? _tx;

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

        public ICoin GetCoin(Client client)
        {
            var tx = GetTx(client);
            return tx.GetCoins()[(int)TxPos] ;
        }

        public long GetConfirmations(Client client)
        {
            var tx = GetTx(client);
            return tx.Confirmations ?? 0;
        }

        private ITransaction GetTx(Client client)
        {
            if (_tx != null) return _tx;

            IError? error;
            _tx = client.GetTransaction(TxHash, out error);
            if (error != null) throw new Exception(error.ToString());
            return _tx;
        }
    }
}
