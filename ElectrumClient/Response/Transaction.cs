using ElectrumClient.Converters;
using ElectrumClient.Hashing;
using NBitcoin;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ElectrumClient.Response
{
    public interface ITransactionKind { }
    public class BroadcastTransaction : ITransactionKind { }
    public class OnChainTransaction : ITransactionKind { }

    public interface ITransaction : IAsyncResponseResult
    {
        public Hash<BitSize256>? BlockHash { get; }
        public DateTime? BlockTime { get; }
        public long? Confirmations { get; }
        public Hash<BitSize256>? Hash { get; }
        public IHexString? Hex { get; }
        public long? LockTime { get; }
        public long? Size { get; }
        public DateTime? Time { get; }
        public Hash<BitSize256>? TxId { get; }
        public long? Version { get; }
        public IList<IVin>? Vin { get; }
        public IList<IVout>? Vout { get; }

        Transaction ToTransaction();
        IList<Coin> GetCoins();
    }
    public interface IVin
    {
        public IScriptSig ScriptSig { get; }
        public long Sequence { get; }
        public Hash<BitSize256> TxId { get; }
        public long Vout { get; }
    }
    public interface IScriptSig
    {
        public string Asm { get; }
        public IHexString Hex { get; }
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
        public IHexString Hex { get; }
        public long ReqSigs { get; }
        public string Type { get; }
    }

    internal class Transaction<T> : ResponseBase, ITransaction
        where T : ITransactionKind
    {
        public Transaction()
        {
            Result = new TransactionResult();
        }

        [JsonProperty("result")]
        internal TransactionResult Result { get; set; }

        public Hash<BitSize256>? BlockHash { get { return Result.BlockHash; } }
        public DateTime? BlockTime { get { return Result.BlockTime; } }
        public long? Confirmations { get { return Result.Confirmations; } }
        public Hash<BitSize256>? Hash { get { return Result.Hash; } }
        public IHexString? Hex { get { return Result.Hex; } }
        public long? LockTime { get { return Result.LockTime; } }
        public long? Size { get { return Result.Size; } }
        public DateTime? Time { get { return Result.Time; } }
        public Hash<BitSize256>? TxId { get { return Result.TxId; } }
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

        public Transaction ToTransaction()
        {
            return Transaction.Parse(Hex?.ToString(), ((IAsyncResponseResult)this).Network);
        }

        public IList<Coin> GetCoins()
        {
            Transaction tx = ToTransaction();
            return tx.Outputs.AsCoins().ToList();
        }

        internal class TransactionResult
        {
            [JsonProperty("blockhash")]
            internal string? _blockHash;

            [JsonProperty("hash")]
            internal string? _hash;

            [JsonProperty("hex")]
            internal string? _hex;

            [JsonProperty("txid")]
            internal string? _txId;

            internal TransactionResult() : this("", "")
            {
            }

            internal TransactionResult(string hash, string hex)
            {
                _hash = hash;
                _hex = hex;
            }

            public static implicit operator TransactionResult(string txResult)
            {
                
                if (typeof(T) == typeof(BroadcastTransaction))
                    return new TransactionResult(txResult, "");
                return new TransactionResult("", txResult);
            }

            
            public Hash<BitSize256>? BlockHash
            {
                get
                {
                    if (_blockHash == null) return null;
                    return HashFactory.Create256(_blockHash);
                }
            }

            [JsonProperty("blocktime")]
            [JsonConverter(typeof(UnixDateTimeConverter))]
            public DateTime? BlockTime { get; set; }

            [JsonProperty("confirmations")]
            public long? Confirmations { get; set; }

            
            public Hash<BitSize256>? Hash
            { 
                get
                {
                    if (_hash == null) return null;
                    return HashFactory.Create256(_hash);
                }
            }

            public IHexString? Hex
            {
                get
                {
                    if (_hex == null) return null;
                    return new HexString(_hex);
                }
            }

            [JsonProperty("locktime")]
            public long? LockTime { get; set; }

            [JsonProperty("size")]
            public long? Size { get; set; }

            [JsonProperty("time")]
            [JsonConverter(typeof(UnixDateTimeConverter))]
            public DateTime? Time { get; set; }

            public Hash<BitSize256>? TxId
            { 
                get
                {
                    if (_txId == null) { return null; }
                    return HashFactory.Create256(_txId);
                }
            }

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
            internal string _txId;

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

            public Hash<BitSize256> TxId { get { return HashFactory.Create256(_txId); } }

            [JsonProperty("vout")]
            public long Vout { get; set; }
        }

        internal class ScriptSigResult : IScriptSig
        {
            [JsonProperty("asm")]
            internal string _asm;

            [JsonProperty("hex")]
            internal string _hex;
            internal ScriptSigResult()
            {
                _asm = "";
                _hex = "";
            }

            
            public string Asm { get { return _asm; } }
            public IHexString Hex { get { return new HexString(_hex); } }
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
            internal string _hex;

            [JsonProperty("addresses")]
            internal List<string> _addresses;

            internal ScriptPubKeyResult()
            {
                _hex = "";
                _addresses = new List<string>();

                Asm = "";
                Type = "";
            }

            

            public IList<string> Addresses
            {
                get { return _addresses; }
            }

            [JsonProperty("asm")]
            public string Asm { get; set; }

            public IHexString Hex { get { return new HexString(_hex); } }

            [JsonProperty("reqSigs")]
            public long ReqSigs { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }
        }
    }
}
