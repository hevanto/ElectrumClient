using ElectrumClient.Hashing;
using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    public interface IScriptHashStatus
    {
        public IHash ScriptHash { get; }
        public Hash<BitSize256> Hash { get; }
    }

    internal class ScriptHashStatus<BS> : ResponseBase, IScriptHashStatus
        where BS : IBitSize, new()
    {
        public ScriptHashStatus()
        {
            Result = new StatusResult();
        }

        public ScriptHashStatus(BS BitSize) : this() { }

        [JsonProperty("result")]
        internal StatusResult Result { get; set; }

        public IHash ScriptHash {
            get { return new HexString(Result.ScriptHash).Hash ?? new Hash<BS>(); }
            set { Result.ScriptHash = value.ToString(); }
        }

        public Hash<BitSize256> Hash {  get { return HashFactory.Create256(Result.Hash); } }

        internal static ScriptHashStatus<BS> FromJson(string? scriptHash, string json)
        {
            var status = JsonConvert.DeserializeObject<ScriptHashStatus<BS>>(json) ?? new ScriptHashStatus<BS>();
            if (scriptHash != null) status.Result.ScriptHash = scriptHash;
            return status;
        }

        internal class StatusResult
        {
            internal StatusResult() : this("", "")
            {
            }

            internal StatusResult(string hash) : this("", hash)
            {
            }

            internal StatusResult(string[] arr) : this(arr[0], arr[1])
            {
            }

            internal StatusResult(object[] arr) : this(arr[0]?.ToString() ?? "", arr[1]?.ToString() ?? "")
            {
            }

            internal StatusResult(string scriptHash, string hash)
            {
                ScriptHash = scriptHash;
                Hash = hash;
            }

            public static implicit operator StatusResult(string hash)
            {
                return new StatusResult(hash);
            }

            public string ScriptHash { get; set;  }
            public string Hash { get; set; }
        }
    }
}
