using NBitcoin;
using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    internal class ResponseBase : IAsyncResponseResult
    {
        [JsonProperty("jsonrpc")]
        internal string JsonRpcVersion { get; set; }

        [JsonProperty("id")]
        internal int MessageId { get; set; }

        internal Network network { get; set; }

        protected ResponseBase()
        {
            JsonRpcVersion = "";
            network = Network.TestNet; // Safe default
        }

        void IAsyncResponseResult.SetNetwork(Network network)
        {
            this.network = network;
        }
    }
}
