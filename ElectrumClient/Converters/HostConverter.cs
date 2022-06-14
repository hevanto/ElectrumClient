using ElectrumClient.Response;
using Newtonsoft.Json;

namespace ElectrumClient.Converters
{
    internal class HostConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<ServerFeatures.Host>);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                var objValue = serializer.Deserialize<Dictionary<string, dynamic>>(reader);
                if (objValue == null) return new List<ServerFeatures.Host>();

                var result = new List<ServerFeatures.Host>();
                foreach (var kv in objValue)
                {
                    var hostName = kv.Key;
                    var tcpPort = kv.Value.tcp_port;
                    var sslPort = kv.Value.ssl_port;

                    result.Add(new ServerFeatures.Host(hostName, (long?)tcpPort, (long?)sslPort));
                }
                return result;
            }

            throw new JsonSerializationException("Cannot convert value to List<ServerFeatres.Host>");
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
