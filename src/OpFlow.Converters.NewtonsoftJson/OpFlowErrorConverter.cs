// Copyright (c) 2026 Paulo Pocinho.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OpFlow.Converters.NewtonsoftJson;

public sealed class OpFlowErrorConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
        => typeof(Error).IsAssignableFrom(objectType);

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is not Error error)
        {
            writer.WriteNull();
            return;
        }

        // Detect whether we are serializing the root object
        bool isRoot = string.IsNullOrEmpty(writer.Path);

        if (isRoot)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("error");
        }

        writer.WriteStartObject();

        switch (error)
        {
            case Error.Validation v:
                writer.WritePropertyName("errorType");
                writer.WriteValue("validation");

                writer.WritePropertyName("message");
                writer.WriteValue(v.Message);

                if (v.Fields is not null)
                {
                    writer.WritePropertyName("fields");
                    serializer.Serialize(writer, v.Fields);
                }
                break;

            case Error.NotFound nf:
                writer.WritePropertyName("errorType");
                writer.WriteValue("notfound");

                writer.WritePropertyName("message");
                writer.WriteValue(nf.Message);
                break;

            case Error.Unauthorized un:
                writer.WritePropertyName("errorType");
                writer.WriteValue("unauthorized");

                writer.WritePropertyName("message");
                writer.WriteValue(un.Message);
                break;

            case Error.Unexpected u:
                writer.WritePropertyName("errorType");
                writer.WriteValue("unexpected");

                writer.WritePropertyName("message");
                writer.WriteValue(u.Message);

                if (u.Exception is not null)
                {
                    writer.WritePropertyName("exception");
                    serializer.Serialize(writer, new
                    {
                        type = u.Exception.GetType().FullName,
                        message = u.Exception.Message,
                        stackTrace = u.Exception.StackTrace
                    });
                }
                break;

            default:
                throw new JsonSerializationException($"Unknown Error subtype: {error.GetType()}");
        }

        writer.WriteEndObject();

        if (isRoot)
            writer.WriteEndObject();
    }

    public override object? ReadJson(
        JsonReader reader,
        Type objectType,
        object? existingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        JObject root = JObject.Load(reader);

        // Case 1: Root-level error object: { "error": { ... } }
        JObject jo;
        if (root.TryGetValue("error", StringComparison.OrdinalIgnoreCase, out JToken? errorToken)
            && errorToken is JObject wrapped)
        {
            jo = wrapped;
        }
        else
        {
            // Case 2: Nested error object: { "errorType": "...", "message": "..." }
            jo = root;
        }

        string type = jo["errorType"]?.Value<string>()?.ToLowerInvariant()
                      ?? throw new JsonSerializationException("Missing 'errorType'");

        string message = jo["message"]?.Value<string>() ?? "Unknown error";

        return type switch
        {
            "validation" =>
                new Error.Validation(
                    message,
                    jo.TryGetValue("fields", StringComparison.OrdinalIgnoreCase, out JToken? fieldsToken)
                        ? fieldsToken.ToObject<IReadOnlyList<string>>(serializer)
                        : null),

            "notfound" =>
                new Error.NotFound(message),

            "unauthorized" =>
                new Error.Unauthorized(message),

            "unexpected" =>
                new Error.Unexpected(
                    message,
                    jo.TryGetValue("exception", StringComparison.OrdinalIgnoreCase, out JToken? exToken)
                        ? new Exception(exToken["message"]?.Value<string>())
                        : null),

            _ =>
                new Error.Unexpected($"Unknown error type '{type}'")
        };
    }
}