// Copyright (c) 2026 Paulo Pocinho.

namespace PP.Toolkit.Operation.Tests.Canon.Guards;

public class RecoverTests
{
    // -------------------------------------------------------------
    // Recover<T>(Func<Error, T>)
    // -------------------------------------------------------------
    [Fact]
    public void Recover_Failure_InvokesRecover()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("boom"));

        Operation<int> result = op.Recover(err => 42);

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(42, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public void Recover_Success_ShortCircuits()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = op.Recover(err => 999);

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, ((Operation<int>.Success)result).Result);
    }

    // -------------------------------------------------------------
    // RecoverAsync<T>(Func<Error, Task<T>>)
    // -------------------------------------------------------------
    [Fact]
    public async Task RecoverAsync_AsyncRecover_ReturnsSuccess()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("bad"));

        Operation<int> result = await op.RecoverAsync(async err =>
        {
            await Task.Delay(1);
            return 123;
        });

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(123, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public async Task RecoverAsync_AsyncRecover_Throws_PropagatesException()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("bad"));

        await Assert.ThrowsAsync<Exception>(() =>
            op.RecoverAsync<int>(async err =>
            {
                await Task.Delay(1);
                throw new Exception("async fail");
            })
        );
    }

    [Fact]
    public async Task RecoverAsync_Success_ShortCircuits()
    {
        Operation<int> op = Operation.Success(5);

        Operation<int> result = await op.RecoverAsync(err =>
            Task.FromResult(999)
        );

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(5, ((Operation<int>.Success)result).Result);
    }

    // -------------------------------------------------------------
    // RecoverAsync<T>(Task<T>)
    // -------------------------------------------------------------
    [Fact]
    public async Task RecoverAsync_Task_ReturnsSuccess()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("oops"));

        Operation<int> result = await op.RecoverAsync(
            Task.FromResult(77)
        );

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(77, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public async Task RecoverAsync_Task_Throws_PropagatesException()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("oops"));

        Task<int> failingTask =
            Task.FromException<int>(new Exception("task fail"));

        await Assert.ThrowsAsync<Exception>(() =>
            op.RecoverAsync(failingTask)
        );
    }

    [Fact]
    public async Task RecoverAsync_Task_Success_ShortCircuits()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = await op.RecoverAsync(
            Task.FromResult(999)
        );

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, ((Operation<int>.Success)result).Result);
    }
}