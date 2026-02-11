// Copyright (c) 2026 Paulo Pocinho.

using OpFlow.Extensions;

namespace OpFlow.Tests.Core;

public class OperationTests
{
    // ------------------------------------------------------------
    // 1. Success construction
    // ------------------------------------------------------------

    [Fact]
    public void Success_CreatesCorrectValue()
    {
        Operation<int>.Success op = new Operation<int>.Success(42);

        Assert.True(op.IsSuccess());
        Assert.True(op.TryGet(out int value));
        Assert.Equal(42, value);
    }

    [Fact]
    public void Success_ToString_ContainsValue()
    {
        Operation<string>.Success op = new Operation<string>.Success("hello");

        string text = op.ToString();

        Assert.Contains("Success", text);
        Assert.Contains("hello", text);
    }

    // ------------------------------------------------------------
    // 2. Failure construction
    // ------------------------------------------------------------

    [Fact]
    public void Failure_CreatesCorrectError()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int>.Failure op = new Operation<int>.Failure(error);

        Assert.True(op.IsFailure());
        Assert.True(op.TryGetError(out Error e));
        Assert.Equal(error, e);
    }

    [Fact]
    public void Failure_ToString_ContainsError()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int>.Failure op = new Operation<int>.Failure(error);

        string text = op.ToString();

        Assert.Contains("Failure", text);
        Assert.Contains("bad", text);
    }

    // ------------------------------------------------------------
    // 3. Implicit conversions
    // ------------------------------------------------------------

    [Fact]
    public void ImplicitConversion_FromError()
    {
        Error.Validation error = new Error.Validation("bad");

        Operation<int> op = error;

        Assert.True(op.IsFailure());
        Assert.True(op.TryGetError(out Error e));
        Assert.Equal(error, e);
    }

    [Fact]
    public void ImplicitConversion_FromException()
    {
        InvalidOperationException ex = new InvalidOperationException("boom");

        Operation<int> op = ex;

        Assert.True(op.IsFailure());
        Assert.True(op.TryGetError(out Error e));

        Error.Unexpected unexpected = Assert.IsType<Error.Unexpected>(e);
        Assert.Equal("boom", unexpected.Message);
        Assert.Equal(ex, unexpected.Exception);
    }

    // ------------------------------------------------------------
    // 4. TryGet and TryGetError behavior
    // ------------------------------------------------------------

    [Fact]
    public void TryGet_ReturnsFalse_ForFailure()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = new Operation<int>.Failure(error);

        Assert.False(op.TryGet(out int _));
    }

    [Fact]
    public void TryGetError_ReturnsFalse_ForSuccess()
    {
        Operation<int> op = new Operation<int>.Success(10);

        Assert.False(op.TryGetError(out Error _));
    }

    // ------------------------------------------------------------
    // 5. Equality semantics
    // ------------------------------------------------------------

    [Fact]
    public void Success_Equality_Works()
    {
        Operation<int> op1 = new Operation<int>.Success(10);
        Operation<int> op2 = new Operation<int>.Success(10);

        Assert.Equal(op1, op2);
        Assert.True(op1 == op2);
    }

    [Fact]
    public void Failure_Equality_Works()
    {
        Error.Validation error = new Error.Validation("bad");

        Operation<int> op1 = new Operation<int>.Failure(error);
        Operation<int> op2 = new Operation<int>.Failure(error);

        Assert.Equal(op1, op2);
        Assert.True(op1 == op2);
    }

    [Fact]
    public void Success_And_Failure_AreNotEqual()
    {
        Operation<int> op1 = new Operation<int>.Success(10);
        Operation<int> op2 = new Operation<int>.Failure(new Error.Validation("bad"));

        Assert.NotEqual(op1, op2);
    }

    // ------------------------------------------------------------
    // 6. Pattern matching
    // ------------------------------------------------------------

    [Fact]
    public void PatternMatching_WorksForSuccess()
    {
        Operation<int> op = new Operation<int>.Success(10);

        string result = op switch
        {
            Operation<int>.Success s => $"value={s.Result}",
            Operation<int>.Failure f => "fail",
            _ => "unknown"
        };

        Assert.Equal("value=10", result);
    }

    [Fact]
    public void PatternMatching_WorksForFailure()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = new Operation<int>.Failure(error);

        string result = op switch
        {
            Operation<int>.Success => "ok",
            Operation<int>.Failure { Error: Error.Validation v } => $"error={v.Message}",
            _ => "unknown"
        };

        Assert.Equal("error=bad", result);
    }

    // ------------------------------------------------------------
    // 7. Type safety
    // ------------------------------------------------------------

    [Fact]
    public void Success_And_Failure_AreDifferentTypes()
    {
        Operation<int>.Success s = new Operation<int>.Success(10);
        Operation<int>.Failure f = new Operation<int>.Failure(new Error.Validation("bad"));

        Assert.IsType<Operation<int>.Success>(s);
        Assert.IsType<Operation<int>.Failure>(f);
    }

    [Fact]
    public void Operation_IsAbstract()
    {
        Assert.True(typeof(Operation<int>).IsAbstract);
    }
}