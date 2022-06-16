using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    public interface IBlockHeader
    {
        public IList<Hash<BitSize256>> Branch { get; }
        public IHexString Header { get; }
        public Hash<BitSize256> Root { get; }
    }

    internal class BlockHeader : ResponseBase, IBlockHeader
    {
        public BlockHeader()
        {
            Result = new BlockHeaderResult();
        }

        [JsonProperty("result")]
        internal BlockHeaderResult Result { get; set; }

        public IList<Hash<BitSize256>> Branch
        {
            get
            {
                var list = new List<Hash<BitSize256>>();
                foreach (var elem in Result.Branch)
                    list.Add(HashFactory.Create256(elem));
                return list;
            }
        }
        public IHexString Header { get { return new HexString(Result.Header); } }
        public Hash<BitSize256> Root { get { return HashFactory.Create256(Result.Root); } }

        internal class BlockHeaderResult
        {
            internal BlockHeaderResult() : this("")
            {
            }

            internal BlockHeaderResult(string value)
            {
                Branch = new List<string>();
                Header = "";
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
