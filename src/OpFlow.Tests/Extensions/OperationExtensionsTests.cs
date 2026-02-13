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

        op.OnSuccess(v => captured = v);

        Assert.Equal(42, captured);
    }

    [Fact]
    public void OnFailure_ExecutesAction()
    {
        Error captured = null!;
        Error.Validation error = new Error.Validation("bad");
        Operation<int>.Failure op = new Operation<int>.Failure(error);

        op.OnFailure(e => captured = e);

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
    // 7. Async Map / Bind
    // ------------------------------------------------------------

    [Fact]
    public async Task MapAsync_TransformsValue()
    {
        Operation<int>.Success op = new Operation<int>.Success(10);

        Operation<int> result = await op.MapAsync(async v =>
        {
            await Task.Delay(1);
            return v * 3;
        });

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out int value));
        Assert.Equal(30, value);
    }

    [Fact]
    public async Task BindAsync_ChainsAsyncOperations()
    {
        // Arrange
        Task<Operation<int>> task =
            Task.FromResult<Operation<int>>(new Operation<int>.Success(10));

        // Act
        Operation<string> result = await OperationExtensions.BindAsync<int, string>(
            task,
            async v =>
            {
                await Task.Delay(1);
                return new Operation<string>.Success($"async={v}");
            });

        // Assert
        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out string value));
        Assert.Equal("async=10", value);
    }

    // ------------------------------------------------------------
    // 8. Async Tap / OnSuccess / OnFailure
    // ------------------------------------------------------------

    [Fact]
    public async Task TapAsync_ExecutesAction()
    {
        int captured = 0;
        Task<Operation<int>> op = Task.FromResult<Operation<int>>(new Operation<int>.Success(7));

        await op.TapAsync(async v => { captured = v; await Task.Yield(); });

        Assert.Equal(7, captured);
    }

    [Fact]
    public async Task OnFailureAsync_ExecutesAction()
    {
        Error captured = null!;
        Error.Validation error = new Error.Validation("bad");
        Task<Operation<int>> op = Task.FromResult<Operation<int>>(new Operation<int>.Failure(error));

        await op.OnFailureAsync(async e => { captured = e; await Task.Yield(); });

        Assert.Equal(error, captured);
    }

    // ------------------------------------------------------------
    // 9. Async Recover
    // ------------------------------------------------------------

    [Fact]
    public async Task RecoverAsync_UsesFallback()
    {
        Error.Validation error = new Error.Validation("bad");
        Task<Operation<int>> op = Task.FromResult<Operation<int>>(new Operation<int>.Failure(error));

        Operation<int> result = await op.RecoverAsync(async _ =>
        {
            await Task.Delay(1);
            return 123;
        });

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out int value));
        Assert.Equal(123, value);
    }

    // ------------------------------------------------------------
    // 10. Match / MatchAsync
    // ------------------------------------------------------------

    [Fact]
    public void Operation_Match_ReturnsSuccessBranch()
    {
        Operation<int>.Success op = new Operation<int>.Success(10);

        string result = op.Match(
            onSuccess: v => $"value:{v}",
            onFailure: e => "error"
        );

        Assert.Equal("value:10", result);
    }

    [Fact]
    public void Operation_Match_ReturnsFailureBranch()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int>.Failure op = new Operation<int>.Failure(error);

        string result = op.Match(
            onSuccess: v => "value",
            onFailure: e => $"error:{e.GetMessage()}"
        );

        Assert.Equal("error:bad", result);
    }

    [Fact]
    public void Operation_Match_Void_InvokesSuccessBranch()
    {
        Operation<int>.Success op = new Operation<int>.Success(10);
        string? observed = null;

        op.Match(
            onSuccess: v => observed = $"value:{v}",
            onFailure: e => observed = "error"
        );

        Assert.Equal("value:10", observed);
    }

    [Fact]
    public void Operation_Match_Void_InvokesFailureBranch()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int>.Failure op = new Operation<int>.Failure(error);
        string? observed = null;

        op.Match(
            onSuccess: v => observed = "value",
            onFailure: e => observed = $"error:{e.GetMessage()}"
        );

        Assert.Equal("error:bad", observed);
    }

    [Fact]
    public async Task Operation_MatchAsync_ReturnsSuccessBranch()
    {
        Task<Operation<int>> task = Task.FromResult<Operation<int>>(new Operation<int>.Success(10));

        string result = await task.MatchAsync(
            onSuccess: async v =>
            {
                await Task.Delay(1);
                return $"value:{v}";
            },
            onFailure: async e =>
            {
                await Task.Delay(1);
                return "error";
            }
        );

        Assert.Equal("value:10", result);
    }

    [Fact]
    public async Task Operation_MatchAsync_ReturnsFailureBranch()
    {
        Error.Validation error = new Error.Validation("bad");
        Task<Operation<int>> task = Task.FromResult<Operation<int>>(new Operation<int>.Failure(error));

        string result = await task.MatchAsync(
            onSuccess: async v =>
            {
                await Task.Delay(1);
                return "value";
            },
            onFailure: async e =>
            {
                await Task.Delay(1);
                return $"error:{e.GetMessage()}";
            }
        );

        Assert.Equal("error:bad", result);
    }

    [Fact]
    public async Task Operation_MatchAsync_Void_InvokesSuccessBranch()
    {
        Task<Operation<int>> task = Task.FromResult<Operation<int>>(new Operation<int>.Success(10));
        string? observed = null;

        await task.MatchAsync(
            onSuccess: async v =>
            {
                await Task.Delay(1);
                observed = $"value:{v}";
            },
            onFailure: async e =>
            {
                await Task.Delay(1);
                observed = "error";
            }
        );

        Assert.Equal("value:10", observed);
    }

    [Fact]
    public async Task Operation_MatchAsync_Void_InvokesFailureBranch()
    {
        Error.Validation error = new Error.Validation("bad");
        Task<Operation<int>> task = Task.FromResult<Operation<int>>(new Operation<int>.Failure(error));
        string? observed = null;

        await task.MatchAsync(
            onSuccess: async v =>
            {
                await Task.Delay(1);
                observed = "value";
            },
            onFailure: async e =>
            {
                await Task.Delay(1);
                observed = $"error:{e.GetMessage()}";
            }
        );

        Assert.Equal("error:bad", observed);
    }

    [Fact]
    public void Match_Void_InvokesSuccessBranch()
    {
        // Arrange
        Operation<int>.Success op = new Operation<int>.Success(42);
        string? observed = null;

        // Act
        op.Match(
            onSuccess: v => observed = $"success:{v}",
            onFailure: e => observed = $"failure:{e.GetMessage()}"
        );

        // Assert
        Assert.Equal("success:42", observed);
    }

    [Fact]
    public void Match_Void_InvokesFailureBranch()
    {
        // Arrange
        Error.Validation error = new Error.Validation("bad");
        Operation<int>.Failure op = new Operation<int>.Failure(error);
        string? observed = null;

        // Act
        op.Match(
            onSuccess: v => observed = $"success:{v}",
            onFailure: e => observed = $"failure:{e.GetMessage()}"
        );

        // Assert
        Assert.Equal("failure:bad", observed);
    }

    [Fact]
    public void Match_Void_ThrowsOnNullSuccessHandler()
    {
        // Arrange
        Operation<int>.Success op = new Operation<int>.Success(10);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            op.Match(
                onSuccess: null!,
                onFailure: _ => { }
            )
        );
    }

    [Fact]
    public void Match_Void_ThrowsOnNullFailureHandler()
    {
        // Arrange
        Operation<int>.Failure op = new Operation<int>.Failure(new Error.Validation("bad"));

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            op.Match(
                onSuccess: _ => { },
                onFailure: null!
            )
        );
    }

    [Fact]
    public void Match_Void_DoesNotInvokeBothBranches()
    {
        // Arrange
        Operation<int>.Success op = new Operation<int>.Success(99);
        bool successCalled = false;
        bool failureCalled = false;

        // Act
        op.Match(
            onSuccess: _ => successCalled = true,
            onFailure: _ => failureCalled = true
        );

        // Assert
        Assert.True(successCalled);
        Assert.False(failureCalled);
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