using ElectrumClient.Response;
using Newtonsoft.Json;

namespace ElectrumClient.Converters
{
    internal class FeeHistogramConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<FeeHistogramPoint>);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                var result = new List<FeeHistogramPoint>();
                var arrayValue = serializer.Deserialize<List<List<long>>>(reader) ?? new List<List<long>>();
                foreach (var pair in arrayValue)
                {
                    var point = new FeeHistogramPoint(pair[0], pair[1]);
                    result.Add(point);
                }
                return result;
            }

            throw new JsonSerializationException("Cannot convert value to List<FeeHistogramPoint>");
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
