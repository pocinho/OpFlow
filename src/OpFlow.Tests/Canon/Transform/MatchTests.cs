// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.Transform;

public class MatchTests
{
    // -------------------------------------------------------------
    // Match(Operation<T>, Func<T,R>, Func<Error,R>)
    // -------------------------------------------------------------
    [Fact]
    public void Match_Success_InvokesOnSuccess_AndReturnsValue()
    {
        Operation<int> op = Operation.Success(10);

        string result = op.Match(
            onSuccess: x => $"Value: {x}",
            onFailure: e => $"Error: {e.Message}"
        );

        Assert.Equal("Value: 10", result);
    }

    [Fact]
    public void Match_Failure_InvokesOnFailure_AndReturnsValue()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        string result = op.Match(
            onSuccess: x => $"Value: {x}",
            onFailure: e => $"Error: {e.Message}"
        );

        Assert.Equal("Error: bad", result);
    }

    [Fact]
    public void Match_CallbackThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(10);

        Assert.Throws<InvalidOperationException>(() =>
            op.Match(
                onSuccess: _ => throw new InvalidOperationException("boom"),
                onFailure: _ => "ignored"
            )
        );
    }

    // -------------------------------------------------------------
    // MatchAsync(Operation<T>, Func<T,R>, Func<Error,R>)
    // -------------------------------------------------------------
    [Fact]
    public async Task MatchAsync_Operation_Success_InvokesOnSuccess_AndReturnsValue()
    {
        Operation<int> op = Operation.Success(10);

        string result = await op.MatchAsync(
            onSuccess: x => $"Value: {x}",
            onFailure: e => $"Error: {e.Message}"
        );

        Assert.Equal("Value: 10", result);
    }

    [Fact]
    public async Task MatchAsync_Operation_Failure_InvokesOnFailure_AndReturnsValue()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        string result = await op.MatchAsync(
            onSuccess: x => $"Value: {x}",
            onFailure: e => $"Error: {e.Message}"
        );

        Assert.Equal("Error: bad", result);
    }

    [Fact]
    public async Task MatchAsync_Operation_CallbackThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(10);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            op.MatchAsync(
                onSuccess: _ => throw new InvalidOperationException("boom"),
                onFailure: _ => "ignored"
            )
        );
    }

    // -------------------------------------------------------------
    // MatchAsync(Task<Operation<T>>, Func<T,R>, Func<Error,R>)
    // (canonical async shape)
    // -------------------------------------------------------------
    [Fact]
    public async Task MatchAsync_Task_Success_InvokesOnSuccess_AndReturnsValue()
    {
        Task<Operation<int>> opTask = Task.FromResult(Operation.Success(10));

        string result = await opTask.MatchAsync(
            onSuccess: x => $"Value: {x}",
            onFailure: e => $"Error: {e.Message}"
        );

        Assert.Equal("Value: 10", result);
    }

    [Fact]
    public async Task MatchAsync_Task_Failure_InvokesOnFailure_AndReturnsValue()
    {
        Error.Validation error = new Error.Validation("bad");
        Task<Operation<int>> opTask = Task.FromResult(Operation.FailureOf<int>(error));

        string result = await opTask.MatchAsync(
            onSuccess: x => $"Value: {x}",
            onFailure: e => $"Error: {e.Message}"
        );

        Assert.Equal("Error: bad", result);
    }

    [Fact]
    public async Task MatchAsync_Task_CallbackThrows_PropagatesException()
    {
        Task<Operation<int>> opTask = Task.FromResult(Operation.Success(10));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            opTask.MatchAsync(
                onSuccess: _ => throw new InvalidOperationException("boom"),
                onFailure: _ => "ignored"
            )
        );
    }
}