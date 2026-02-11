// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.Representation;

public class OperationToStringTests
{
    // -------------------------------------------------------------
    // Success<T>.ToString()
    // -------------------------------------------------------------
    [Fact]
    public void ToString_OnSuccess_ReturnsFormattedString()
    {
        Operation<int> op = Operation.Success(42);

        string result = op.ToString();

        Assert.Equal("Success: 42", result);
    }

    [Fact]
    public void ToString_OnSuccess_UsesValueToString()
    {
        var obj = new { Name = "Paulo", Age = 99 };
        Operation<object> op = Operation.Success(obj);

        string result = op.ToString();

        Assert.Equal($"Success: {obj}", result);
    }

    // -------------------------------------------------------------
    // Failure<T>.ToString()
    // -------------------------------------------------------------
    [Fact]
    public void ToString_OnFailure_ReturnsFormattedString()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        string result = op.ToString();

        Assert.Equal($"Failure: {error}", result);
    }

    [Fact]
    public void ToString_OnFailure_UsesErrorToString()
    {
        Error.NotFound error = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(error);

        string result = op.ToString();

        Assert.Equal($"Failure: {error}", result);
    }
}