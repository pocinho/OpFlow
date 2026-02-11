// Copyright (c) 2026 Paulo Pocinho.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OpFlow.Converters.NewtonsoftJson;

public sealed class OpFlowErrorConverter : JsonConverter<Error>
{
    public override void WriteJson(JsonWriter writer, Error? value, JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteStartObject();
        writer.WritePropertyName("error");
        writer.WriteStartObject();

        switch (value)
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
                throw new JsonSerializationException($"Unknown Error subtype: {value.GetType()}");
        }

        writer.WriteEndObject();
        writer.WriteEndObject();
    }

    public override Error? ReadJson(
        JsonReader reader,
        Type objectType,
        Error? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        JObject root = JObject.Load(reader);

        if (!root.TryGetValue("error", StringComparison.OrdinalIgnoreCase, out JToken? errorToken)
            || errorToken is not JObject jo)
            throw new JsonSerializationException("Missing 'error' object");

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