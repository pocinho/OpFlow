// Copyright (c) 2026 Paulo Pocinho.

namespace PP.Toolkit.Operation.Tests;

public class BindTests
{
    // -------------------------------------------------------------
    // Bind<T, U>(Func<T, Operation<U>>)
    // -------------------------------------------------------------
    [Fact]
    public void Bind_Success_InvokesBinder()
    {
        var op = Operation.Success(10);

        var result = op.Bind(x => Operation.Success(x * 2));

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(20, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public void Bind_Failure_ShortCircuits()
    {
        var error = new Error.Unexpected("boom");
        var op = Operation.FailureOf<int>(error);

        var result = op.Bind(x => Operation.Success(x * 2));

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, ((Operation<int>.Failure)result).Error);
    }

    [Fact]
    public void Bind_BinderReturnsFailure()
    {
        var op = Operation.Success(5);

        var result = op.Bind<int, int>(x => Operation.FailureOf<int>(new Error.Unexpected("fail")));

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal("fail", ((Operation<int>.Failure)result).Error.Message);
    }

    // -------------------------------------------------------------
    // BindAsync<T, U>(Func<T, Task<Operation<U>>>)
    // -------------------------------------------------------------
    [Fact]
    public async Task BindAsync_AsyncBinder_ReturnsSuccess()
    {
        var op = Operation.Success("hello");

        var result = await op.BindAsync(async s =>
        {
            await Task.Delay(10);
            return Operation.Success(s.Length);
        });

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(5, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public async Task BindAsync_AsyncBinder_Throws_ReturnsFailure()
    {
        var op = Operation.Success(1);

        var result = await op.BindAsync<int, int>(async _ =>
        {
            await Task.Delay(10);
            throw new Exception("async fail");
        });

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal("async fail", ((Operation<int>.Failure)result).Error.Message);
    }

    [Fact]
    public async Task BindAsync_Failure_ShortCircuits()
    {
        var error = new Error.Unexpected("boom");
        var op = Operation.FailureOf<int>(error);

        var result = await op.BindAsync(x => Task.FromResult(Operation.Success(x * 2)));

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, ((Operation<int>.Failure)result).Error);
    }

    // -------------------------------------------------------------
    // BindAsync<T, U>(Task<Operation<U>>)
    // -------------------------------------------------------------
    [Fact]
    public async Task BindAsync_Task_ReturnsSuccess()
    {
        var op = Operation.Success(3);

        var result = await op.BindAsync(Task.FromResult(Operation.Success(9)));

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(9, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public async Task BindAsync_Task_Failure_ReturnsFailure()
    {
        var op = Operation.Success(3);

        var failingTask = Task.FromResult<Operation<int>>(
            Operation.FailureOf<int>(new Error.Unexpected("task fail"))
        );

        var result = await op.BindAsync(failingTask);

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal("task fail", ((Operation<int>.Failure)result).Error.Message);
    }

    [Fact]
    public async Task BindAsync_Task_Failure_ShortCircuits()
    {
        var error = new Error.Unexpected("boom");
        var op = Operation.FailureOf<int>(error);

        var result = await op.BindAsync(Task.FromResult(Operation.Success(99)));

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, ((Operation<int>.Failure)result).Error);
    }

    [Fact]
    public async Task BindAsync_Task_Throws_PropagatesException()
    {
        var op = Operation.Success(3);

        var failingTask = Task.FromException<Operation<int>>(new Exception("boom"));

        await Assert.ThrowsAsync<Exception>(() => op.BindAsync(failingTask));
    }
}