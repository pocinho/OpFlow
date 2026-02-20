// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.SideEffects;

public class TapErrorTests
{
    // -------------------------------------------------------------
    // TapError(Action<Error>)
    // -------------------------------------------------------------
    [Fact]
    public void TapError_Failure_InvokesCallback_AndReturnsSameFailure()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        Error? captured = null;

        Operation<int> result = op.TapError(err => captured = err);

        Assert.Equal(error, captured);
        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    [Fact]
    public void TapError_Success_DoesNotInvokeCallback()
    {
        Operation<int> op = Operation.Success(10);
        bool invoked = false;

        Operation<int> result = op.TapError(err => invoked = true);

        Assert.False(invoked);
        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, success.Result);
    }

    [Fact]
    public void TapError_CallbackThrows_PropagatesException()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(error);

        Assert.Throws<InvalidOperationException>(() =>
            op.TapError(_ => throw new InvalidOperationException("fail"))
        );
    }

    [Fact]
    public void TapError_CallbackInvokedExactlyOnce()
    {
        Error.NotFound error = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(error);

        int count = 0;

        Operation<int> result = op.TapError(_ => count++);

        Assert.Equal(1, count);
        Assert.IsType<Operation<int>.Failure>(result);
    }

    // -------------------------------------------------------------
    // TapErrorAsync(Func<Error, Task>)
    // -------------------------------------------------------------
    [Fact]
    public async Task TapErrorAsync_Failure_InvokesCallback_AndReturnsSameFailure()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        Error? captured = null;

        Operation<int> result = await op.TapErrorAsync(async err =>
        {
            await Task.Delay(1);
            captured = err;
        });

        Assert.Equal(error, captured);
        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    [Fact]
    public async Task TapErrorAsync_Success_DoesNotInvokeCallback()
    {
        Operation<int> op = Operation.Success(10);
        bool invoked = false;

        Operation<int> result = await op.TapErrorAsync(async err =>
        {
            invoked = true;
            await Task.Delay(1);
        });

        Assert.False(invoked);
        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, success.Result);
    }

    [Fact]
    public async Task TapErrorAsync_CallbackThrows_PropagatesException()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(error);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            op.TapErrorAsync(async _ =>
            {
                await Task.Delay(1);
                throw new InvalidOperationException("fail");
            })
        );
    }

    [Fact]
    public async Task TapErrorAsync_CallbackInvokedExactlyOnce()
    {
        Error.NotFound error = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(error);

        int count = 0;

        Operation<int> result = await op.TapErrorAsync(async _ =>
        {
            await Task.Delay(1);
            count++;
        });

        Assert.Equal(1, count);
        Assert.IsType<Operation<int>.Failure>(result);
    }
}