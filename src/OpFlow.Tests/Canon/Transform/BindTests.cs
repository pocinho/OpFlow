// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.Transform;

public class BindTests
{
    // -------------------------------------------------------------
    // Bind(Operation<T>, Func<T, Operation<U>>)
    // -------------------------------------------------------------
    [Fact]
    public void Bind_Success_InvokesBind_AndReturnsNewOperation()
    {
        Operation<int> op = Operation.Success(10);

        Operation<string> result = op.Bind(x => Operation.Success($"Value: {x}"));

        Operation<string>.Success success = Assert.IsType<Operation<string>.Success>(result);
        Assert.Equal("Value: 10", success.Result);
    }

    [Fact]
    public void Bind_Failure_SkipsBind_AndPropagatesFailure()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<string> result = op.Bind(x => Operation.Success($"Value: {x}"));

        Operation<string>.Failure failure = Assert.IsType<Operation<string>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    [Fact]
    public void Bind_CallbackThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(10);

        Assert.Throws<InvalidOperationException>(() =>
            op.Bind<int, string>(_ => throw new InvalidOperationException("boom"))
        );
    }

    // -------------------------------------------------------------
    // BindAsync(Operation<T>, Func<T, Task<Operation<U>>>)
    // -------------------------------------------------------------
    [Fact]
    public async Task BindAsync_Operation_Success_InvokesBindAsync_AndReturnsNewOperation()
    {
        Operation<int> op = Operation.Success(10);

        Operation<string> result = await op.BindAsync(async x =>
        {
            await Task.Delay(1);
            return Operation.Success($"Value: {x}");
        });

        Operation<string>.Success success = Assert.IsType<Operation<string>.Success>(result);
        Assert.Equal("Value: 10", success.Result);
    }

    [Fact]
    public async Task BindAsync_Operation_Failure_SkipsBindAsync_AndPropagatesFailure()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<string> result = await op.BindAsync(async x =>
        {
            await Task.Delay(1);
            return Operation.Success($"Value: {x}");
        });

        Operation<string>.Failure failure = Assert.IsType<Operation<string>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    [Fact]
    public async Task BindAsync_Operation_CallbackThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(10);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            op.BindAsync<int, string>(async _ =>
            {
                await Task.Delay(1);
                throw new InvalidOperationException("boom");
            })
        );
    }

    // -------------------------------------------------------------
    // BindAsync(Task<Operation<T>>, Func<T, Task<Operation<U>>>)
    // (canonical async shape)
    // -------------------------------------------------------------
    [Fact]
    public async Task BindAsync_Task_Success_InvokesBindAsync_AndReturnsNewOperation()
    {
        Task<Operation<int>> opTask = Task.FromResult(Operation.Success(10));

        Operation<string> result = await opTask.BindAsync(async x =>
        {
            await Task.Delay(1);
            return Operation.Success($"Value: {x}");
        });

        Operation<string>.Success success = Assert.IsType<Operation<string>.Success>(result);
        Assert.Equal("Value: 10", success.Result);
    }

    [Fact]
    public async Task BindAsync_Task_Failure_SkipsBindAsync_AndPropagatesFailure()
    {
        Error.Validation error = new Error.Validation("bad");
        Task<Operation<int>> opTask = Task.FromResult(Operation.FailureOf<int>(error));

        Operation<string> result = await opTask.BindAsync(async x =>
        {
            await Task.Delay(1);
            return Operation.Success($"Value: {x}");
        });

        Operation<string>.Failure failure = Assert.IsType<Operation<string>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    [Fact]
    public async Task BindAsync_Task_CallbackThrows_PropagatesException()
    {
        Task<Operation<int>> opTask = Task.FromResult(Operation.Success(10));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            opTask.BindAsync<int, string>(async _ =>
            {
                await Task.Delay(1);
                throw new InvalidOperationException("boom");
            })
        );
    }
}