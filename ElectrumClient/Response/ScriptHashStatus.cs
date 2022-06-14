using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    public interface IScriptHashStatus
    {
        public string ScriptHash { get; }
        public string Hash { get; }
    }

    internal class ScriptHashStatus : ResponseBase, IScriptHashStatus
    {
        public ScriptHashStatus()
        {
            Result = new StatusResult();
        }

        [JsonProperty("result")]
        internal StatusResult Result { get; set; }

        public string ScriptHash {
            get { return Result.ScriptHash; }
            set { Result.ScriptHash = value; }
        }

        public string Hash {  get { return Result.Hash; } }

        internal static ScriptHashStatus FromJson(string? scriptHash, string json)
        {
            var status = JsonConvert.DeserializeObject<ScriptHashStatus>(json) ?? new ScriptHashStatus();
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
