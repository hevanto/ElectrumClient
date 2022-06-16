using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    public interface IBlockHeaders
    {
        public IList<IHexString> Headers { get; }
        public long Max { get; }
        public Hash<BitSize256> Root { get; }
        public IList<Hash<BitSize256>> Branch { get; }
    }

    internal class BlockHeaders : ResponseBase, IBlockHeaders
    {
        private BlockHeadersResult _result;
        private List<IHexString> _hdrs;

        public BlockHeaders()
        {
            _hdrs = new List<IHexString>();
            _result = new BlockHeadersResult();
        }

        [JsonProperty("result")]
        internal BlockHeadersResult Result
        {
            get { return _result; }
            set
            {
                _hdrs = new List<IHexString>();
                _result = value;
            }
        }

        public IList<IHexString> Headers
        {
            get
            {
                _hdrs = new List<IHexString>();
                if (_hdrs.Count != 0)
                {
                    int hdrLen = (int)(_result.Hex.Length / _result.Count);

                    for (int i = 0; i < _result.Count; i++)
                        _hdrs.Add(new HexString(_result.Hex.Substring(i * hdrLen, hdrLen)));
                }
                return _hdrs;
            }
        }
        public long Max {  get { return _result.Max;  } }
        public Hash<BitSize256> Root { get { return HashFactory.Create256(Result.Root); } }
        public IList<Hash<BitSize256>> Branch
        {
            get
            {
                var lst = new List<Hash<BitSize256>>();
                foreach (var item in _result.Branch)
                    lst.Add(HashFactory.Create256(item));
                return lst;
            }
        }

        internal class BlockHeadersResult
        {
            internal BlockHeadersResult()
            {
                Hex = "";
                Root = "";
                Branch = new List<string>();
            }

            [JsonProperty("count")]
            public long Count { get; set; }

            [JsonProperty("hex")]
            public string Hex { get; set; }

            [JsonProperty("max")]
            public long Max { get; set; }

            [JsonProperty("root")]
            public string Root { get; set; }

            [JsonProperty("branch")]
            public List<string> Branch { get; set; }
        }
    }
}
