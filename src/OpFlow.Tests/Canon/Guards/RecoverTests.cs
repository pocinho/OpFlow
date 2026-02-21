// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.Guards;

public class RecoverTests
{
    // -------------------------------------------------------------
    // Recover(Func<Error, T>)
    // -------------------------------------------------------------
    [Fact]
    public void Recover_Success_PassesThroughUnchanged()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = op.Recover(error => -1);

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, success.Result);
    }

    [Fact]
    public void Recover_Failure_InvokesCallback_AndReturnsSuccess()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<int> result = op.Recover(err => 42);

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(42, success.Result);
    }

    [Fact]
    public void Recover_CallbackThrows_PropagatesException()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        Assert.Throws<InvalidOperationException>(() =>
            op.Recover(_ => throw new InvalidOperationException("boom"))
        );
    }

    // -------------------------------------------------------------
    // RecoverAsync(Operation<T>, Func<Error, T>)
    // -------------------------------------------------------------
    [Fact]
    public async Task RecoverAsync_Operation_Success_PassesThroughUnchanged()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = await op.RecoverAsync(error => -1);

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, success.Result);
    }

    [Fact]
    public async Task RecoverAsync_Operation_Failure_InvokesCallback_AndReturnsSuccess()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<int> result = await op.RecoverAsync(err => 42);

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(42, success.Result);
    }

    [Fact]
    public async Task RecoverAsync_Operation_CallbackThrows_PropagatesException()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            op.RecoverAsync(_ => throw new InvalidOperationException("boom"))
        );
    }

    // -------------------------------------------------------------
    // RecoverAsync(Task<Operation<T>>, Func<Error, T>)
    // (canonical async shape)
    // -------------------------------------------------------------
    [Fact]
    public async Task RecoverAsync_Task_Success_PassesThroughUnchanged()
    {
        Task<Operation<int>> opTask = Task.FromResult(Operation.Success(10));

        Operation<int> result = await opTask.RecoverAsync(error => -1);

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, success.Result);
    }

    [Fact]
    public async Task RecoverAsync_Task_Failure_InvokesCallback_AndReturnsSuccess()
    {
        Error.Validation error = new Error.Validation("bad");
        Task<Operation<int>> opTask = Task.FromResult(Operation.FailureOf<int>(error));

        Operation<int> result = await opTask.RecoverAsync(err => 42);

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(42, success.Result);
    }

    [Fact]
    public async Task RecoverAsync_Task_CallbackThrows_PropagatesException()
    {
        Error.Validation error = new Error.Validation("bad");
        Task<Operation<int>> opTask = Task.FromResult(Operation.FailureOf<int>(error));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            opTask.RecoverAsync(_ => throw new InvalidOperationException("boom"))
        );
    }
}