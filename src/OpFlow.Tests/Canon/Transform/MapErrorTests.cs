// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.Transform;

public class MapErrorTests
{
    // -------------------------------------------------------------
    // MapError<T>(Func<Error, Error>)
    // -------------------------------------------------------------
    [Fact]
    public void MapError_OnFailure_MapsError()
    {
        Error.NotFound original = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(original);

        Error.Validation mapped = new Error.Validation("bad");

        Operation<int> result = op.MapError(_ => mapped);

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(mapped, failure.Error);
    }

    [Fact]
    public void MapError_OnSuccess_PreservesValue()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = op.MapError(err => new Error.Unexpected("ignored"));

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, success.Result);
    }

    // -------------------------------------------------------------
    // MapErrorAsync<T>(Func<Error, Task<Error>>)
    // -------------------------------------------------------------
    [Fact]
    public async Task MapErrorAsync_Func_OnFailure_MapsError()
    {
        Error.Validation original = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(original);

        Error.Unauthorized mapped = new Error.Unauthorized("nope");

        Operation<int> result = await op.MapErrorAsync(async err =>
        {
            await Task.Delay(1);
            return mapped;
        });

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(mapped, failure.Error);
    }

    [Fact]
    public async Task MapErrorAsync_Func_OnSuccess_PreservesValue()
    {
        Operation<int> op = Operation.Success(5);

        Operation<int> result = await op.MapErrorAsync(async err =>
        {
            await Task.Delay(1);
            return new Error.NotFound("ignored");
        });

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(5, success.Result);
    }

    // -------------------------------------------------------------
    // MapErrorAsync<T>(Task<Error>)
    // -------------------------------------------------------------
    [Fact]
    public async Task MapErrorAsync_Task_OnFailure_MapsError()
    {
        Error.Unexpected original = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(original);

        Error.Validation mapped = new Error.Validation("bad");
        Task<Error> task = Task.FromResult<Error>(mapped);

        Operation<int> result = await op.MapErrorAsync(task);

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(mapped, failure.Error);
    }

    [Fact]
    public async Task MapErrorAsync_Task_OnSuccess_PreservesValue()
    {
        Operation<int> op = Operation.Success(42);

        Task<Error> task = Task.FromResult<Error>(new Error.NotFound("ignored"));

        Operation<int> result = await op.MapErrorAsync(task);

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(42, success.Result);
    }
}