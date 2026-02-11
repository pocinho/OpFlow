// Copyright (c) 2026 Paulo Pocinho.

using Newtonsoft.Json;

namespace OpFlow.Converters.NewtonsoftJson.Tests.Core;

public class OpFlowErrorConverterTests
{
    private static readonly JsonSerializerSettings Settings =
        new JsonSerializerSettings
        {
            Converters =
            {
                new OpFlowErrorConverter()
            },
            Formatting = Formatting.None
        };

    private static string Serialize(Error error) =>
        JsonConvert.SerializeObject(error, Settings);

    private static Error Deserialize(string json) =>
        JsonConvert.DeserializeObject<Error>(json, Settings)!;

    // -------------------------------------------------------------
    // VALIDATION
    // -------------------------------------------------------------

    [Fact]
    public void Validation_RoundTrip()
    {
        Error.Validation error = new Error.Validation("Invalid fields", new[] { "Name", "Email" });

        string json = Serialize(error);
        Error result = Deserialize(json);

        Error.Validation v = Assert.IsType<Error.Validation>(result);
        Assert.Equal("Invalid fields", v.Message);
        Assert.Equal(new[] { "Name", "Email" }, v.Fields);
    }

    // -------------------------------------------------------------
    // NOT FOUND
    // -------------------------------------------------------------

    [Fact]
    public void NotFound_RoundTrip()
    {
        Error.NotFound error = new Error.NotFound("Item missing");

        string json = Serialize(error);
        Error result = Deserialize(json);

        Error.NotFound nf = Assert.IsType<Error.NotFound>(result);
        Assert.Equal("Item missing", nf.Message);
    }

    // -------------------------------------------------------------
    // UNAUTHORIZED
    // -------------------------------------------------------------

    [Fact]
    public void Unauthorized_RoundTrip()
    {
        Error.Unauthorized error = new Error.Unauthorized("No access");

        string json = Serialize(error);
        Error result = Deserialize(json);

        Error.Unauthorized un = Assert.IsType<Error.Unauthorized>(result);
        Assert.Equal("No access", un.Message);
    }

    // -------------------------------------------------------------
    // UNEXPECTED
    // -------------------------------------------------------------

    [Fact]
    public void Unexpected_WithException_RoundTrip()
    {
        Error.Unexpected error = new Error.Unexpected("Boom", new InvalidOperationException("Oops"));

        string json = Serialize(error);
        Error result = Deserialize(json);

        Error.Unexpected u = Assert.IsType<Error.Unexpected>(result);
        Assert.Equal("Boom", u.Message);
        Assert.NotNull(u.Exception);
        Assert.Contains("Oops", u.Exception!.Message);
    }

    [Fact]
    public void Unexpected_WithoutException_RoundTrip()
    {
        Error.Unexpected error = new Error.Unexpected("Boom");

        string json = Serialize(error);
        Error result = Deserialize(json);

        Error.Unexpected u = Assert.IsType<Error.Unexpected>(result);
        Assert.Equal("Boom", u.Message);
        Assert.Null(u.Exception);
    }

    // -------------------------------------------------------------
    // INVALID CASES
    // -------------------------------------------------------------

    [Fact]
    public void MissingErrorType_Throws()
    {
        string json = @"{ ""error"": { ""message"": ""x"" } }";

        Assert.Throws<JsonSerializationException>(() =>
            Deserialize(json));
    }

    [Fact]
    public void UnknownErrorType_ProducesUnexpected()
    {
        string json = @"{ ""error"": { ""errorType"": ""weird"", ""message"": ""x"" } }";

        Error result = Deserialize(json);

        Error.Unexpected u = Assert.IsType<Error.Unexpected>(result);
        Assert.Contains("Unknown error type", u.Message);
    }
}