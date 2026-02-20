// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.Transform;

public class BindTests
{
    // -------------------------------------------------------------
    // Bind(Func<T, Operation<U>>)
    // -------------------------------------------------------------
    [Fact]
    public void Bind_Success_InvokesBinder_AndReturnsResult()
    {
        Operation<int> op = Operation.Success(10);

        Operation<string> result = op.Bind(x =>
            Operation.Success($"value:{x}")
        );

        Operation<string>.Success success = Assert.IsType<Operation<string>.Success>(result);
        Assert.Equal("value:10", success.Result);
    }

    [Fact]
    public void Bind_Failure_DoesNotInvokeBinder()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        bool invoked = false;

        Operation<string> result = op.Bind(x =>
        {
            invoked = true;
            return Operation.Success("ignored");
        });

        Assert.False(invoked);
        Operation<string>.Failure failure = Assert.IsType<Operation<string>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    [Fact]
    public void Bind_BinderThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(10);

        Assert.Throws<InvalidOperationException>(() =>
            op.Bind<int, int>(_ => throw new InvalidOperationException("boom"))
        );
    }

    [Fact]
    public void Bind_BinderInvokedExactlyOnce()
    {
        Operation<int> op = Operation.Success(10);
        int count = 0;

        Operation<int> result = op.Bind(x =>
        {
            count++;
            return Operation.Success(x);
        });

        Assert.Equal(1, count);
        Assert.IsType<Operation<int>.Success>(result);
    }

    // -------------------------------------------------------------
    // BindAsync(Func<T, Task<Operation<U>>>)
    // -------------------------------------------------------------
    [Fact]
    public async Task BindAsync_Success_InvokesBinder_AndReturnsResult()
    {
        Operation<int> op = Operation.Success(10);

        Operation<string> result = await op.BindAsync(async x =>
        {
            await Task.Delay(1);
            return Operation.Success($"value:{x}");
        });

        Operation<string>.Success success = Assert.IsType<Operation<string>.Success>(result);
        Assert.Equal("value:10", success.Result);
    }

    [Fact]
    public async Task BindAsync_Failure_DoesNotInvokeBinder()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        bool invoked = false;

        Operation<string> result = await op.BindAsync(async x =>
        {
            invoked = true;
            await Task.Delay(1);
            return Operation.Success("ignored");
        });

        Assert.False(invoked);
        Operation<string>.Failure failure = Assert.IsType<Operation<string>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    [Fact]
    public async Task BindAsync_BinderThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(10);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            op.BindAsync<int, int>(async _ =>
            {
                await Task.Delay(1);
                throw new InvalidOperationException("boom");
            })
        );
    }

    [Fact]
    public async Task BindAsync_BinderInvokedExactlyOnce()
    {
        Operation<int> op = Operation.Success(10);
        int count = 0;

        Operation<int> result = await op.BindAsync(async x =>
        {
            await Task.Delay(1);
            count++;
            return Operation.Success(x);
        });

        Assert.Equal(1, count);
        Assert.IsType<Operation<int>.Success>(result);
    }
}