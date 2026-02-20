// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.Transform;

public class MatchTests
{
    // -------------------------------------------------------------
    // Match<T, TResult> (functional)
    // -------------------------------------------------------------
    [Fact]
    public void Match_Functional_Success_ReturnsMappedValue()
    {
        Operation<int> op = Operation.Success(10);

        string result = op.Match(
            onSuccess: x => $"value:{x}",
            onFailure: e => $"error:{e.Message}"
        );

        Assert.Equal("value:10", result);
    }

    [Fact]
    public void Match_Functional_Failure_ReturnsMappedError()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        string result = op.Match(
            onSuccess: x => $"value:{x}",
            onFailure: e => $"error:{e.Message}"
        );

        Assert.Equal("error:bad", result);
    }

    [Fact]
    public void Match_Functional_Throws_WhenSuccessHandlerThrows()
    {
        Operation<int> op = Operation.Success(10);

        Assert.Throws<InvalidOperationException>(() =>
            op.Match(
                onSuccess: _ => throw new InvalidOperationException("boom"),
                onFailure: _ => "ignored"
            )
        );
    }

    [Fact]
    public void Match_Functional_Throws_WhenFailureHandlerThrows()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        Assert.Throws<InvalidOperationException>(() =>
            op.Match(
                onSuccess: _ => "ignored",
                onFailure: _ => throw new InvalidOperationException("boom")
            )
        );
    }

    // -------------------------------------------------------------
    // Match<T> (side-effect)
    // -------------------------------------------------------------
    [Fact]
    public void Match_Action_Success_InvokesSuccessAction()
    {
        Operation<int> op = Operation.Success(10);
        int captured = 0;

        op.Match(
            onSuccess: x => captured = x,
            onFailure: _ => { }
        );

        Assert.Equal(10, captured);
    }

    [Fact]
    public void Match_Action_Failure_InvokesFailureAction()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        Error? captured = null;

        op.Match(
            onSuccess: _ => { },
            onFailure: e => captured = e
        );

        Assert.Equal(error, captured);
    }

    [Fact]
    public void Match_Action_Throws_WhenSuccessActionThrows()
    {
        Operation<int> op = Operation.Success(10);

        Assert.Throws<InvalidOperationException>(() =>
            op.Match(
                onSuccess: _ => throw new InvalidOperationException("boom"),
                onFailure: _ => { }
            )
        );
    }

    [Fact]
    public void Match_Action_Throws_WhenFailureActionThrows()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        Assert.Throws<InvalidOperationException>(() =>
            op.Match(
                onSuccess: _ => { },
                onFailure: _ => throw new InvalidOperationException("boom")
            )
        );
    }

    // -------------------------------------------------------------
    // MatchAsync<T> (side-effect async)
    // -------------------------------------------------------------
    [Fact]
    public async Task MatchAsync_Action_Success_InvokesSuccessAction()
    {
        Operation<int> op = Operation.Success(10);
        int captured = 0;

        await op.MatchAsync(
            onSuccessAsync: async x =>
            {
                await Task.Delay(1);
                captured = x;
            },
            onFailureAsync: _ => Task.CompletedTask
        );

        Assert.Equal(10, captured);
    }

    [Fact]
    public async Task MatchAsync_Action_Failure_InvokesFailureAction()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        Error? captured = null;

        await op.MatchAsync(
            onSuccessAsync: _ => Task.CompletedTask,
            onFailureAsync: async e =>
            {
                await Task.Delay(1);
                captured = e;
            }
        );

        Assert.Equal(error, captured);
    }

    [Fact]
    public async Task MatchAsync_Action_Throws_WhenSuccessActionThrows()
    {
        Operation<int> op = Operation.Success(10);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            op.MatchAsync(
                onSuccessAsync: async _ =>
                {
                    await Task.Delay(1);
                    throw new InvalidOperationException("boom");
                },
                onFailureAsync: _ => Task.CompletedTask
            )
        );
    }

    [Fact]
    public async Task MatchAsync_Action_Throws_WhenFailureActionThrows()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            op.MatchAsync(
                onSuccessAsync: _ => Task.CompletedTask,
                onFailureAsync: async _ =>
                {
                    await Task.Delay(1);
                    throw new InvalidOperationException("boom");
                }
            )
        );
    }
}