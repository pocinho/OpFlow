// Copyright (c) 2026 Paulo Pocinho.

namespace PP.Toolkit.Operation.Tests.Canon.Transform;

public class BindErrorTests
{
    // -------------------------------------------------------------
    // BindError<T>(Func<Error, Operation<T>>)
    // -------------------------------------------------------------
    [Fact]
    public void BindError_Failure_InvokesBinder()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<int> result = op.BindError(err => Operation.Success(42));

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(42, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public void BindError_Success_ShortCircuits()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = op.BindError(err => Operation.Success(999));

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public void BindError_BinderReturnsFailure()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<int> result = op.BindError(err =>
            Operation.FailureOf<int>(new Error.Unexpected("handled"))
        );

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal("handled", ((Operation<int>.Failure)result).Error.Message);
    }

    // -------------------------------------------------------------
    // BindErrorAsync<T>(Func<Error, Task<Operation<T>>>)
    // -------------------------------------------------------------
    [Fact]
    public async Task BindErrorAsync_AsyncBinder_ReturnsSuccess()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("bad"));

        Operation<int> result = await op.BindErrorAsync(async err =>
        {
            await Task.Delay(10);
            return Operation.Success(123);
        });

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(123, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public async Task BindErrorAsync_AsyncBinder_Throws_PropagatesException()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("bad"));

        await Assert.ThrowsAsync<Exception>(() =>
            op.BindErrorAsync<int>(async err =>
            {
                await Task.Delay(10);
                throw new Exception("async fail");
            })
        );
    }

    [Fact]
    public async Task BindErrorAsync_Success_ShortCircuits()
    {
        Operation<int> op = Operation.Success(5);

        Operation<int> result = await op.BindErrorAsync(err =>
            Task.FromResult(Operation.Success(999))
        );

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(5, ((Operation<int>.Success)result).Result);
    }

    // -------------------------------------------------------------
    // BindErrorAsync<T>(Task<Operation<T>>)
    // -------------------------------------------------------------
    [Fact]
    public async Task BindErrorAsync_Task_ReturnsSuccess()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("bad"));

        Operation<int> result = await op.BindErrorAsync(
            Task.FromResult(Operation.Success(77))
        );

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(77, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public async Task BindErrorAsync_Task_Throws_PropagatesException()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("bad"));

        Task<Operation<int>> failingTask =
            Task.FromException<Operation<int>>(new Exception("task fail"));

        await Assert.ThrowsAsync<Exception>(() =>
            op.BindErrorAsync(failingTask)
        );
    }

    [Fact]
    public async Task BindErrorAsync_Task_Success_ShortCircuits()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = await op.BindErrorAsync(
            Task.FromResult(Operation.Success(999))
        );

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, ((Operation<int>.Success)result).Result);
    }
}