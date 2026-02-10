// Copyright (c) 2026 Paulo Pocinho.

namespace PP.Toolkit.Operation.Tests.Canon.Transform;

public class MapTests
{
    // -------------------------------------------------------------
    // Map<T, U>(Func<T, U>)
    // -------------------------------------------------------------
    [Fact]
    public void Map_Success_TransformsValue()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = op.Map(x => x * 2);

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(20, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public void Map_Failure_ShortCircuits()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<int> result = op.Map(x => x * 2);

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, ((Operation<int>.Failure)result).Error);
    }

    // -------------------------------------------------------------
    // MapAsync<T, U>(Func<T, Task<U>>)
    // -------------------------------------------------------------
    [Fact]
    public async Task MapAsync_AsyncMapper_ReturnsSuccess()
    {
        Operation<string> op = Operation.Success("hello");

        Operation<int> result = await op.MapAsync(async s =>
        {
            await Task.Delay(10);
            return s.Length;
        });

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(5, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public async Task MapAsync_AsyncMapper_Throws_PropagatesException()
    {
        Operation<int> op = Operation.Success(1);

        await Assert.ThrowsAsync<Exception>(() =>
            op.MapAsync<int, int>(async _ =>
            {
                await Task.Delay(10);
                throw new Exception("async fail");
            })
        );
    }

    [Fact]
    public async Task MapAsync_Failure_ShortCircuits()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<int> result = await op.MapAsync(x =>
            Task.FromResult(x * 2)
        );

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, ((Operation<int>.Failure)result).Error);
    }

    // -------------------------------------------------------------
    // MapAsync<T, U>(Task<U>)
    // -------------------------------------------------------------
    [Fact]
    public async Task MapAsync_Task_ReturnsSuccess()
    {
        Operation<int> op = Operation.Success(3);

        Operation<int> result = await op.MapAsync(
            Task.FromResult(9)
        );

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(9, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public async Task MapAsync_Task_Throws_PropagatesException()
    {
        Operation<int> op = Operation.Success(3);

        Task<int> failingTask =
            Task.FromException<int>(new Exception("task fail"));

        await Assert.ThrowsAsync<Exception>(() =>
            op.MapAsync(failingTask)
        );
    }

    [Fact]
    public async Task MapAsync_Task_Failure_ShortCircuits()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<int> result = await op.MapAsync(
            Task.FromResult(99)
        );

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, ((Operation<int>.Failure)result).Error);
    }
}