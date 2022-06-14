using ElectrumClient.Response;
using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    public interface IBlockHeaders
    {
        public IList<string> Headers { get; }
        public long Max { get; }
        public string Root { get; }
        public IList<string> Branch { get; }
    }

    internal class BlockHeaders : ResponseBase, IBlockHeaders
    {
        private BlockHeadersResult _result;
        private List<string> _hdrs;

        public BlockHeaders()
        {
            _hdrs = new List<string>();
            _result = new BlockHeadersResult();
        }

        [JsonProperty("result")]
        internal BlockHeadersResult Result
        {
            get { return _result; }
            set
            {
                _hdrs = new List<string>();
                _result = value;
            }
        }

        public IList<string> Headers
        {
            get
            {
                _hdrs = new List<string>();
                int hdrLen = (int)(_result.Hex.Length / _result.Count);

                for (int i = 0; i < _result.Count; i++)
                    _hdrs.Add(_result.Hex.Substring(i * hdrLen, hdrLen));
                
                return _hdrs;
            }
        }
        public long Max {  get { return _result.Max;  } }
        public string Root { get { return _result.Root; } }
        public IList<string> Branch { get { return _result.Branch;  } }

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
