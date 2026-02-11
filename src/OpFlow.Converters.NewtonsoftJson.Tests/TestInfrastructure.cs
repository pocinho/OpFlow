// Copyright (c) 2026 Paulo Pocinho.

using Newtonsoft.Json;

namespace OpFlow.Converters.NewtonsoftJson.Tests;

public static class TestJson
{
    public static readonly JsonSerializerSettings Settings =
        new JsonSerializerSettings
        {
            Converters =
            {
                new OpFlowErrorConverter(),
                new OpFlowOperationConverter()
            },
            Formatting = Formatting.None
        };

    public static string Serialize<T>(T value) =>
        JsonConvert.SerializeObject(value, Settings);

    public static T Deserialize<T>(string json) =>
        JsonConvert.DeserializeObject<T>(json, Settings)!;
}