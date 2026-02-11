// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.Creation;

public class TryTests
{
    // -------------------------------------------------------------
    // Try<T>(Func<T>)
    // -------------------------------------------------------------
    [Fact]
    public void Try_Func_ReturnsSuccess_WhenNoException()
    {
        Operation<int> op = Operation.Try(() => 10);

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(op);
        Assert.Equal(10, success.Result);
    }

    [Fact]
    public void Try_Func_ReturnsUnexpectedError_WhenExceptionThrown()
    {
        InvalidOperationException ex = new InvalidOperationException("boom");

        Operation<int> op = Operation.Try<int>(() => throw ex);

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(op);
        Error.Unexpected unexpected = Assert.IsType<Error.Unexpected>(failure.Error);

        Assert.Equal("boom", unexpected.Message);
        Assert.Equal(ex, unexpected.Exception);
    }

    // -------------------------------------------------------------
    // TryAsync<T>(Func<Task<T>>)
    // -------------------------------------------------------------
    [Fact]
    public async Task TryAsync_Func_ReturnsSuccess_WhenNoException()
    {
        Operation<int> op = await Operation.TryAsync(async () =>
        {
            await Task.Delay(1);
            return 7;
        });

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(op);
        Assert.Equal(7, success.Result);
    }

    [Fact]
    public async Task TryAsync_Func_ReturnsUnexpectedError_WhenExceptionThrown()
    {
        InvalidOperationException ex = new InvalidOperationException("bad");

        Operation<int> op = await Operation.TryAsync<int>(async () =>
        {
            await Task.Delay(1);
            throw ex;
        });

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(op);
        Error.Unexpected unexpected = Assert.IsType<Error.Unexpected>(failure.Error);

        Assert.Equal("bad", unexpected.Message);
        Assert.Equal(ex, unexpected.Exception);
    }

    // -------------------------------------------------------------
    // TryAsync<T>(Task<T>)
    // -------------------------------------------------------------
    [Fact]
    public async Task TryAsync_Task_ReturnsSuccess_WhenNoException()
    {
        Task<int> task = Task.FromResult(42);

        Operation<int> op = await Operation.TryAsync(task);

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(op);
        Assert.Equal(42, success.Result);
    }

    [Fact]
    public async Task TryAsync_Task_ReturnsUnexpectedError_WhenExceptionThrown()
    {
        InvalidOperationException ex = new InvalidOperationException("oops");
        Task<int> task = Task.FromException<int>(ex);

        Operation<int> op = await Operation.TryAsync(task);

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(op);
        Error.Unexpected unexpected = Assert.IsType<Error.Unexpected>(failure.Error);

        Assert.Equal("oops", unexpected.Message);
        Assert.Equal(ex, unexpected.Exception);
    }
}