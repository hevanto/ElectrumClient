using ElectrumClient.Hashing;
using ElectrumClient.Response;
using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    public interface IBlockHeaders
    {
        public IList<IHex> Headers { get; }
        public long Max { get; }
        public IHash Root { get; }
        public IList<IHash> Branch { get; }
    }

    internal class BlockHeaders : ResponseBase, IBlockHeaders
    {
        private BlockHeadersResult _result;
        private List<IHex> _hdrs;

        public BlockHeaders()
        {
            _hdrs = new List<IHex>();
            _result = new BlockHeadersResult();
        }

        [JsonProperty("result")]
        internal BlockHeadersResult Result
        {
            get { return _result; }
            set
            {
                _hdrs = new List<IHex>();
                _result = value;
            }
        }

        public IList<IHex> Headers
        {
            get
            {
                _hdrs = new List<IHex>();
                if (_hdrs.Count != 0)
                {
                    int hdrLen = (int)(_result.Hex.Length / _result.Count);

                    for (int i = 0; i < _result.Count; i++)
                        _hdrs.Add(new Hex(_result.Hex.Substring(i * hdrLen, hdrLen)));
                }
                return _hdrs;
            }
        }
        public long Max {  get { return _result.Max;  } }
        public IHash Root { get { return _result.Root; } }
        public IList<IHash> Branch
        {
            get
            {
                var lst = new List<IHash>();
                foreach (var item in _result.Branch)
                    lst.Add(item);
                return lst;
            }
        }

        internal class BlockHeadersResult
        {
            internal BlockHeadersResult()
            {
                Hex = "";
                Root = "";
                Branch = new List<Hash>();
            }

            [JsonProperty("count")]
            public long Count { get; set; }

            [JsonProperty("hex")]
            public string Hex { get; set; }

            [JsonProperty("max")]
            public long Max { get; set; }

            [JsonProperty("root")]
            public Hash Root { get; set; }

            [JsonProperty("branch")]
            public List<Hash> Branch { get; set; }
        }
    }
}
