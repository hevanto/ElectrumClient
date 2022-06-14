using ElectrumClient.Converters;
using NBitcoin;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ElectrumClient.Response
{
    public class BroadcastTransaction { }
    public class OnChainTransaction { }

    public interface ITransaction
    {
        public string? BlockHash { get; }
        public DateTime? BlockTime { get; }
        public long? Confirmations { get; }
        public string? Hash { get; }
        public string? Hex { get; }
        public long? LockTime { get; }
        public long? Size { get; }
        public DateTime? Time { get; }
        public string? TxId { get; }
        public long? Version { get; }
        public IList<IVin>? Vin { get; }
        public IList<IVout>? Vout { get; }
    }
    public interface IVin
    {
        public IScriptSig ScriptSig { get; }
        public long Sequence { get; }
        public string TxId { get; }
        public long Vout { get; }
    }
    public interface IScriptSig
    {
        public string Asm { get; }
        public string Hex { get; }
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
        public string Hex { get; }
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

        public string? BlockHash { get { return Result.BlockHash; } }
        public DateTime? BlockTime { get { return Result.BlockTime; } }
        public long? Confirmations { get { return Result.Confirmations; } }
        public string? Hash { get { return Result.Hash; } }
        public string? Hex { get { return Result.Hex; } }
        public long? LockTime { get { return Result.LockTime; } }
        public long? Size { get { return Result.Size; } }
        public DateTime? Time { get { return Result.Time; } }
        public string? TxId { get { return Result.TxId; } }
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

        internal class TransactionResult
        {
            internal TransactionResult() : this("", "")
            {
            }

            internal TransactionResult(string hash, string hex)
            {
                Hash = hash;
                Hex = hex;
            }

            public static implicit operator TransactionResult(string txResult)
            {
                
                if (typeof(T) == typeof(BroadcastTransaction))
                    return new TransactionResult(txResult, "");
                return new TransactionResult("", txResult);
            }

            [JsonProperty("blockhash")]
            public string? BlockHash { get; set; }

            [JsonProperty("blocktime")]
            [JsonConverter(typeof(UnixDateTimeConverter))]
            public DateTime? BlockTime { get; set; }

            [JsonProperty("confirmations")]
            public long? Confirmations { get; set; }

            [JsonProperty("hash")]
            public string? Hash { get; set; }

            [JsonProperty("hex")]
            public string? Hex { get; set; }

            [JsonProperty("locktime")]
            public long? LockTime { get; set; }

            [JsonProperty("size")]
            public long? Size { get; set; }

            [JsonProperty("time")]
            [JsonConverter(typeof(UnixDateTimeConverter))]
            public DateTime? Time { get; set; }

            [JsonProperty("txid")]
            public string? TxId { get; set; }

            [JsonProperty("version")]
            public long? Version { get; set; }

            [JsonProperty("vin")]
            public List<VinResult>? Vin { get; set; }

            [JsonProperty("vout")]
            public List<VoutResult>? Vout { get; set; }
        }

        internal class VinResult : IVin
        {
            internal VinResult()
            {
                scriptSig = new ScriptSigResult();
                TxId = "";
            }

            [JsonProperty("scriptSig")]
            internal ScriptSigResult scriptSig { get; set; }

            public IScriptSig ScriptSig
            {
                get { return scriptSig; }
            }
            
            [JsonProperty("sequence")]
            public long Sequence { get; set; }

            [JsonProperty("txid")]
            public string TxId { get; set; }

            [JsonProperty("vout")]
            public long Vout { get; set; }
        }

        internal class ScriptSigResult : IScriptSig
        {
            internal ScriptSigResult()
            {
                Asm = "";
                Hex = "";
            }

            [JsonProperty("asm")]
            public string Asm { get; set; }

            [JsonProperty("hex")]
            public string Hex { get; set; }
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
            internal ScriptPubKeyResult()
            {
                addresses = new List<string>();
                Asm = "";
                Hex = "";
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

            [JsonProperty("hex")]
            public string Hex { get; set; }

            [JsonProperty("reqSigs")]
            public long ReqSigs { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }
        }
    }
}
