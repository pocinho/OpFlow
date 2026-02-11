// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.Transform;

public class BindTests
{
    // -------------------------------------------------------------
    // Bind<T, U>(Func<T, Operation<U>>)
    // -------------------------------------------------------------
    [Fact]
    public void Bind_OnSuccess_BindsToNewOperation()
    {
        Operation<int> op = Operation.Success(10);

        Operation<string> result = op.Bind(x => Operation.Success($"Value: {x}"));

        Operation<string>.Success success = Assert.IsType<Operation<string>.Success>(result);
        Assert.Equal("Value: 10", success.Result);
    }

    [Fact]
    public void Bind_OnSuccess_CanReturnFailure()
    {
        Operation<int> op = Operation.Success(10);
        Error.Validation error = new Error.Validation("bad");

        Operation<string> result = op.Bind(x => Operation.FailureOf<string>(error));

        Operation<string>.Failure failure = Assert.IsType<Operation<string>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    [Fact]
    public void Bind_OnFailure_PreservesError()
    {
        Error.NotFound error = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<string> result = op.Bind(x => Operation.Success($"Value: {x}"));

        Operation<string>.Failure failure = Assert.IsType<Operation<string>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    // -------------------------------------------------------------
    // BindAsync<T, U>(Func<T, Task<Operation<U>>>)
    // -------------------------------------------------------------
    [Fact]
    public async Task BindAsync_Func_OnSuccess_BindsToNewOperation()
    {
        Operation<int> op = Operation.Success(5);

        Operation<string> result = await op.BindAsync(async x =>
        {
            await Task.Delay(1);
            return Operation.Success($"Mapped: {x}");
        });

        Operation<string>.Success success = Assert.IsType<Operation<string>.Success>(result);
        Assert.Equal("Mapped: 5", success.Result);
    }

    [Fact]
    public async Task BindAsync_Func_OnSuccess_CanReturnFailure()
    {
        Operation<int> op = Operation.Success(5);
        Error.Unauthorized error = new Error.Unauthorized("nope");

        Operation<string> result = await op.BindAsync(async x =>
        {
            await Task.Delay(1);
            return Operation.FailureOf<string>(error);
        });

        Operation<string>.Failure failure = Assert.IsType<Operation<string>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    [Fact]
    public async Task BindAsync_Func_OnFailure_PreservesError()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<string> result = await op.BindAsync(async x =>
        {
            await Task.Delay(1);
            return Operation.Success($"Mapped: {x}");
        });

        Operation<string>.Failure failure = Assert.IsType<Operation<string>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    // -------------------------------------------------------------
    // BindAsync<T, U>(Task<Operation<U>>)
    // -------------------------------------------------------------
    [Fact]
    public async Task BindAsync_Task_OnSuccess_BindsToTaskResult()
    {
        Operation<int> op = Operation.Success(42);
        Task<Operation<string>> task = Task.FromResult<Operation<string>>(Operation.Success("hello"));

        Operation<string> result = await op.BindAsync(task);

        Operation<string>.Success success = Assert.IsType<Operation<string>.Success>(result);
        Assert.Equal("hello", success.Result);
    }

    [Fact]
    public async Task BindAsync_Task_OnFailure_PreservesError()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(error);

        Task<Operation<string>> task = Task.FromResult<Operation<string>>(Operation.Success("ignored"));

        Operation<string> result = await op.BindAsync(task);

        Operation<string>.Failure failure = Assert.IsType<Operation<string>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }
}