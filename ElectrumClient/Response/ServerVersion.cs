using ElectrumClient.Converters;
using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    public interface IServerVersion : IAsyncResponseResult
    {
        public string SoftwareVersion { get; }
        public string ProtocolVersion { get; }
    }

    internal class ServerVersion : ResponseBase, IServerVersion
    {
       public ServerVersion()
        {
            Result = new ServerVersionResult();
        }

        [JsonProperty("result")]
        [JsonConverter(typeof(ServerVersionConverter))]
        internal ServerVersionResult Result { get; set; }

        public string SoftwareVersion { get { return Result.SoftwareVersion; } }
        public string ProtocolVersion { get { return Result.ProtocolVersion; } }

        internal class ServerVersionResult
        {
            public ServerVersionResult()
            {
                SoftwareVersion = "";
                ProtocolVersion = "";
            }

            internal string SoftwareVersion { get; set; }
            internal string ProtocolVersion { get; set; }
        }
    }
}
