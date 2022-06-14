using ElectrumClient.Response;
using Newtonsoft.Json;

namespace ElectrumClient.Converters
{
    internal class PeerListConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<Peer>);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                var listValue = serializer.Deserialize<List<List<dynamic>>>(reader);
                if (listValue == null) return new List<Peer>();

                var result = new List<Peer>();
                foreach (var item in listValue)
                {
                    var peerElements = item.ToArray();
                    if (peerElements.Length != 3) continue;

                    var peer = new Peer();
                    peer.Ip = (string)(peerElements[0] ?? "");
                    peer.HostName = (string)(peerElements[1] ?? "");

                    var features = peerElements[2];
                    foreach (var kv in features)
                    {
                        var elem = (string)(kv.Value ?? "");
                        if (elem == null) continue;

                        switch (elem[0])
                        {
                            case 'v': peer.setProtocolMax(elem); break;
                            case 'p': peer.setPruning(elem); break;
                            case 't': peer.setTcpPort(elem); break;
                            case 's': peer.setSslPort(elem); break;
                        }
                    }

                    result.Add(peer);
                }
                return result;
            }
            throw new JsonSerializationException("Cannot convert value to List<Peer>");
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
