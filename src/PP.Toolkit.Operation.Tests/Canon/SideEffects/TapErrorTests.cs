// Copyright (c) 2026 Paulo Pocinho.

namespace PP.Toolkit.Operation.Tests.Canon.SideEffects;

public class TapErrorTests
{
    // -------------------------------------------------------------
    // TapError<T>(Action<Error>)
    // -------------------------------------------------------------
    [Fact]
    public void TapError_Failure_InvokesAction()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(error);

        Error? tapped = null;

        Operation<int> result = op.TapError(err => tapped = err);

        Assert.Equal(error, tapped);
        Assert.IsType<Operation<int>.Failure>(result);
    }

    [Fact]
    public void TapError_Success_DoesNotInvokeAction()
    {
        Operation<int> op = Operation.Success(10);
        bool invoked = false;

        Operation<int> result = op.TapError(err => invoked = true);

        Assert.False(invoked);
        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, ((Operation<int>.Success)result).Result);
    }

    // -------------------------------------------------------------
    // TapErrorAsync<T>(Func<Error, Task>)
    // -------------------------------------------------------------
    [Fact]
    public async Task TapErrorAsync_AsyncAction_InvokesOnFailure()
    {
        Error.Unexpected error = new Error.Unexpected("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        Error? tapped = null;

        Operation<int> result = await op.TapErrorAsync(async err =>
        {
            await Task.Delay(10);
            tapped = err;
        });

        Assert.Equal(error, tapped);
        Assert.IsType<Operation<int>.Failure>(result);
    }

    [Fact]
    public async Task TapErrorAsync_AsyncAction_Throws_PropagatesException()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("boom"));

        await Assert.ThrowsAsync<Exception>(() =>
            op.TapErrorAsync<int>(async _ =>
            {
                await Task.Delay(10);
                throw new Exception("async fail");
            })
        );
    }

    [Fact]
    public async Task TapErrorAsync_Success_DoesNotInvokeAction()
    {
        Operation<int> op = Operation.Success(5);
        bool invoked = false;

        Operation<int> result = await op.TapErrorAsync(err =>
        {
            invoked = true;
            return Task.CompletedTask;
        });

        Assert.False(invoked);
        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(5, ((Operation<int>.Success)result).Result);
    }

    // -------------------------------------------------------------
    // TapErrorAsync<T>(Func<Task>)
    // -------------------------------------------------------------
    [Fact]
    public async Task TapErrorAsync_Task_InvokesOnFailure()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("oops"));
        bool invoked = false;

        Operation<int> result = await op.TapErrorAsync(() => Task.Run(() =>
        {
            invoked = true;
        }));

        Assert.True(invoked);
        Assert.IsType<Operation<int>.Failure>(result);
    }

    [Fact]
    public async Task TapErrorAsync_Task_Throws_PropagatesException()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("oops"));

        await Assert.ThrowsAsync<Exception>(() =>
            op.TapErrorAsync(() => Task.FromException(new Exception("task fail")))
        );
    }

    [Fact]
    public async Task TapErrorAsync_Task_Success_DoesNotInvokeTask()
    {
        Operation<int> op = Operation.Success(10);
        bool invoked = false;

        Operation<int> result = await op.TapErrorAsync(() => Task.Run(() =>
        {
            invoked = true;
        }));

        Assert.False(invoked);
        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, ((Operation<int>.Success)result).Result);
    }
}