using NBitcoin;
using Newtonsoft.Json;

namespace ElectrumClient.Converters
{
    internal class MoneyConverterBTC : MoneyConverter<decimal>
    {
    }

    internal class MoneyConverterSats : MoneyConverter<long>
    {
    }

    internal class MoneyConverter<T> : JsonConverter where T : IComparable<T>
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Money);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) return Money.Zero;

            if (reader.TokenType == JsonToken.Integer)
            {
                long val = (long)reader.Value;
                if (val == -1) return Money.Zero;

                return new Money(val);
            }

            if (reader.TokenType == JsonToken.Float)
            {
                double val = (double)reader.Value;
                return new Money(new decimal(val), MoneyUnit.BTC);
            }

            if (reader.TokenType == JsonToken.String)
            {
                return new Money(decimal.Parse((string)reader.Value), MoneyUnit.BTC);
            }

            throw new JsonSerializationException("Cannot convert value to Money");
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
