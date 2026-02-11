// Copyright (c) 2026 Paulo Pocinho.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OpFlow.Converters.NewtonsoftJson;

public sealed class OpFlowOperationConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        // Case 1: Operation<T>
        if (objectType.IsGenericType &&
            objectType.GetGenericTypeDefinition() == typeof(Operation<>))
            return true;

        // Case 2: Operation<T>.Success or Operation<T>.Failure
        Type? baseType = objectType.BaseType;
        return baseType?.IsGenericType == true &&
               baseType.GetGenericTypeDefinition() == typeof(Operation<>);
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteStartObject();
        writer.WritePropertyName("operation");
        writer.WriteStartObject();

        Type type = value.GetType();
        Type t = type.BaseType!.GetGenericArguments()[0];

        if (type.Name.StartsWith("Success", StringComparison.OrdinalIgnoreCase))
        {
            writer.WritePropertyName("kind");
            writer.WriteValue("success");

            writer.WritePropertyName("result");
            object? result = type.GetProperty("Result")!.GetValue(value);
            serializer.Serialize(writer, result);
        }
        else if (type.Name.StartsWith("Failure", StringComparison.OrdinalIgnoreCase))
        {
            writer.WritePropertyName("kind");
            writer.WriteValue("failure");

            writer.WritePropertyName("error");
            object? error = type.GetProperty("Error")!.GetValue(value);
            serializer.Serialize(writer, error);
        }
        else
        {
            throw new JsonSerializationException($"Unknown Operation<T> subtype: {type}");
        }

        writer.WriteEndObject();
        writer.WriteEndObject();
    }

    public override object? ReadJson(
        JsonReader reader,
        Type objectType,
        object? existingValue,
        JsonSerializer serializer)
    {
        JObject root = JObject.Load(reader);

        if (!root.TryGetValue("operation", StringComparison.OrdinalIgnoreCase, out JToken? opToken)
            || opToken is not JObject jo)
            throw new JsonSerializationException("Missing 'operation' object");

        string kind = jo["kind"]?.Value<string>()?.ToLowerInvariant()
            ?? throw new JsonSerializationException("Missing 'kind'");

        Type t = objectType.GetGenericArguments()[0];

        return kind switch
        {
            "success" =>
                Activator.CreateInstance(
                    typeof(Operation<>.Success).MakeGenericType(t),
                    jo.TryGetValue("result", StringComparison.OrdinalIgnoreCase, out JToken? resultToken)
                        ? resultToken.ToObject(t, serializer)
                            ?? throw new JsonSerializationException("'result' cannot be null")
                        : throw new JsonSerializationException("Success must contain 'result'")
                ),

            "failure" =>
                Activator.CreateInstance(
                    typeof(Operation<>.Failure).MakeGenericType(t),
                    jo.TryGetValue("error", StringComparison.OrdinalIgnoreCase, out JToken? errorToken)
                        ? errorToken.ToObject<Error>(serializer)
                            ?? throw new JsonSerializationException("'error' cannot be null")
                        : throw new JsonSerializationException("Failure must contain 'error'")
                ),

            _ => throw new JsonSerializationException($"Unknown Operation<T> kind '{kind}'")
        };
    }
}