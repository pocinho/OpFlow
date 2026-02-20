// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.SideEffects;

public class TapTests
{
    // -------------------------------------------------------------
    // Tap(Action<T>)
    // -------------------------------------------------------------
    [Fact]
    public void Tap_Success_InvokesCallback_AndReturnsSameSuccess()
    {
        Operation<int> op = Operation.Success(10);
        int? captured = null;

        Operation<int> result = op.Tap(x => captured = x);

        Assert.Equal(10, captured);
        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, success.Result);
    }

    [Fact]
    public void Tap_Failure_DoesNotInvokeCallback()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);
        bool invoked = false;

        Operation<int> result = op.Tap(x => invoked = true);

        Assert.False(invoked);
        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    [Fact]
    public void Tap_CallbackThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(10);

        Assert.Throws<InvalidOperationException>(() =>
            op.Tap(_ => throw new InvalidOperationException("boom"))
        );
    }

    [Fact]
    public void Tap_CallbackInvokedExactlyOnce()
    {
        Operation<int> op = Operation.Success(10);
        int count = 0;

        Operation<int> result = op.Tap(_ => count++);

        Assert.Equal(1, count);
        Assert.IsType<Operation<int>.Success>(result);
    }

    // -------------------------------------------------------------
    // TapAsync(Func<T, Task>)
    // -------------------------------------------------------------
    [Fact]
    public async Task TapAsync_Success_InvokesCallback_AndReturnsSameSuccess()
    {
        Operation<int> op = Operation.Success(10);
        int? captured = null;

        Operation<int> result = await op.TapAsync(async x =>
        {
            await Task.Delay(1);
            captured = x;
        });

        Assert.Equal(10, captured);
        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, success.Result);
    }

    [Fact]
    public async Task TapAsync_Failure_DoesNotInvokeCallback()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);
        bool invoked = false;

        Operation<int> result = await op.TapAsync(async x =>
        {
            invoked = true;
            await Task.Delay(1);
        });

        Assert.False(invoked);
        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    [Fact]
    public async Task TapAsync_CallbackThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(10);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            op.TapAsync(async _ =>
            {
                await Task.Delay(1);
                throw new InvalidOperationException("boom");
            })
        );
    }

    [Fact]
    public async Task TapAsync_CallbackInvokedExactlyOnce()
    {
        Operation<int> op = Operation.Success(10);
        int count = 0;

        Operation<int> result = await op.TapAsync(async _ =>
        {
            await Task.Delay(1);
            count++;
        });

        Assert.Equal(1, count);
        Assert.IsType<Operation<int>.Success>(result);
    }
}