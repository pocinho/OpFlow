// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.SideEffects;

public class TapTests
{
    // -------------------------------------------------------------
    // Tap<T>(Action<T>)
    // -------------------------------------------------------------
    [Fact]
    public void Tap_OnSuccess_InvokesAction()
    {
        Operation<int> op = Operation.Success(10);

        int captured = 0;

        Operation<int> result = op.Tap(x => captured = x * 2);

        Assert.Same(op, result);
        Assert.Equal(20, captured);
    }

    [Fact]
    public void Tap_OnFailure_DoesNotInvokeAction()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        bool invoked = false;

        Operation<int> result = op.Tap(x => invoked = true);

        Assert.Same(op, result);
        Assert.False(invoked);
    }

    [Fact]
    public void Tap_OnSuccessThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(5);

        Assert.Throws<InvalidOperationException>(() =>
            op.Tap(_ => throw new InvalidOperationException("boom"))
        );
    }

    // -------------------------------------------------------------
    // TapAsync<T>(Func<T, Task>)
    // -------------------------------------------------------------
    [Fact]
    public async Task TapAsync_Func_OnSuccess_InvokesAction()
    {
        Operation<int> op = Operation.Success(7);

        int captured = 0;

        Operation<int> result = await op.TapAsync(async x =>
        {
            await Task.Delay(1);
            captured = x + 1;
        });

        Assert.Same(op, result);
        Assert.Equal(8, captured);
    }

    [Fact]
    public async Task TapAsync_Func_OnFailure_DoesNotInvokeAction()
    {
        Error.NotFound error = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(error);

        bool invoked = false;

        Operation<int> result = await op.TapAsync(async x =>
        {
            await Task.Delay(1);
            invoked = true;
        });

        Assert.Same(op, result);
        Assert.False(invoked);
    }

    [Fact]
    public async Task TapAsync_Func_OnSuccessThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(3);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await op.TapAsync(async _ =>
            {
                await Task.Delay(1);
                throw new InvalidOperationException("boom");
            })
        );
    }

    // -------------------------------------------------------------
    // TapAsync<T>(Func<Task>)
    // -------------------------------------------------------------
    [Fact]
    public async Task TapAsync_Task_OnSuccess_InvokesAction()
    {
        Operation<int> op = Operation.Success(42);

        bool invoked = false;

        Operation<int> result = await op.TapAsync(async () =>
        {
            await Task.Delay(1);
            invoked = true;
        });

        Assert.Same(op, result);
        Assert.True(invoked);
    }

    [Fact]
    public async Task TapAsync_Task_OnFailure_DoesNotInvokeAction()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(error);

        bool invoked = false;

        Operation<int> result = await op.TapAsync(async () =>
        {
            await Task.Delay(1);
            invoked = true;
        });

        Assert.Same(op, result);
        Assert.False(invoked);
    }

    [Fact]
    public async Task TapAsync_Task_OnSuccessThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(1);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await op.TapAsync(async () =>
            {
                await Task.Delay(1);
                throw new InvalidOperationException("boom");
            })
        );
    }
}