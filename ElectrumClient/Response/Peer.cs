using ElectrumClient.Converters;
using Newtonsoft.Json;

namespace ElectrumClient.Response
{
    internal interface IPeerList : IAsyncResponseResult
    {
        IList<IPeer> List { get; }
    }
    public interface IPeer
    {
        public string Ip { get; }
        public string HostName { get; }
        public string ProtocolMax { get; }
        public long? Pruning { get; }
        public long? TcpPort { get;  }
        public long? SslPort { get; }
    }

    internal class PeerList : ResponseBase, IPeerList
    {
        public PeerList()
        {
            Result = new List<Peer>();
        }

        [JsonProperty("result")]
        [JsonConverter(typeof(PeerListConverter))]
        internal List<Peer> Result { get; set; }

        public IList<IPeer> List
        {
            get
            {
                var list = new List<IPeer>();
                foreach (var item in Result) list.Add(item);
                return list;
            }
        }
    }

    internal class Peer : IPeer
    {
        public Peer()
        {
            Ip = "";
            HostName = "";
            ProtocolMax = "";
        }

        public string Ip { get; set; }
        public string HostName { get; set; }
        public string ProtocolMax { get; set; }
        public long? Pruning { get; set; }
        public long? TcpPort { get; set; }
        public long? SslPort { get; set; }

        internal void setProtocolMax(string feature)
        {
            if (feature == "") return;
            if (feature[0] == 'v') ProtocolMax = feature.Substring(1);
        }

        internal void setPruning(string feature)
        {
            if (feature == "") Pruning = null;
            if (feature[0] == 'p') Pruning = long.Parse(feature.Substring(1));
        }

        internal void setTcpPort(string feature)
        {
            if (feature == "") TcpPort = null;
            if (feature[0] == 't')
            {
                if (feature.Length == 1) TcpPort = 50001;
                else TcpPort = long.Parse(feature.Substring(1));
            }
        }

        internal void setSslPort(string feature)
        {
            if (feature == "") SslPort = null;
            if (feature[0] == 's')
            {
                if (feature.Length == 1) SslPort = 50002;
                else SslPort = long.Parse(feature.Substring(1));
            }
        }
    }
}
