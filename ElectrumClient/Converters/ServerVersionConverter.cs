using ElectrumClient.Response;
using Newtonsoft.Json;

namespace ElectrumClient.Converters
{
    internal class ServerVersionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ServerVersion.ServerVersionResult);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                var list = serializer.Deserialize<List<string>>(reader) ?? new List<string>();
                if (list.Count != 2) throw new JsonSerializationException("Invalid data received");

                var result = new ServerVersion.ServerVersionResult();
                result.SoftwareVersion = list[0];
                result.ProtocolVersion = list[1];
                return result;
            }
            throw new JsonSerializationException("Cannot convert value to List<ServerVersion.ServerVersionResult>");
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
