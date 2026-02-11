// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Converters.NewtonsoftJson.Tests.Core;

using Newtonsoft.Json;
using Xunit;

public class OpFlowOperationConverterTests
{
    private static readonly JsonSerializerSettings Settings =
        new JsonSerializerSettings
        {
            Converters =
            {
                new OpFlowErrorConverter(),
                new OpFlowOperationConverter()
            },
            Formatting = Formatting.None
        };

    private static string Serialize<T>(Operation<T> op) =>
        JsonConvert.SerializeObject(op, Settings);

    private static Operation<T> Deserialize<T>(string json) =>
        JsonConvert.DeserializeObject<Operation<T>>(json, Settings)!;

    // -------------------------------------------------------------
    // SUCCESS CASES
    // -------------------------------------------------------------

    [Fact]
    public void Success_RoundTrip_String()
    {
        Operation<string>.Success op = new Operation<string>.Success("Hello");

        string json = Serialize(op);
        Operation<string> result = Deserialize<string>(json);

        Operation<string>.Success success = Assert.IsType<Operation<string>.Success>(result);
        Assert.Equal("Hello", success.Result);
    }

    [Fact]
    public void Success_RoundTrip_Int()
    {
        Operation<int>.Success op = new Operation<int>.Success(42);

        string json = Serialize(op);
        Operation<int> result = Deserialize<int>(json);

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(42, success.Result);
    }

    [Fact]
    public void Success_MissingResult_Throws()
    {
        string json = @"{ ""operation"": { ""kind"": ""success"" } }";

        Assert.Throws<JsonSerializationException>(() =>
            Deserialize<string>(json));
    }

    // -------------------------------------------------------------
    // FAILURE CASES
    // -------------------------------------------------------------

    [Fact]
    public void Failure_RoundTrip_NotFound()
    {
        Operation<string>.Failure op =
            new Operation<string>.Failure(new Error.NotFound("Missing"));

        string json = Serialize(op);
        Operation<string> result = Deserialize<string>(json);

        Operation<string>.Failure failure = Assert.IsType<Operation<string>.Failure>(result);
        Error.NotFound notFound = Assert.IsType<Error.NotFound>(failure.Error);

        Assert.Equal("Missing", notFound.Message);
    }

    [Fact]
    public void Failure_RoundTrip_Validation()
    {
        Operation<string>.Failure op = new Operation<string>.Failure(
            new Error.Validation("Bad", new[] { "Field1" }));

        string json = Serialize(op);
        Operation<string> result = Deserialize<string>(json);

        Operation<string>.Failure failure = Assert.IsType<Operation<string>.Failure>(result);
        Error.Validation validation = Assert.IsType<Error.Validation>(failure.Error);

        Assert.Equal("Bad", validation.Message);
        Assert.Equal(new[] { "Field1" }, validation.Fields);
    }

    [Fact]
    public void Failure_MissingError_Throws()
    {
        string json = @"{ ""operation"": { ""kind"": ""failure"" } }";

        Assert.Throws<JsonSerializationException>(() =>
            Deserialize<string>(json));
    }

    // -------------------------------------------------------------
    // INVALID CASES
    // -------------------------------------------------------------

    [Fact]
    public void UnknownKind_Throws()
    {
        string json = @"{ ""operation"": { ""kind"": ""weird"" } }";

        Assert.Throws<JsonSerializationException>(() =>
            Deserialize<string>(json));
    }

    [Fact]
    public void MissingOperationObject_Throws()
    {
        string json = @"{ ""op"": { ""kind"": ""success"", ""result"": ""x"" } }";

        Assert.Throws<JsonSerializationException>(() =>
            Deserialize<string>(json));
    }

    // -------------------------------------------------------------
    // NESTED ERROR SERIALIZATION
    // -------------------------------------------------------------

    [Fact]
    public void Failure_WithUnexpectedError_RoundTrip()
    {
        Operation<string>.Failure op = new Operation<string>.Failure(
            new Error.Unexpected("Boom", new InvalidOperationException("Oops")));

        string json = Serialize(op);
        Operation<string> result = Deserialize<string>(json);

        Operation<string>.Failure failure = Assert.IsType<Operation<string>.Failure>(result);
        Error.Unexpected unexpected = Assert.IsType<Error.Unexpected>(failure.Error);

        Assert.Equal("Boom", unexpected.Message);
        Assert.NotNull(unexpected.Exception);
        Assert.Contains("Oops", unexpected.Exception!.Message);
    }
}