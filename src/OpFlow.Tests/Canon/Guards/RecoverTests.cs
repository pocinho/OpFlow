// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.Guards;

public class RecoverTests
{
    // -------------------------------------------------------------
    // Recover<T>(Func<Error, T>)
    // -------------------------------------------------------------
    [Fact]
    public void Recover_OnSuccess_ReturnsOriginal()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = op.Recover(err => 999);

        Assert.Same(op, result);
    }

    [Fact]
    public void Recover_OnFailure_InvokesRecoverAndReturnsSuccess()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<int> result = op.Recover(err => 42);

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(42, success.Result);
    }

    [Fact]
    public void Recover_OnFailureRecoverThrows_PropagatesException()
    {
        Error.NotFound error = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(error);

        Assert.Throws<InvalidOperationException>(() =>
            op.Recover(_ => throw new InvalidOperationException("boom"))
        );
    }

    // -------------------------------------------------------------
    // RecoverAsync<T>(Func<Error, Task<T>>)
    // -------------------------------------------------------------
    [Fact]
    public async Task RecoverAsync_Func_OnSuccess_ReturnsOriginal()
    {
        Operation<int> op = Operation.Success(5);

        Operation<int> result = await op.RecoverAsync(async err =>
        {
            await Task.Delay(1);
            return 999;
        });

        Assert.Same(op, result);
    }

    [Fact]
    public async Task RecoverAsync_Func_OnFailure_InvokesRecoverAndReturnsSuccess()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<int> result = await op.RecoverAsync(async err =>
        {
            await Task.Delay(1);
            return 123;
        });

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(123, success.Result);
    }

    [Fact]
    public async Task RecoverAsync_Func_OnFailureRecoverThrows_PropagatesException()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await op.RecoverAsync(async _ =>
            {
                await Task.Delay(1);
                throw new InvalidOperationException("boom");
            })
        );
    }

    // -------------------------------------------------------------
    // RecoverAsync<T>(Task<T>)
    // -------------------------------------------------------------
    [Fact]
    public async Task RecoverAsync_Task_OnSuccess_ReturnsOriginal()
    {
        Operation<int> op = Operation.Success(42);

        Task<int> task = Task.FromResult(999);

        Operation<int> result = await op.RecoverAsync(task);

        Assert.Same(op, result);
    }

    [Fact]
    public async Task RecoverAsync_Task_OnFailure_UsesTaskResult()
    {
        Error.NotFound error = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(error);

        Task<int> task = Task.FromResult(777);

        Operation<int> result = await op.RecoverAsync(task);

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(777, success.Result);
    }

    [Fact]
    public async Task RecoverAsync_Task_OnFailureTaskThrows_PropagatesException()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(error);

        Task<int> task = Task.FromException<int>(new InvalidOperationException("fail"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await op.RecoverAsync(task)
        );
    }
}