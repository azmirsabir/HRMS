using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.converters;

public class NullableFloatJsonConverter : JsonConverter<float?>
{
    public override float? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetSingle(out var number))
            {
                return number;
            }
            return null;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString();
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            if (float.TryParse(str, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var valueInvariant))
            {
                return valueInvariant;
            }

            if (float.TryParse(str, out var valueCurrent))
            {
                return valueCurrent;
            }

            return null;
        }

        // Fallback: attempt default deserialization; if it fails, treat as null
        try
        {
            return JsonSerializer.Deserialize<float?>(ref reader, options);
        }
        catch
        {
            return null;
        }
    }

    public override void Write(Utf8JsonWriter writer, float? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteNumberValue(value.Value);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}