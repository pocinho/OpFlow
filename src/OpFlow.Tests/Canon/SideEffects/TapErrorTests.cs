// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.ControlFlow;

public class TapErrorTests
{
    // -------------------------------------------------------------
    // TapError<T>(Action<Error>)
    // -------------------------------------------------------------
    [Fact]
    public void TapError_OnFailure_InvokesAction()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        Error? captured = null;

        Operation<int> result = op.TapError(err => captured = err);

        Assert.Same(op, result);
        Assert.Equal(error, captured);
    }

    [Fact]
    public void TapError_OnSuccess_DoesNotInvokeAction()
    {
        Operation<int> op = Operation.Success(10);

        bool invoked = false;

        Operation<int> result = op.TapError(err => invoked = true);

        Assert.Same(op, result);
        Assert.False(invoked);
    }

    [Fact]
    public void TapError_OnFailureThrows_PropagatesException()
    {
        Error.NotFound error = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(error);

        Assert.Throws<InvalidOperationException>(() =>
            op.TapError(_ => throw new InvalidOperationException("boom"))
        );
    }

    // -------------------------------------------------------------
    // TapErrorAsync<T>(Func<Error, Task>)
    // -------------------------------------------------------------
    [Fact]
    public async Task TapErrorAsync_Func_OnFailure_InvokesAction()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(error);

        Error? captured = null;

        Operation<int> result = await op.TapErrorAsync(async err =>
        {
            await Task.Delay(1);
            captured = err;
        });

        Assert.Same(op, result);
        Assert.Equal(error, captured);
    }

    [Fact]
    public async Task TapErrorAsync_Func_OnSuccess_DoesNotInvokeAction()
    {
        Operation<int> op = Operation.Success(5);

        bool invoked = false;

        Operation<int> result = await op.TapErrorAsync(async err =>
        {
            await Task.Delay(1);
            invoked = true;
        });

        Assert.Same(op, result);
        Assert.False(invoked);
    }

    [Fact]
    public async Task TapErrorAsync_Func_OnFailureThrows_PropagatesException()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await op.TapErrorAsync(async _ =>
            {
                await Task.Delay(1);
                throw new InvalidOperationException("boom");
            })
        );
    }

    // -------------------------------------------------------------
    // TapErrorAsync<T>(Func<Task>)
    // -------------------------------------------------------------
    [Fact]
    public async Task TapErrorAsync_Task_OnFailure_InvokesAction()
    {
        Error.NotFound error = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(error);

        bool invoked = false;

        Operation<int> result = await op.TapErrorAsync(async () =>
        {
            await Task.Delay(1);
            invoked = true;
        });

        Assert.Same(op, result);
        Assert.True(invoked);
    }

    [Fact]
    public async Task TapErrorAsync_Task_OnSuccess_DoesNotInvokeAction()
    {
        Operation<int> op = Operation.Success(42);

        bool invoked = false;

        Operation<int> result = await op.TapErrorAsync(async () =>
        {
            await Task.Delay(1);
            invoked = true;
        });

        Assert.Same(op, result);
        Assert.False(invoked);
    }

    [Fact]
    public async Task TapErrorAsync_Task_OnFailureThrows_PropagatesException()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(error);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await op.TapErrorAsync(async () =>
            {
                await Task.Delay(1);
                throw new InvalidOperationException("boom");
            })
        );
    }
}