// Copyright (c) 2026 Paulo Pocinho.

namespace PP.Toolkit.Operation.Tests.Canon.Creation;

public class TryTests
{
    // -------------------------------------------------------------
    // Try<T>(Func<T>)
    // -------------------------------------------------------------
    [Fact]
    public void Try_Func_ReturnsSuccess()
    {
        Operation<int> op = Operation.Try(() => 42);

        Assert.IsType<Operation<int>.Success>(op);
        Assert.Equal(42, ((Operation<int>.Success)op).Result);
    }

    [Fact]
    public void Try_Func_Throws_ReturnsFailure()
    {
        Operation<int> op = Operation.Try<int>(() => throw new InvalidOperationException("boom"));

        Assert.IsType<Operation<int>.Failure>(op);

        Error error = ((Operation<int>.Failure)op).Error;
        Assert.IsType<Error.Unexpected>(error);
        Assert.Equal("boom", error.Message);
    }

    // -------------------------------------------------------------
    // TryAsync<T>(Func<Task<T>>)
    // -------------------------------------------------------------
    [Fact]
    public async Task TryAsync_AsyncFunc_ReturnsSuccess()
    {
        Operation<string> op = await Operation.TryAsync(async () =>
        {
            await Task.Delay(1);
            return "ok";
        });

        Assert.IsType<Operation<string>.Success>(op);
        Assert.Equal("ok", ((Operation<string>.Success)op).Result);
    }

    [Fact]
    public async Task TryAsync_AsyncFunc_Throws_ReturnsFailure()
    {
        Operation<int> op = await Operation.TryAsync<int>(async () =>
        {
            await Task.Delay(1);
            throw new Exception("async fail");
        });

        Assert.IsType<Operation<int>.Failure>(op);

        Error error = ((Operation<int>.Failure)op).Error;
        Assert.IsType<Error.Unexpected>(error);
        Assert.Equal("async fail", error.Message);
    }

    // -------------------------------------------------------------
    // TryAsync<T>(Task<T>)
    // -------------------------------------------------------------
    [Fact]
    public async Task TryAsync_Task_ReturnsSuccess()
    {
        Operation<int> op = await Operation.TryAsync(Task.FromResult(123));

        Assert.IsType<Operation<int>.Success>(op);
        Assert.Equal(123, ((Operation<int>.Success)op).Result);
    }

    [Fact]
    public async Task TryAsync_Task_Throws_ReturnsFailure()
    {
        Task<int> failingTask = Task.FromException<int>(new Exception("task fail"));

        Operation<int> op = await Operation.TryAsync(failingTask);

        Assert.IsType<Operation<int>.Failure>(op);

        Error error = ((Operation<int>.Failure)op).Error;
        Assert.IsType<Error.Unexpected>(error);
        Assert.Equal("task fail", error.Message);
    }
}