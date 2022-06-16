using ElectrumClient.Response;
using Newtonsoft.Json;

namespace ElectrumClient.Converters
{
    internal class EmbeddedErrorConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) return null;

            if (reader.TokenType == JsonToken.String)
            {
                var value = (string)reader.Value;
                var colonIndex = value.IndexOf(':');
                if (colonIndex == -1) return new Error.ErrorResult(value);

                var descr = value.Substring(0, colonIndex).Trim();
                value = value.Substring(colonIndex+1).Trim();
                var err = JsonConvert.DeserializeObject<Error.ErrorResult>(value) ?? new Error.ErrorResult(descr);
                err.Description = descr;
                return err;
            }
            throw new JsonSerializationException("Cannoe convert value to error json");
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
