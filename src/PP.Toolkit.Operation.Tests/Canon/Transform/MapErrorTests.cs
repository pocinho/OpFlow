// Copyright (c) 2026 Paulo Pocinho.

namespace PP.Toolkit.Operation.Tests.Canon.Transform;

public class MapErrorTests
{
    // -------------------------------------------------------------
    // MapError<T>(Func<Error, Error>)
    // -------------------------------------------------------------
    [Fact]
    public void MapError_Failure_InvokesMapper()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("boom"));

        Operation<int> result = op.MapError(err =>
            new Error.Unexpected(err.Message + " mapped")
        );

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal("boom mapped", ((Operation<int>.Failure)result).Error.Message);
    }

    [Fact]
    public void MapError_Success_ShortCircuits()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = op.MapError(err =>
            new Error.Unexpected("should not run")
        );

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, ((Operation<int>.Success)result).Result);
    }

    // -------------------------------------------------------------
    // MapErrorAsync<T>(Func<Error, Task<Error>>)
    // -------------------------------------------------------------
    [Fact]
    public async Task MapErrorAsync_AsyncMapper_ReturnsMappedError()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("bad"));

        Operation<int> result = await op.MapErrorAsync(async err =>
        {
            await Task.Delay(10);
            return new Error.Unexpected(err.Message + " async");
        });

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal("bad async", ((Operation<int>.Failure)result).Error.Message);
    }

    [Fact]
    public async Task MapErrorAsync_AsyncMapper_Throws_PropagatesException()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("bad"));

        await Assert.ThrowsAsync<Exception>(() =>
            op.MapErrorAsync<int>(async err =>
            {
                await Task.Delay(10);
                throw new Exception("async fail");
            })
        );
    }

    [Fact]
    public async Task MapErrorAsync_Success_ShortCircuits()
    {
        Operation<int> op = Operation.Success(5);

        Operation<int> result = await op.MapErrorAsync(err =>
            Task.FromResult<Error>(new Error.Unexpected("ignored"))
        );

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(5, ((Operation<int>.Success)result).Result);
    }

    // -------------------------------------------------------------
    // MapErrorAsync<T>(Task<Error>)
    // -------------------------------------------------------------
    [Fact]
    public async Task MapErrorAsync_Task_ReturnsMappedError()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("oops"));

        Operation<int> result = await op.MapErrorAsync(
            Task.FromResult<Error>(new Error.Unexpected("mapped"))
        );

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal("mapped", ((Operation<int>.Failure)result).Error.Message);
    }

    [Fact]
    public async Task MapErrorAsync_Task_Throws_PropagatesException()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("oops"));

        Task<Error> failingTask =
            Task.FromException<Error>(new Exception("task fail"));

        await Assert.ThrowsAsync<Exception>(() =>
            op.MapErrorAsync(failingTask)
        );
    }

    [Fact]
    public async Task MapErrorAsync_Task_Success_ShortCircuits()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = await op.MapErrorAsync(
            Task.FromResult<Error>(new Error.Unexpected("ignored"))
        );

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, ((Operation<int>.Success)result).Result);
    }
}