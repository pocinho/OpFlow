// Copyright (c) 2026 Paulo Pocinho.

namespace PP.Toolkit.Operation.Tests.Canon.ControlFlow;

public class MatchTests
{
    // -------------------------------------------------------------
    // Match<T, TResult>(Func<T, TResult>, Func<Error, TResult>)
    // -------------------------------------------------------------
    [Fact]
    public void Match_Success_InvokesOnSuccess()
    {
        Operation<int> op = Operation.Success(10);

        int result = op.Match(
            onSuccess: x => x * 2,
            onFailure: _ => -1
        );

        Assert.Equal(20, result);
    }

    [Fact]
    public void Match_Failure_InvokesOnFailure()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(error);

        int result = op.Match(
            onSuccess: _ => -1,
            onFailure: err => err.Message.Length
        );

        Assert.Equal("boom".Length, result);
    }

    [Fact]
    public void Match_OnSuccessThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(5);

        Assert.Throws<Exception>(() =>
            op.Match<int, int>(
                onSuccess: _ => throw new Exception("fail"),
                onFailure: _ => 0
            )
        );
    }

    [Fact]
    public void Match_OnFailureThrows_PropagatesException()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("boom"));

        Assert.Throws<Exception>(() =>
            op.Match<int, int>(
                onSuccess: _ => 0,
                onFailure: _ => throw new Exception("fail")
            )
        );
    }

    [Fact]
    public void Match_ReturnsCorrectType()
    {
        Operation<int> op = Operation.Success(42);

        string result = op.Match(
            onSuccess: x => $"Value: {x}",
            onFailure: err => $"Error: {err.Message}"
        );

        Assert.Equal("Value: 42", result);
    }
}