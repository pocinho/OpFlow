// Copyright (c) 2026 Paulo Pocinho.

namespace PP.Toolkit.Operation.Tests.Canon.SideEffects;

public class TapTests
{
    // -------------------------------------------------------------
    // Tap<T>(Action<T>)
    // -------------------------------------------------------------
    [Fact]
    public void Tap_Success_InvokesAction()
    {
        Operation<int> op = Operation.Success(10);
        int tapped = 0;

        Operation<int> result = op.Tap(x => tapped = x);

        Assert.Equal(10, tapped);
        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public void Tap_Failure_DoesNotInvokeAction()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("boom"));
        bool invoked = false;

        Operation<int> result = op.Tap(x => invoked = true);

        Assert.False(invoked);
        Assert.IsType<Operation<int>.Failure>(result);
    }

    // -------------------------------------------------------------
    // TapAsync<T>(Func<T, Task>)
    // -------------------------------------------------------------
    [Fact]
    public async Task TapAsync_AsyncAction_InvokesOnSuccess()
    {
        Operation<int> op = Operation.Success(5);
        int tapped = 0;

        Operation<int> result = await op.TapAsync(async x =>
        {
            await Task.Delay(1);
            tapped = x;
        });

        Assert.Equal(5, tapped);
        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(5, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public async Task TapAsync_AsyncAction_Throws_PropagatesException()
    {
        Operation<int> op = Operation.Success(1);

        await Assert.ThrowsAsync<Exception>(() =>
            op.TapAsync<int>(async _ =>
            {
                await Task.Delay(1);
                throw new Exception("async fail");
            })
        );
    }

    [Fact]
    public async Task TapAsync_Failure_DoesNotInvokeAction()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("boom"));
        bool invoked = false;

        Operation<int> result = await op.TapAsync(x =>
        {
            invoked = true;
            return Task.CompletedTask;
        });

        Assert.False(invoked);
        Assert.IsType<Operation<int>.Failure>(result);
    }

    // -------------------------------------------------------------
    // TapAsync<T>(Func<Task>)
    // -------------------------------------------------------------
    [Fact]
    public async Task TapAsync_Task_InvokesOnSuccess()
    {
        Operation<int> op = Operation.Success(10);
        bool invoked = false;

        Operation<int> result = await op.TapAsync(() => Task.Run(() =>
        {
            invoked = true;
        }));

        Assert.True(invoked);
        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public async Task TapAsync_Task_Throws_PropagatesException()
    {
        Operation<int> op = Operation.Success(10);

        await Assert.ThrowsAsync<Exception>(() =>
            op.TapAsync(() => Task.FromException(new Exception("task fail")))
        );
    }

    [Fact]
    public async Task TapAsync_Task_Failure_DoesNotInvokeTask()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("boom"));
        bool invoked = false;

        Operation<int> result = await op.TapAsync(() => Task.Run(() =>
        {
            invoked = true;
        }));

        Assert.False(invoked);
        Assert.IsType<Operation<int>.Failure>(result);
    }
}