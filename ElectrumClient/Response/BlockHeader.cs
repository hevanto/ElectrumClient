using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    public interface IBlockHeader
    {
        public IList<string> Branch { get; }
        public string Header { get; }
        public string Root { get; }
    }

    internal class BlockHeader : ResponseBase, IBlockHeader
    {
        public BlockHeader()
        {
            Result = new BlockHeaderResult();
        }

        [JsonProperty("result")]
        internal BlockHeaderResult Result { get; set; }

        public IList<string> Branch { get { return Result.Branch; } }
        public string Header { get { return Result.Header; } }
        public string Root { get { return Result.Root; } }

        internal class BlockHeaderResult
        {
            internal BlockHeaderResult() : this("")
            {
            }

            internal BlockHeaderResult(string value)
            {
                Branch = new List<string>();
                Header = value;
                Root = "";
            }

            public static implicit operator BlockHeaderResult(string header)
            {
                return new BlockHeaderResult(header);
            }
            

            [JsonProperty("branch")]
            public List<string> Branch { get; set; }

            [JsonProperty("header")]
            public string Header { get; set; }

            [JsonProperty("root")]
            public string Root { get; set; }
        }
    }
}
