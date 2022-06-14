using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    internal class ResponseBase
    {
        [JsonProperty("jsonrpc")]
        internal string JsonRpcVersion { get; set; }

        [JsonProperty("id")]
        internal int MessageId { get; set; }

        protected ResponseBase()
        {
            JsonRpcVersion = "";
        }
    }
}
