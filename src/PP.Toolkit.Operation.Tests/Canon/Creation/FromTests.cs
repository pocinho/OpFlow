// Copyright (c) 2026 Paulo Pocinho.

namespace PP.Toolkit.Operation.Tests.Canon.Creation;

public class FromTests
{
    // -------------------------------------------------------------
    // Success<T>
    // -------------------------------------------------------------
    [Fact]
    public void Success_ReturnsSuccessCase()
    {
        Operation<int> op = Operation.Success(42);

        Assert.IsType<Operation<int>.Success>(op);
        Assert.Equal(42, ((Operation<int>.Success)op).Result);
    }

    // -------------------------------------------------------------
    // FailureOf<T>
    // -------------------------------------------------------------
    [Fact]
    public void FailureOf_ReturnsFailureCase()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(error);

        Assert.IsType<Operation<int>.Failure>(op);
        Assert.Equal("boom", ((Operation<int>.Failure)op).Error.Message);
    }

    // -------------------------------------------------------------
    // FromValue<T>
    // -------------------------------------------------------------
    [Fact]
    public void FromValue_ReturnsSuccess()
    {
        Operation<int> op = Operation.FromValue(123);

        Assert.IsType<Operation<int>.Success>(op);
        Assert.Equal(123, ((Operation<int>.Success)op).Result);
    }

    // -------------------------------------------------------------
    // FromError<T>
    // -------------------------------------------------------------
    [Fact]
    public void FromError_ReturnsFailure()
    {
        Error.Unexpected error = new Error.Unexpected("bad");
        Operation<int> op = Operation.FromError<int>(error);

        Assert.IsType<Operation<int>.Failure>(op);
        Assert.Equal("bad", ((Operation<int>.Failure)op).Error.Message);
    }

    // -------------------------------------------------------------
    // From(Func<T>)
    // -------------------------------------------------------------
    [Fact]
    public void From_Func_ReturnsSuccess()
    {
        Operation<string> op = Operation.From(() => "hello");

        Assert.IsType<Operation<string>.Success>(op);
        Assert.Equal("hello", ((Operation<string>.Success)op).Result);
    }

    [Fact]
    public void From_Func_Throws_ReturnsFailure()
    {
        Operation<int> op = Operation.From<int>(() => throw new InvalidOperationException("oops"));

        Assert.IsType<Operation<int>.Failure>(op);
        Assert.Equal("oops", ((Operation<int>.Failure)op).Error.Message);
    }

    // -------------------------------------------------------------
    // FromAsync(Func<Task<T>>)
    // -------------------------------------------------------------
    [Fact]
    public async Task FromAsync_AsyncFunc_ReturnsSuccess()
    {
        Operation<int> op = await Operation.FromAsync(async () =>
        {
            await Task.Delay(1);
            return 99;
        });

        Assert.IsType<Operation<int>.Success>(op);
        Assert.Equal(99, ((Operation<int>.Success)op).Result);
    }

    [Fact]
    public async Task FromAsync_AsyncFunc_Throws_ReturnsFailure()
    {
        Operation<int> op = await Operation.FromAsync<int>(async () =>
        {
            await Task.Delay(1);
            throw new Exception("async fail");
        });

        Assert.IsType<Operation<int>.Failure>(op);
        Assert.Equal("async fail", ((Operation<int>.Failure)op).Error.Message);
    }

    // -------------------------------------------------------------
    // FromAsync(Task<T>)
    // -------------------------------------------------------------
    [Fact]
    public async Task FromAsync_Task_ReturnsSuccess()
    {
        Operation<string> op = await Operation.FromAsync(Task.FromResult("task-ok"));

        Assert.IsType<Operation<string>.Success>(op);
        Assert.Equal("task-ok", ((Operation<string>.Success)op).Result);
    }

    [Fact]
    public async Task FromAsync_Task_Throws_ReturnsFailure()
    {
        Task<int> failingTask = Task.FromException<int>(new Exception("task fail"));

        Operation<int> op = await Operation.FromAsync(failingTask);

        Assert.IsType<Operation<int>.Failure>(op);
        Assert.Equal("task fail", ((Operation<int>.Failure)op).Error.Message);
    }

    // -------------------------------------------------------------
    // FromException<T>
    // -------------------------------------------------------------
    [Fact]
    public void FromException_ReturnsFailure()
    {
        Exception ex = new Exception("boom");
        Operation<int> op = Operation.FromException<int>(ex);

        Assert.IsType<Operation<int>.Failure>(op);
        Assert.Equal("boom", ((Operation<int>.Failure)op).Error.Message);
    }

    // -------------------------------------------------------------
    // FromNullable<T>
    // -------------------------------------------------------------
    [Fact]
    public void FromNullable_ValueNotNull_ReturnsSuccess()
    {
        string? value = "ok";

        Operation<string> op = Operation.FromNullable(value);

        Assert.IsType<Operation<string>.Success>(op);
        Assert.Equal("ok", ((Operation<string>.Success)op).Result);
    }

    [Fact]
    public void FromNullable_ValueNull_ReturnsFailure()
    {
        string? value = null;

        Operation<string> op = Operation.FromNullable(value);

        Assert.IsType<Operation<string>.Failure>(op);
    }

    [Fact]
    public void FromNullable_CustomMessage_ReturnsFailureWithMessage()
    {
        string? value = null;

        Operation<string> op = Operation.FromNullable(value, "custom");

        Assert.IsType<Operation<string>.Failure>(op);
        Assert.Equal("custom", ((Operation<string>.Failure)op).Error.Message);
    }
}