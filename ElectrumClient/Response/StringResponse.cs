using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    public interface IString : IAsyncResponseResult
    {
        public string Value { get; }
    }

    internal class StringResponse : ResponseBase, IString
    {
        public StringResponse()
        {
            Result = "";
        }

        [JsonProperty("result")]
        internal string Result { get; set; }

        public string Value { get { return Result; } }
    }
}
