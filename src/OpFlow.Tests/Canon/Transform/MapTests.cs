// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.Transform;

public class MapTests
{
    // -------------------------------------------------------------
    // Map(Func<T, U>)
    // -------------------------------------------------------------
    [Fact]
    public void Map_Success_TransformsValue()
    {
        Operation<int> op = Operation.Success(10);

        Operation<string> result = op.Map(x => $"value:{x}");

        Operation<string>.Success success = Assert.IsType<Operation<string>.Success>(result);
        Assert.Equal("value:10", success.Result);
    }

    [Fact]
    public void Map_Failure_DoesNotInvokeMapper_AndReturnsSameFailure()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        bool invoked = false;

        Operation<string> result = op.Map(x =>
        {
            invoked = true;
            return "ignored";
        });

        Assert.False(invoked);
        Operation<string>.Failure failure = Assert.IsType<Operation<string>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    [Fact]
    public void Map_MapperThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(10);

        Assert.Throws<InvalidOperationException>(() =>
            op.Map<int, int>(_ => throw new InvalidOperationException("boom"))
        );
    }

    [Fact]
    public void Map_MapperInvokedExactlyOnce()
    {
        Operation<int> op = Operation.Success(10);
        int count = 0;

        Operation<int> result = op.Map(x =>
        {
            count++;
            return x;
        });

        Assert.Equal(1, count);
        Assert.IsType<Operation<int>.Success>(result);
    }

    // -------------------------------------------------------------
    // MapAsync(Func<T, Task<U>>)
    // -------------------------------------------------------------
    [Fact]
    public async Task MapAsync_Success_TransformsValue()
    {
        Operation<int> op = Operation.Success(10);

        Operation<string> result = await op.MapAsync(async x =>
        {
            await Task.Delay(1);
            return $"value:{x}";
        });

        Operation<string>.Success success = Assert.IsType<Operation<string>.Success>(result);
        Assert.Equal("value:10", success.Result);
    }

    [Fact]
    public async Task MapAsync_Failure_DoesNotInvokeMapper()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        bool invoked = false;

        Operation<string> result = await op.MapAsync(async x =>
        {
            invoked = true;
            await Task.Delay(1);
            return "ignored";
        });

        Assert.False(invoked);
        Operation<string>.Failure failure = Assert.IsType<Operation<string>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    [Fact]
    public async Task MapAsync_MapperThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(10);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            op.MapAsync<int, int>(async _ =>
            {
                await Task.Delay(1);
                throw new InvalidOperationException("boom");
            })
        );
    }

    [Fact]
    public async Task MapAsync_MapperInvokedExactlyOnce()
    {
        Operation<int> op = Operation.Success(10);
        int count = 0;

        Operation<int> result = await op.MapAsync(async x =>
        {
            await Task.Delay(1);
            count++;
            return x;
        });

        Assert.Equal(1, count);
        Assert.IsType<Operation<int>.Success>(result);
    }
}