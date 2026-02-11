// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.Transform;

public class BindErrorTests
{
    // -------------------------------------------------------------
    // BindError<T>(Func<Error, Operation<T>>)
    // -------------------------------------------------------------
    [Fact]
    public void BindError_OnFailure_BindsToNewOperation()
    {
        Error.NotFound original = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(original);

        Operation<int> result = op.BindError(err => Operation.Success(99));

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(99, success.Result);
    }

    [Fact]
    public void BindError_OnFailure_CanReturnNewFailure()
    {
        Error.Validation original = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(original);

        Error.Unauthorized mapped = new Error.Unauthorized("nope");

        Operation<int> result = op.BindError(err => Operation.FailureOf<int>(mapped));

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(mapped, failure.Error);
    }

    [Fact]
    public void BindError_OnSuccess_PreservesValue()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = op.BindError(err => Operation.FailureOf<int>(new Error.NotFound("ignored")));

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, success.Result);
    }

    // -------------------------------------------------------------
    // BindErrorAsync<T>(Func<Error, Task<Operation<T>>>)
    // -------------------------------------------------------------
    [Fact]
    public async Task BindErrorAsync_Func_OnFailure_BindsToNewOperation()
    {
        Error.Unexpected original = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(original);

        Operation<int> result = await op.BindErrorAsync(async err =>
        {
            await Task.Delay(1);
            return Operation.Success(123);
        });

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(123, success.Result);
    }

    [Fact]
    public async Task BindErrorAsync_Func_OnFailure_CanReturnNewFailure()
    {
        Error.Validation original = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(original);

        Error.NotFound mapped = new Error.NotFound("missing");

        Operation<int> result = await op.BindErrorAsync(async err =>
        {
            await Task.Delay(1);
            return Operation.FailureOf<int>(mapped);
        });

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(mapped, failure.Error);
    }

    [Fact]
    public async Task BindErrorAsync_Func_OnSuccess_PreservesValue()
    {
        Operation<int> op = Operation.Success(5);

        Operation<int> result = await op.BindErrorAsync(async err =>
        {
            await Task.Delay(1);
            return Operation.FailureOf<int>(new Error.Validation("ignored"));
        });

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(5, success.Result);
    }

    // -------------------------------------------------------------
    // BindErrorAsync<T>(Task<Operation<T>>)
    // -------------------------------------------------------------
    [Fact]
    public async Task BindErrorAsync_Task_OnFailure_BindsToTaskResult()
    {
        Error.NotFound original = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(original);

        Task<Operation<int>> task = Task.FromResult<Operation<int>>(Operation.Success(777));

        Operation<int> result = await op.BindErrorAsync(task);

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(777, success.Result);
    }

    [Fact]
    public async Task BindErrorAsync_Task_OnSuccess_PreservesValue()
    {
        Operation<int> op = Operation.Success(42);

        Task<Operation<int>> task = Task.FromResult<Operation<int>>(Operation.FailureOf<int>(new Error.Validation("ignored")));

        Operation<int> result = await op.BindErrorAsync(task);

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(42, success.Result);
    }
}