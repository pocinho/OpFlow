// Copyright (c) 2026 Paulo Pocinho.

using OpFlow.Extensions;

namespace OpFlow.Tests.Extensions;

public class OperationExtensionsTests
{
    // ------------------------------------------------------------
    // 1. Basic Helpers
    // ------------------------------------------------------------

    [Fact]
    public void IsSuccess_ReturnsTrue_ForSuccess()
    {
        Operation<int>.Success op = new Operation<int>.Success(10);
        Assert.True(op.IsSuccess());
        Assert.False(op.IsFailure());
    }

    [Fact]
    public void TryGet_ReturnsValue_ForSuccess()
    {
        Operation<string>.Success op = new Operation<string>.Success("hello");

        bool ok = op.TryGet(out string value);

        Assert.True(ok);
        Assert.Equal("hello", value);
    }

    [Fact]
    public void TryGetError_ReturnsError_ForFailure()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int>.Failure op = new Operation<int>.Failure(error);

        bool ok = op.TryGetError(out Error e);

        Assert.True(ok);
        Assert.Equal(error, e);
    }

    // ------------------------------------------------------------
    // 2. Tap / TapError
    // ------------------------------------------------------------

    [Fact]
    public void Tap_ExecutesAction_OnSuccess()
    {
        int captured = 0;
        Operation<int>.Success op = new Operation<int>.Success(5);

        op.Tap(v => captured = v);

        Assert.Equal(5, captured);
    }

    [Fact]
    public void TapError_ExecutesAction_OnFailure()
    {
        Error captured = null!;
        Error.Validation error = new Error.Validation("bad");
        Operation<int>.Failure op = new Operation<int>.Failure(error);

        op.TapError(e => captured = e);

        Assert.Equal(error, captured);
    }

    // ------------------------------------------------------------
    // 3. OnSuccess / OnFailure
    // ------------------------------------------------------------

    [Fact]
    public void OnSuccess_ExecutesAction()
    {
        int captured = 0;
        Operation<int>.Success op = new Operation<int>.Success(42);

        op.IfSuccess(v => captured = v);

        Assert.Equal(42, captured);
    }

    [Fact]
    public void OnFailure_ExecutesAction()
    {
        Error captured = null!;
        Error.Validation error = new Error.Validation("bad");
        Operation<int>.Failure op = new Operation<int>.Failure(error);

        op.IfFailure(e => captured = e);

        Assert.Equal(error, captured);
    }

    // ------------------------------------------------------------
    // 4. Map
    // ------------------------------------------------------------

    [Fact]
    public void Map_TransformsValue_OnSuccess()
    {
        Operation<int>.Success op = new Operation<int>.Success(10);

        Operation<int> result = op.Map(v => v * 2);

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out int value));
        Assert.Equal(20, value);
    }

    [Fact]
    public void Map_PreservesError_OnFailure()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int>.Failure op = new Operation<int>.Failure(error);

        Operation<int> result = op.Map(v => v * 2);

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.Equal(error, e);
    }

    // ------------------------------------------------------------
    // 5. Bind
    // ------------------------------------------------------------

    [Fact]
    public void Bind_ChainsOperations()
    {
        Operation<int>.Success op = new Operation<int>.Success(10);

        Operation<string> result = op.Bind(v => new Operation<string>.Success($"v={v}"));

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out string value));
        Assert.Equal("v=10", value);
    }

    [Fact]
    public void Bind_PropagatesFailure()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int>.Failure op = new Operation<int>.Failure(error);

        Operation<string> result = op.Bind(v => new Operation<string>.Success("ignored"));

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.Equal(error, e);
    }

    // ------------------------------------------------------------
    // 6. Recover
    // ------------------------------------------------------------

    [Fact]
    public void Recover_ReturnsOriginal_OnSuccess()
    {
        Operation<int>.Success op = new Operation<int>.Success(5);

        Operation<int> result = op.Recover(_ => 99);

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out int value));
        Assert.Equal(5, value);
    }

    [Fact]
    public void Recover_UsesFallback_OnFailure()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int>.Failure op = new Operation<int>.Failure(error);

        Operation<int> result = op.Recover(_ => 99);

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out int value));
        Assert.Equal(99, value);
    }


    // ------------------------------------------------------------
    // 11. AsTask returns a completed Task
    // ------------------------------------------------------------

    [Fact]
    public async Task AsTask_ReturnsCompletedTask_ForSuccess()
    {
        Operation<int>.Success op = new Operation<int>.Success(10);

        Task<Operation<int>> task = op.AsTask();

        Assert.True(task.IsCompleted);

        Operation<int> result = await task;
        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out int value));
        Assert.Equal(10, value);
    }

    [Fact]
    public async Task AsTask_ReturnsCompletedTask_ForFailure()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int>.Failure op = new Operation<int>.Failure(error);

        Task<Operation<int>> task = op.AsTask();

        Assert.True(task.IsCompleted);

        Operation<int> result = await task;
        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.Equal(error, e);
    }

    // ------------------------------------------------------------
    // 12. AsTask preserves the exact instance
    // ------------------------------------------------------------

    [Fact]
    public async Task AsTask_PreservesInstance()
    {
        Operation<int>.Success op = new Operation<int>.Success(42);

        Operation<int> result = await op.AsTask();

        Assert.Same(op, result);
    }

    // ------------------------------------------------------------
    // 13. AsTask works inside async pipelines
    // ------------------------------------------------------------

    [Fact]
    public async Task AsTask_WorksInAsyncPipeline()
    {
        Operation<int>.Success op = new Operation<int>.Success(10);

        Operation<int> result =
            await op
                .AsTask()
                .SelectAsync(async v =>
                {
                    await Task.Delay(1);
                    return v + 5;
                });

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out int value));
        Assert.Equal(15, value);
    }

    [Fact]
    public async Task AsTask_WorksInAsyncBindPipeline()
    {
        Operation<int>.Success op = new Operation<int>.Success(10);

        Operation<string> result =
            await op
                .AsTask()
                .SelectManyAsync<int, string, string>(
                    async v =>
                    {
                        await Task.Delay(1);
                        return new Operation<string>.Success($"value={v}");
                    },
                    (v, s) => $"{s} (from {v})"
                );

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out string value));
        Assert.Equal("value=10 (from 10)", value);
    }

    // ------------------------------------------------------------
    // 14. AsTask does not swallow failures in async pipelines
    // ------------------------------------------------------------

    [Fact]
    public async Task AsTask_PropagatesFailureInAsyncPipeline()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int>.Failure op = new Operation<int>.Failure(error);

        Operation<int> result =
            await op
                .AsTask()
                .SelectAsync(async v =>
                {
                    await Task.Delay(1);
                    return v + 1; // should never run
                });

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.Equal(error, e);
    }
}