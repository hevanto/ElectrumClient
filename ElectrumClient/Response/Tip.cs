using NBitcoin;
using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    public interface ITip : IAsyncResponseResult
    {
        public long Height { get; }
        public IHexString Hex { get; }
    }

    internal class Tip : ResponseBase, ITip
    {
        public Tip() : this(new TipResult())
        {
        }

        internal Tip(TipResult result)
        {
            Result = result;
        }

        [JsonProperty("result")]
        internal TipResult Result { get; set; }

        public long Height {  get {  return Result.Height; } }
        public IHexString Hex { get { return new HexString(Result.Hex); } }

        public static implicit operator Tip(TipResult result)
        {
            return new Tip(result);
        }

        internal class TipResult
        {
            internal TipResult()
            {
                Hex = "";
            }

            [JsonProperty("height")]
            public long Height { get; set; }

            [JsonProperty("hex")]
            public string Hex { get; set; }
        }
    }

    internal class TipList : ResponseBase
    {
        private List<Tip.TipResult> _result;

        internal TipList()
        {
            _result = new List<Tip.TipResult>();
        }

        [JsonProperty("params")]
        internal List<Tip.TipResult> Result
        {
            get { return _result; }
            set
            { 
                _result = value;
                _result.Sort((e1, e2) => (e1.Height.CompareTo(e2.Height)));
            }
        }

        public IList<ITip> List
        {
            get
            {
                var list = new List<ITip>();
                foreach (var item in Result)
                {
                    Tip tip = new Tip(item);
                    ((IAsyncResponseResult)tip).SetNetwork(((IAsyncResponseResult)this).Network);
                    list.Add(tip);
                }
                return list;
            }
        }

        internal static TipList FromJson(string json, Network network)
        {
            var tiplist = JsonConvert.DeserializeObject<TipList>(json) ?? new TipList();
            ((IAsyncResponseResult)tiplist).SetNetwork(network);
            return tiplist;
        }
    }
}
