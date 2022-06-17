using NBitcoin;
using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    internal interface IGenericResponse : IAsyncResponseResult
    {
        public string Method { get; }
        public string Error { get; }
        public bool IsError { get; }
        public string Raw { get; }
    }

    internal class GenericResponse : ResponseBase, IGenericResponse
    {
        public GenericResponse()
        {
            Method = "";
            Raw = "";
            Error = "";
        }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        public bool IsError {  get { return Error != ""; } }

        public string Raw { get; set; }

        internal static GenericResponse FromJson(string json, Network network)
        {
            var resp = JsonConvert.DeserializeObject<GenericResponse>(json) ?? new GenericResponse();
            resp.Raw = json;
            resp.network = network;
            return resp;
        }
    }
}
