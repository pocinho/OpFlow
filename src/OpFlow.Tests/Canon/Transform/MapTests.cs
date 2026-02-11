// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.Transform;

public class MapTests
{
    // -------------------------------------------------------------
    // Map<T, U>(Func<T, U>)
    // -------------------------------------------------------------
    [Fact]
    public void Map_OnSuccess_MapsValue()
    {
        Operation<int> op = Operation.Success(10);

        Operation<string> result = op.Map(x => $"Value: {x}");

        Operation<string>.Success success = Assert.IsType<Operation<string>.Success>(result);
        Assert.Equal("Value: 10", success.Result);
    }

    [Fact]
    public void Map_OnFailure_PreservesError()
    {
        Error.NotFound error = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<string> result = op.Map(x => $"Value: {x}");

        Operation<string>.Failure failure = Assert.IsType<Operation<string>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    // -------------------------------------------------------------
    // MapAsync<T, U>(Func<T, Task<U>>)
    // -------------------------------------------------------------
    [Fact]
    public async Task MapAsync_Func_OnSuccess_MapsValue()
    {
        Operation<int> op = Operation.Success(5);

        Operation<string> result = await op.MapAsync(async x =>
        {
            await Task.Delay(1);
            return $"Mapped: {x}";
        });

        Operation<string>.Success success = Assert.IsType<Operation<string>.Success>(result);
        Assert.Equal("Mapped: 5", success.Result);
    }

    [Fact]
    public async Task MapAsync_Func_OnFailure_PreservesError()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<string> result = await op.MapAsync(async x =>
        {
            await Task.Delay(1);
            return $"Mapped: {x}";
        });

        Operation<string>.Failure failure = Assert.IsType<Operation<string>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    // -------------------------------------------------------------
    // MapAsync<T, U>(Task<U>)
    // -------------------------------------------------------------
    [Fact]
    public async Task MapAsync_Task_OnSuccess_MapsValue()
    {
        Operation<int> op = Operation.Success(42);
        Task<string> task = Task.FromResult("hello");

        Operation<string> result = await op.MapAsync(task);

        Operation<string>.Success success = Assert.IsType<Operation<string>.Success>(result);
        Assert.Equal("hello", success.Result);
    }

    [Fact]
    public async Task MapAsync_Task_OnFailure_PreservesError()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(error);

        Task<string> task = Task.FromResult("ignored");

        Operation<string> result = await op.MapAsync(task);

        Operation<string>.Failure failure = Assert.IsType<Operation<string>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }
}