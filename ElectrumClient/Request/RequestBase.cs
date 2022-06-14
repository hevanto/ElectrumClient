using Newtonsoft.Json;

namespace ElectrumClient.Request
{
    internal class RequestBase
    {
        internal RequestBase() : this("")
        {
        }

        internal RequestBase(string method)
        {
            Method = method;
            Params = new object[0];
        }

        internal RequestBase(string method, params object[] prms)
        {
            Method = method;
            Params = prms;
        }

        [JsonProperty("id")]
        public int MessageId { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("params")]
        public object Params { get; set; }

        public byte[] GetRequestData<T>()
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(ToJson<T>());
            return data;
        }

        protected string ToJson<T>()
        {
            return JsonConvert.SerializeObject(this) + "\n";
        }
    }
}
