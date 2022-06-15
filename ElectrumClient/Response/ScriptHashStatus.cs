using ElectrumClient.Hashing;
using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    public interface IScriptHashStatus
    {
        public IHash ScriptHash { get; }
        public IHash Hash { get; }
    }

    internal class ScriptHashStatus : ResponseBase, IScriptHashStatus
    {
        public ScriptHashStatus()
        {
            Result = new StatusResult();
        }

        [JsonProperty("result")]
        internal StatusResult Result { get; set; }

        public IHash ScriptHash {
            get { return Result.ScriptHash; }
            set { Result.ScriptHash = new Hash(value.Hex, true); }
        }

        public IHash Hash {  get { return Result.Hash; } }

        internal static ScriptHashStatus FromJson(string? scriptHash, string json)
        {
            var status = JsonConvert.DeserializeObject<ScriptHashStatus>(json) ?? new ScriptHashStatus();
            if (scriptHash != null) status.Result.ScriptHash = new Hash((IHex)new Hex(scriptHash), true);
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
                ScriptHash = new Hash((IHex)new Hex(scriptHash), true);
                Hash = new Hash((IHex)new Hex(hash));
            }

            public static implicit operator StatusResult(string hash)
            {
                return new StatusResult(hash);
            }

            public Hash ScriptHash { get; set;  }
            public Hash Hash { get; set; }
        }
    }
}
