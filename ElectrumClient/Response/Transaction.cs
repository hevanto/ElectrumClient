using ElectrumClient.Converters;
using ElectrumClient.Hashing;
using NBitcoin;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ElectrumClient.Response
{
    public class BroadcastTransaction { }
    public class OnChainTransaction { }

    public interface ITransaction
    {
        public IHash? BlockHash { get; }
        public DateTime? BlockTime { get; }
        public long? Confirmations { get; }
        public IHash? Hash { get; }
        public IHex? Hex { get; }
        public long? LockTime { get; }
        public long? Size { get; }
        public DateTime? Time { get; }
        public IHash? TxId { get; }
        public long? Version { get; }
        public IList<IVin>? Vin { get; }
        public IList<IVout>? Vout { get; }

        Transaction ToTransaction(Network network);
    }
    public interface IVin
    {
        public IScriptSig ScriptSig { get; }
        public long Sequence { get; }
        public IHash TxId { get; }
        public long Vout { get; }
    }
    public interface IScriptSig
    {
        public string Asm { get; }
        public IHex Hex { get; }
    }
    public interface IVout
    {
        public long N { get; }

        public IScriptPubKey ScriptPubKey { get; }
        public Money Value { get; }
    }
    public interface IScriptPubKey
    {
        public IList<string> Addresses { get; }
        public string Asm { get; }
        public IHex Hex { get; }
        public long ReqSigs { get; }
        public string Type { get; }
    }

    internal class Transaction<T> : ResponseBase, ITransaction
    {
        public Transaction()
        {
            Result = new TransactionResult();
        }

        [JsonProperty("result")]
        internal TransactionResult Result { get; set; }

        public IHash? BlockHash { get { return Result.BlockHash; } }
        public DateTime? BlockTime { get { return Result.BlockTime; } }
        public long? Confirmations { get { return Result.Confirmations; } }
        public IHash? Hash { get { return Result.Hash; } }
        public IHex? Hex { get { return Result.Hex; } }
        public long? LockTime { get { return Result.LockTime; } }
        public long? Size { get { return Result.Size; } }
        public DateTime? Time { get { return Result.Time; } }
        public IHash? TxId { get { return Result.TxId; } }
        public long? Version { get { return Result.Version; } }
        public IList<IVin>? Vin
        {
            get
            {
                if (Result.Vin == null) return null;

                var res = new List<IVin>();
                foreach (var item in Result.Vin) res.Add(item);
                return res;
            }
        }
        public IList<IVout>? Vout
        {
            get
            {
                if (Result.Vout == null) return null;

                var res = new List<IVout>();
                foreach (var item in Result.Vout) res.Add(item);
                return res;
            }
        }

        public Transaction ToTransaction(Network network)
        {
            return Transaction.Parse(Hex?.ToString(), network);
        }

        internal class TransactionResult
        {
            internal TransactionResult() : this("", "")
            {
            }

            internal TransactionResult(string hash, string hex)
            {
                Hash = new Hash((IHex)new Hex(hash));
                Hex = new Hex(hex);
            }

            public static implicit operator TransactionResult(string txResult)
            {
                
                if (typeof(T) == typeof(BroadcastTransaction))
                    return new TransactionResult(txResult, "");
                return new TransactionResult("", txResult);
            }

            [JsonProperty("blockhash")]
            public Hash? BlockHash { get; set; }

            [JsonProperty("blocktime")]
            [JsonConverter(typeof(UnixDateTimeConverter))]
            public DateTime? BlockTime { get; set; }

            [JsonProperty("confirmations")]
            public long? Confirmations { get; set; }

            [JsonProperty("hash")]
            public Hash? Hash { get; set; }

            [JsonProperty("hex")]
            public Hex? Hex { get; set; }

            [JsonProperty("locktime")]
            public long? LockTime { get; set; }

            [JsonProperty("size")]
            public long? Size { get; set; }

            [JsonProperty("time")]
            [JsonConverter(typeof(UnixDateTimeConverter))]
            public DateTime? Time { get; set; }

            [JsonProperty("txid")]
            public Hash? TxId { get; set; }

            [JsonProperty("version")]
            public long? Version { get; set; }

            [JsonProperty("vin")]
            public List<VinResult>? Vin { get; set; }

            [JsonProperty("vout")]
            public List<VoutResult>? Vout { get; set; }
        }

        internal class VinResult : IVin
        {
            [JsonProperty("txid")]
            internal Hash _txId;

            internal VinResult()
            {
                scriptSig = new ScriptSigResult();
                _txId = "";
            }

            [JsonProperty("scriptSig")]
            internal ScriptSigResult scriptSig { get; set; }

            public IScriptSig ScriptSig
            {
                get { return scriptSig; }
            }
            
            [JsonProperty("sequence")]
            public long Sequence { get; set; }

            public IHash TxId { get { return _txId; } }

            [JsonProperty("vout")]
            public long Vout { get; set; }
        }

        internal class ScriptSigResult : IScriptSig
        {
            [JsonProperty("asm")]
            internal string _asm;

            [JsonProperty("hex")]
            internal Hex _hex;
            internal ScriptSigResult()
            {
                _asm = "";
                _hex = Hashing.Hex.Empty;
            }

            
            public string Asm { get { return _asm; } }
            public IHex Hex { get { return _hex; } }
        }

        internal class VoutResult : IVout
        {
            internal VoutResult()
            {
                scriptPubKey = new ScriptPubKeyResult();
                value = Money.Zero;
            }

            [JsonProperty("n")]
            public long N { get; set; }

            [JsonProperty("scriptPubKey")]
            internal ScriptPubKeyResult scriptPubKey { get; set; }

            public IScriptPubKey ScriptPubKey
            {
                get { return scriptPubKey; }
            }

            [JsonProperty("value")]
            [JsonConverter(typeof(MoneyConverterBTC))]
            internal Money value { get; set; }

            public Money Value
            {
                get { return value; }
            }
        }

        internal class ScriptPubKeyResult : IScriptPubKey
        {
            [JsonProperty("hex")]
            internal Hex _hex;

            internal ScriptPubKeyResult()
            {
                addresses = new List<string>();
                Asm = "";
                _hex = Hashing.Hex.Empty;
                Type = "";
            }

            [JsonProperty("addresses")]
            public List<string> addresses { get; set; }

            public IList<string> Addresses
            {
                get { return addresses; }
            }

            [JsonProperty("asm")]
            public string Asm { get; set; }

            public IHex Hex { get { return _hex; } }

            [JsonProperty("reqSigs")]
            public long ReqSigs { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }
        }
    }
}
