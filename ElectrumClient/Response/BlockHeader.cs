using ElectrumClient.Hashing;
using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    public interface IBlockHeader
    {
        public IList<IHash> Branch { get; }
        public IHex Header { get; }
        public IHash Root { get; }
    }

    internal class BlockHeader : ResponseBase, IBlockHeader
    {
        public BlockHeader()
        {
            Result = new BlockHeaderResult();
        }

        [JsonProperty("result")]
        internal BlockHeaderResult Result { get; set; }

        public IList<IHash> Branch
        {
            get
            {
                var list = new List<IHash>();
                foreach (var elem in Result.Branch)
                    list.Add(elem);
                return list;
            }
        }
        public IHex Header { get { return Result.Header; } }
        public IHash Root { get { return Result.Root; } }

        internal class BlockHeaderResult
        {
            internal BlockHeaderResult() : this("")
            {
            }

            internal BlockHeaderResult(string value)
            {
                Branch = new List<Hash>();
                Header = value;
                Root = "";
            }

            public static implicit operator BlockHeaderResult(string header)
            {
                return new BlockHeaderResult(header);
            }
            

            [JsonProperty("branch")]
            public List<Hash> Branch { get; set; }

            [JsonProperty("header")]
            public Hex Header { get; set; }

            [JsonProperty("root")]
            public Hash Root { get; set; }
        }
    }
}
