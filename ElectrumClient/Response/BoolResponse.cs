using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    public interface IBool
    {
        public bool Value { get; }
    }

    internal class BoolResponse : ResponseBase, IBool
    {
        public BoolResponse()
        {
        }

        [JsonProperty("result")]
        internal bool Result { get; set; }

        public bool Value { get { return Result; } }
    }
}
