// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.SideEffects;

public class FinallyTests
{
    // -------------------------------------------------------------
    // Finally(Operation<T>, Action<T>, Action<Error>)
    // -------------------------------------------------------------
    [Fact]
    public void Finally_Success_InvokesOnSuccess_AndReturnsSameSuccess()
    {
        Operation<int> op = Operation.Success(10);
        int? captured = null;
        bool failureCalled = false;

        Operation<int> result = op.Finally(
            onSuccess: x => captured = x,
            onFailure: _ => failureCalled = true
        );

        Assert.Equal(10, captured);
        Assert.False(failureCalled);

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, success.Result);
    }

    [Fact]
    public void Finally_Failure_InvokesOnFailure_AndReturnsSameFailure()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        bool successCalled = false;
        Error? captured = null;

        Operation<int> result = op.Finally(
            onSuccess: _ => successCalled = true,
            onFailure: e => captured = e
        );

        Assert.False(successCalled);
        Assert.Equal(error, captured);

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    [Fact]
    public void Finally_CallbackThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(10);

        Assert.Throws<InvalidOperationException>(() =>
            op.Finally(
                onSuccess: _ => throw new InvalidOperationException("boom"),
                onFailure: _ => { }
            )
        );
    }

    // -------------------------------------------------------------
    // FinallyAsync(Operation<T>, Action<T>, Action<Error>)
    // -------------------------------------------------------------
    [Fact]
    public async Task FinallyAsync_Operation_Success_InvokesOnSuccess_AndReturnsSameSuccess()
    {
        Operation<int> op = Operation.Success(10);
        int? captured = null;
        bool failureCalled = false;

        Operation<int> result = await op.FinallyAsync(
            onSuccess: x => captured = x,
            onFailure: _ => failureCalled = true
        );

        Assert.Equal(10, captured);
        Assert.False(failureCalled);

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, success.Result);
    }

    [Fact]
    public async Task FinallyAsync_Operation_Failure_InvokesOnFailure_AndReturnsSameFailure()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        bool successCalled = false;
        Error? captured = null;

        Operation<int> result = await op.FinallyAsync(
            onSuccess: _ => successCalled = true,
            onFailure: e => captured = e
        );

        Assert.False(successCalled);
        Assert.Equal(error, captured);

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    [Fact]
    public async Task FinallyAsync_Operation_CallbackThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(10);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            op.FinallyAsync(
                onSuccess: _ => throw new InvalidOperationException("boom"),
                onFailure: _ => { }
            )
        );
    }

    // -------------------------------------------------------------
    // FinallyAsync(Task<Operation<T>>, Action<T>, Action<Error>)
    // (canonical async shape)
    // -------------------------------------------------------------
    [Fact]
    public async Task FinallyAsync_Task_Success_InvokesOnSuccess_AndReturnsSameSuccess()
    {
        Task<Operation<int>> opTask = Task.FromResult(Operation.Success(10));
        int? captured = null;
        bool failureCalled = false;

        Operation<int> result = await opTask.FinallyAsync(
            onSuccess: x => captured = x,
            onFailure: _ => failureCalled = true
        );

        Assert.Equal(10, captured);
        Assert.False(failureCalled);

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, success.Result);
    }

    [Fact]
    public async Task FinallyAsync_Task_Failure_InvokesOnFailure_AndReturnsSameFailure()
    {
        Error.Validation error = new Error.Validation("bad");
        Task<Operation<int>> opTask = Task.FromResult(Operation.FailureOf<int>(error));

        bool successCalled = false;
        Error? captured = null;

        Operation<int> result = await opTask.FinallyAsync(
            onSuccess: _ => successCalled = true,
            onFailure: e => captured = e
        );

        Assert.False(successCalled);
        Assert.Equal(error, captured);

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    [Fact]
    public async Task FinallyAsync_Task_CallbackThrows_PropagatesException()
    {
        Task<Operation<int>> opTask = Task.FromResult(Operation.Success(10));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            opTask.FinallyAsync(
                onSuccess: _ => throw new InvalidOperationException("boom"),
                onFailure: _ => { }
            )
        );
    }
}