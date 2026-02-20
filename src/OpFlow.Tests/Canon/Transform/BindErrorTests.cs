// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.Transform;

public class BindErrorTests
{
    // -------------------------------------------------------------
    // BindError(Func<Error, Operation<T>>)
    // -------------------------------------------------------------
    [Fact]
    public void BindError_Failure_InvokesBinder_AndReturnsResult()
    {
        Error.Validation original = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(original);

        Operation<int> result = op.BindError(err =>
            Operation.FailureOf<int>(new Error.Unexpected($"wrapped:{err.Message}"))
        );

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal("wrapped:bad", failure.Error.Message);
        Assert.IsType<Error.Unexpected>(failure.Error);
    }

    [Fact]
    public void BindError_Success_DoesNotInvokeBinder()
    {
        Operation<int> op = Operation.Success(10);
        bool invoked = false;

        Operation<int> result = op.BindError(err =>
        {
            invoked = true;
            return Operation.FailureOf<int>(err);
        });

        Assert.False(invoked);
        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, success.Result);
    }

    [Fact]
    public void BindError_BinderThrows_PropagatesException()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        Assert.Throws<InvalidOperationException>(() =>
            op.BindError(_ => throw new InvalidOperationException("boom"))
        );
    }

    [Fact]
    public void BindError_BinderInvokedExactlyOnce()
    {
        Error.NotFound error = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(error);

        int count = 0;

        Operation<int> result = op.BindError(err =>
        {
            count++;
            return Operation.FailureOf<int>(err);
        });

        Assert.Equal(1, count);
        Assert.IsType<Operation<int>.Failure>(result);
    }

    // -------------------------------------------------------------
    // BindErrorAsync(Func<Error, Task<Operation<T>>>)
    // -------------------------------------------------------------
    [Fact]
    public async Task BindErrorAsync_Failure_InvokesBinder_AndReturnsResult()
    {
        Error.Validation original = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(original);

        Operation<int> result = await op.BindErrorAsync(async err =>
        {
            await Task.Delay(1);
            return Operation.FailureOf<int>(new Error.Unexpected($"wrapped:{err.Message}"));
        });

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal("wrapped:bad", failure.Error.Message);
        Assert.IsType<Error.Unexpected>(failure.Error);
    }

    [Fact]
    public async Task BindErrorAsync_Success_DoesNotInvokeBinder()
    {
        Operation<int> op = Operation.Success(10);
        bool invoked = false;

        Operation<int> result = await op.BindErrorAsync(async err =>
        {
            invoked = true;
            await Task.Delay(1);
            return Operation.FailureOf<int>(err);
        });

        Assert.False(invoked);
        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, success.Result);
    }

    [Fact]
    public async Task BindErrorAsync_BinderThrows_PropagatesException()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            op.BindErrorAsync(async _ =>
            {
                await Task.Delay(1);
                throw new InvalidOperationException("boom");
            })
        );
    }

    [Fact]
    public async Task BindErrorAsync_BinderInvokedExactlyOnce()
    {
        Error.NotFound error = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(error);

        int count = 0;

        Operation<int> result = await op.BindErrorAsync(async err =>
        {
            await Task.Delay(1);
            count++;
            return Operation.FailureOf<int>(err);
        });

        Assert.Equal(1, count);
        Assert.IsType<Operation<int>.Failure>(result);
    }
}