using ElectrumClient.Converters;
using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    public interface IServerFeatures
    {
        public string GenesisHash { get; }
        public IList<IHost> Hosts { get; }
        public string ProtocolMax { get; }
        public string ProtocolMin { get; }
        public long? Pruning { get; }
        public string ServerVersion { get; }
        public string HashFunction { get; }
    }

    public interface IHost
    {
        public string HostName { get; }
        public long? TcpPort { get; }
        public long? SslPort { get; }
    }

    internal class ServerFeatures : IServerFeatures
    {
        public ServerFeatures()
        {
            Result = new ServerFeaturesResult();
        }

        [JsonProperty("result")]
        internal ServerFeaturesResult Result { get; set; }

        public string GenesisHash { get { return Result.GenesisHash; } }
        public IList<IHost> Hosts
        {
            get
            {
                IList<IHost> res = new List<IHost>();
                foreach (var host in Result.Hosts) res.Add(host);
                return res;
            }
        }
        public string ProtocolMax { get { return Result.ProtocolMax; } }
        public string ProtocolMin { get { return Result.ProtocolMin; } }
        public long? Pruning { get { return Result.Pruning; } }
        public string ServerVersion { get { return Result.ServerVersion; } }
        public string HashFunction { get { return Result.HashFunction; } }

        internal class ServerFeaturesResult
        {
            internal ServerFeaturesResult()
            {
                GenesisHash = "";
                Hosts = new List<Host>();
                ProtocolMax = "";
                ProtocolMin = "";
                ServerVersion = "";
                HashFunction = "";
            }

            [JsonProperty("genesis_hash")]
            public string GenesisHash { get; set;}

            [JsonProperty("hosts")]
            [JsonConverter(typeof(HostConverter))]
            public List<Host> Hosts { get; set; }

            [JsonProperty("protocol_max")]
            public string ProtocolMax { get; set; }

            [JsonProperty("protocol_min")]
            public string ProtocolMin { get; set; }

            [JsonProperty("pruning")]
            public long? Pruning { get; set; }

            [JsonProperty("server_version")]
            public string ServerVersion { get; set; }

            [JsonProperty("hash_function")]
            public string HashFunction { get; set; }
        }

        internal class Host : IHost
        {
            public Host() : this("", null, null)
            {
            }

            public Host(string hostName, long? tcpPort, long? sslPort)
            {
                HostName = hostName;
                TcpPort = tcpPort;
                SslPort = sslPort;
            }

            public string HostName { get; set; }
            public long? TcpPort { get; set; }
            public long? SslPort { get; set; }
        }
    }
}
