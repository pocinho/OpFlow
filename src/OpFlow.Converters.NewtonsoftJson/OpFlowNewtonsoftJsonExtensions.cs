// Copyright (c) 2026 Paulo Pocinho.

using Newtonsoft.Json;

namespace OpFlow.Converters.NewtonsoftJson;

public static class OpFlowNewtonsoftJsonExtensions
{
    public static JsonSerializerSettings AddOpFlowConverters(this JsonSerializerSettings settings)
    {
        settings.Converters.Add(new OpFlowErrorConverter());
        settings.Converters.Add(new OpFlowOperationConverter());
        return settings;
    }
}

