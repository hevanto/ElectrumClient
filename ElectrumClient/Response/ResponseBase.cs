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

        internal Network _network;

        Network IAsyncResponseResult.Network { get { return _network; } }

        protected ResponseBase()
        {
            JsonRpcVersion = "";
            _network = Network.TestNet; // Safe default
        }

        void IAsyncResponseResult.SetNetwork(Network network)
        {
            this._network = network;
        }
    }
}
