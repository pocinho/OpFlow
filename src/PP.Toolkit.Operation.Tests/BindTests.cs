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
        Operation<int> op = Operation.Success(10);

        Operation<int> result = op.Bind(x => Operation.Success(x * 2));

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(20, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public void Bind_Failure_ShortCircuits()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<int> result = op.Bind(x => Operation.Success(x * 2));

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, ((Operation<int>.Failure)result).Error);
    }

    [Fact]
    public void Bind_BinderReturnsFailure()
    {
        Operation<int> op = Operation.Success(5);

        Operation<int> result = op.Bind<int, int>(x =>
            Operation.FailureOf<int>(new Error.Unexpected("fail"))
        );

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal("fail", ((Operation<int>.Failure)result).Error.Message);
    }

    // -------------------------------------------------------------
    // BindAsync<T, U>(Func<T, Task<Operation<U>>>)
    // -------------------------------------------------------------
    [Fact]
    public async Task BindAsync_AsyncBinder_ReturnsSuccess()
    {
        Operation<string> op = Operation.Success("hello");

        Operation<int> result = await op.BindAsync(async s =>
        {
            await Task.Delay(10);
            return Operation.Success(s.Length);
        });

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(5, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public async Task BindAsync_AsyncBinder_Throws_PropagatesException()
    {
        Operation<int> op = Operation.Success(1);

        await Assert.ThrowsAsync<Exception>(() =>
            op.BindAsync<int, int>(async _ =>
            {
                await Task.Delay(10);
                throw new Exception("async fail");
            })
        );
    }

    [Fact]
    public async Task BindAsync_Failure_ShortCircuits()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<int> result = await op.BindAsync(x =>
            Task.FromResult(Operation.Success(x * 2))
        );

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, ((Operation<int>.Failure)result).Error);
    }

    // -------------------------------------------------------------
    // BindAsync<T, U>(Task<Operation<U>>)
    // -------------------------------------------------------------
    [Fact]
    public async Task BindAsync_Task_ReturnsSuccess()
    {
        Operation<int> op = Operation.Success(3);

        Operation<int> result = await op.BindAsync(
            Task.FromResult(Operation.Success(9))
        );

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(9, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public async Task BindAsync_Task_Throws_PropagatesException()
    {
        Operation<int> op = Operation.Success(3);

        Task<Operation<int>> failingTask =
            Task.FromException<Operation<int>>(new Exception("task fail"));

        await Assert.ThrowsAsync<Exception>(() =>
            op.BindAsync(failingTask)
        );
    }

    [Fact]
    public async Task BindAsync_Task_Failure_ReturnsFailure()
    {
        Operation<int> op = Operation.Success(3);

        Task<Operation<int>> failingTask = Task.FromResult(
            Operation.FailureOf<int>(new Error.Unexpected("task fail"))
        );

        Operation<int> result = await op.BindAsync(failingTask);

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal("task fail", ((Operation<int>.Failure)result).Error.Message);
    }

    [Fact]
    public async Task BindAsync_Task_Failure_ShortCircuits()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<int> result = await op.BindAsync(
            Task.FromResult(Operation.Success(99))
        );

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, ((Operation<int>.Failure)result).Error);
    }
}