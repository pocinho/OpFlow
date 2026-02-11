// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.Creation;

public class FromTests
{
    // -------------------------------------------------------------
    // FromValue<T>
    // -------------------------------------------------------------
    [Fact]
    public void FromValue_ReturnsSuccess()
    {
        Operation<int> op = Operation.FromValue(10);

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(op);
        Assert.Equal(10, success.Result);
    }

    // -------------------------------------------------------------
    // FromError<T>
    // -------------------------------------------------------------
    [Fact]
    public void FromError_ReturnsFailure()
    {
        Error.NotFound error = new Error.NotFound("missing");
        Operation<int> op = Operation.FromError<int>(error);

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(op);
        Assert.Equal(error, failure.Error);
    }

    // -------------------------------------------------------------
    // From(Func<T>)
    // -------------------------------------------------------------
    [Fact]
    public void From_Func_ReturnsSuccess_WhenNoException()
    {
        Operation<int> op = Operation.From(() => 5);

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(op);
        Assert.Equal(5, success.Result);
    }

    [Fact]
    public void From_Func_ReturnsUnexpectedError_WhenExceptionThrown()
    {
        InvalidOperationException ex = new InvalidOperationException("boom");

        Operation<int> op = Operation.From<int>(() => throw ex);

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(op);
        Error.Unexpected unexpected = Assert.IsType<Error.Unexpected>(failure.Error);

        Assert.Equal("boom", unexpected.Message);
        Assert.Equal(ex, unexpected.Exception);
    }

    // -------------------------------------------------------------
    // FromAsync(Func<Task<T>>)
    // -------------------------------------------------------------
    [Fact]
    public async Task FromAsync_Func_ReturnsSuccess_WhenNoException()
    {
        Operation<int> op = await Operation.FromAsync(async () =>
        {
            await Task.Delay(1);
            return 7;
        });

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(op);
        Assert.Equal(7, success.Result);
    }

    [Fact]
    public async Task FromAsync_Func_ReturnsUnexpectedError_WhenExceptionThrown()
    {
        InvalidOperationException ex = new InvalidOperationException("bad");

        Operation<int> op = await Operation.FromAsync<int>(async () =>
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
    // FromAsync(Task<T>)
    // -------------------------------------------------------------
    [Fact]
    public async Task FromAsync_Task_ReturnsSuccess_WhenNoException()
    {
        Task<int> task = Task.FromResult(42);

        Operation<int> op = await Operation.FromAsync(task);

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(op);
        Assert.Equal(42, success.Result);
    }

    [Fact]
    public async Task FromAsync_Task_ReturnsUnexpectedError_WhenExceptionThrown()
    {
        InvalidOperationException ex = new InvalidOperationException("oops");
        Task<int> task = Task.FromException<int>(ex);

        Operation<int> op = await Operation.FromAsync(task);

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(op);
        Error.Unexpected unexpected = Assert.IsType<Error.Unexpected>(failure.Error);

        Assert.Equal("oops", unexpected.Message);
        Assert.Equal(ex, unexpected.Exception);
    }

    // -------------------------------------------------------------
    // FromException<T>
    // -------------------------------------------------------------
    [Fact]
    public void FromException_ReturnsUnexpectedError()
    {
        InvalidOperationException ex = new InvalidOperationException("fail");

        Operation<int> op = Operation.FromException<int>(ex);

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(op);
        Error.Unexpected unexpected = Assert.IsType<Error.Unexpected>(failure.Error);

        Assert.Equal("fail", unexpected.Message);
        Assert.Equal(ex, unexpected.Exception);
    }

    // -------------------------------------------------------------
    // FromNullable<T>
    // -------------------------------------------------------------
    [Fact]
    public void FromNullable_ReturnsSuccess_WhenValueNotNull()
    {
        string value = "hello";

        Operation<string> op = Operation.FromNullable(value);

        Operation<string>.Success success = Assert.IsType<Operation<string>.Success>(op);
        Assert.Equal("hello", success.Result);
    }

    [Fact]
    public void FromNullable_ReturnsUnexpectedError_WhenValueIsNull()
    {
        Operation<string> op = Operation.FromNullable<string>(null);

        Operation<string>.Failure failure = Assert.IsType<Operation<string>.Failure>(op);
        Error.Unexpected unexpected = Assert.IsType<Error.Unexpected>(failure.Error);

        Assert.Equal("Value of type String was null.", unexpected.Message);
        Assert.Null(unexpected.Exception);
    }

    [Fact]
    public void FromNullable_UsesCustomMessage_WhenProvided()
    {
        Operation<string> op = Operation.FromNullable<string>(null, "custom");

        Operation<string>.Failure failure = Assert.IsType<Operation<string>.Failure>(op);
        Error.Unexpected unexpected = Assert.IsType<Error.Unexpected>(failure.Error);

        Assert.Equal("custom", unexpected.Message);
        Assert.Null(unexpected.Exception);
    }
}