// Copyright (c) 2026 Paulo Pocinho.

using OpFlow.Extensions;

namespace OpFlow.Tests.Extensions;

public class OperationParallelExtensionsTests
{
    // ------------------------------------------------------------
    // 1. SYNC WhenAll (2)
    // ------------------------------------------------------------

    [Fact]
    public void WhenAll_Two_Success()
    {
        Operation<int>.Success op1 = new Operation<int>.Success(10);
        Operation<string>.Success op2 = new Operation<string>.Success("ok");

        Operation<(int, string)> result = OperationParallelExtensions.WhenAll(op1, op2);

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out (int, string) tuple));
        Assert.Equal(10, tuple.Item1);
        Assert.Equal("ok", tuple.Item2);
    }

    [Fact]
    public void WhenAll_Two_Failure_First()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int>.Failure op1 = new Operation<int>.Failure(error);
        Operation<string>.Success op2 = new Operation<string>.Success("ok");

        Operation<(int, string)> result = OperationParallelExtensions.WhenAll(op1, op2);

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.Equal(error, e);
    }

    [Fact]
    public void WhenAll_Two_Failure_Second()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int>.Success op1 = new Operation<int>.Success(10);
        Operation<string>.Failure op2 = new Operation<string>.Failure(error);

        Operation<(int, string)> result = OperationParallelExtensions.WhenAll(op1, op2);

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.Equal(error, e);
    }

    // ------------------------------------------------------------
    // 2. SYNC WhenAll (3)
    // ------------------------------------------------------------

    [Fact]
    public void WhenAll_Three_Success()
    {
        Operation<int>.Success op1 = new Operation<int>.Success(10);
        Operation<string>.Success op2 = new Operation<string>.Success("ok");
        Operation<bool>.Success op3 = new Operation<bool>.Success(true);

        Operation<(int, string, bool)> result = OperationParallelExtensions.WhenAll(op1, op2, op3);

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out (int, string, bool) tuple));
        Assert.Equal(10, tuple.Item1);
        Assert.Equal("ok", tuple.Item2);
        Assert.True(tuple.Item3);
    }

    [Fact]
    public void WhenAll_Three_Failure_Second()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int>.Success op1 = new Operation<int>.Success(10);
        Operation<string>.Failure op2 = new Operation<string>.Failure(error);
        Operation<bool>.Success op3 = new Operation<bool>.Success(true);

        Operation<(int, string, bool)> result = OperationParallelExtensions.WhenAll(op1, op2, op3);

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.Equal(error, e);
    }

    // ------------------------------------------------------------
    // 3. SYNC WhenAll (params)
    // ------------------------------------------------------------

    [Fact]
    public void WhenAll_Params_Success()
    {
        Operation<int>.Success[] ops = new[]
        {
            new Operation<int>.Success(1),
            new Operation<int>.Success(2),
            new Operation<int>.Success(3)
        };

        Operation<IReadOnlyList<int>> result = OperationParallelExtensions.WhenAll(ops);

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out IReadOnlyList<int> list));
        Assert.Equal(new[] { 1, 2, 3 }, list);
    }

    [Fact]
    public void WhenAll_Params_Failure()
    {
        Error.Validation error = new Error.Validation("bad");

        Operation<int>[] ops =
        [
            new Operation<int>.Success(1),
            new Operation<int>.Failure(error),
            new Operation<int>.Success(3)
        ];

        Operation<IReadOnlyList<int>> result = OperationParallelExtensions.WhenAll(ops);

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.Equal(error, e);
    }


    // ------------------------------------------------------------
    // 4. ASYNC WhenAllAsync (2)
    // ------------------------------------------------------------

    [Fact]
    public async Task WhenAllAsync_Two_Success()
    {
        Task<Operation<int>> op1 = Task.FromResult<Operation<int>>(new Operation<int>.Success(10));
        Task<Operation<string>> op2 = Task.FromResult<Operation<string>>(new Operation<string>.Success("ok"));

        Operation<(int, string)> result = await OperationParallelExtensions.WhenAllAsync(op1, op2);

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out (int, string) tuple));
        Assert.Equal(10, tuple.Item1);
        Assert.Equal("ok", tuple.Item2);
    }

    [Fact]
    public async Task WhenAllAsync_Two_Failure_First()
    {
        Error.Validation error = new Error.Validation("bad");

        Task<Operation<int>> op1 = Task.FromResult<Operation<int>>(new Operation<int>.Failure(error));
        Task<Operation<string>> op2 = Task.FromResult<Operation<string>>(new Operation<string>.Success("ok"));

        Operation<(int, string)> result = await OperationParallelExtensions.WhenAllAsync(op1, op2);

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.Equal(error, e);
    }

    // ------------------------------------------------------------
    // 5. ASYNC WhenAllAsync (3)
    // ------------------------------------------------------------

    [Fact]
    public async Task WhenAllAsync_Three_Success()
    {
        Task<Operation<int>> op1 = Task.FromResult<Operation<int>>(new Operation<int>.Success(10));
        Task<Operation<string>> op2 = Task.FromResult<Operation<string>>(new Operation<string>.Success("ok"));
        Task<Operation<bool>> op3 = Task.FromResult<Operation<bool>>(new Operation<bool>.Success(true));

        Operation<(int, string, bool)> result = await OperationParallelExtensions.WhenAllAsync(op1, op2, op3);

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out (int, string, bool) tuple));
        Assert.Equal(10, tuple.Item1);
        Assert.Equal("ok", tuple.Item2);
        Assert.True(tuple.Item3);
    }

    [Fact]
    public async Task WhenAllAsync_Three_Failure_Third()
    {
        Error.Validation error = new Error.Validation("bad");

        Task<Operation<int>> op1 = Task.FromResult<Operation<int>>(new Operation<int>.Success(10));
        Task<Operation<string>> op2 = Task.FromResult<Operation<string>>(new Operation<string>.Success("ok"));
        Task<Operation<bool>> op3 = Task.FromResult<Operation<bool>>(new Operation<bool>.Failure(error));

        Operation<(int, string, bool)> result = await OperationParallelExtensions.WhenAllAsync(op1, op2, op3);

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.Equal(error, e);
    }

    // ------------------------------------------------------------
    // 6. ASYNC WhenAllAsync (params)
    // ------------------------------------------------------------

    [Fact]
    public async Task WhenAllAsync_Params_Success()
    {
        Task<Operation<int>>[] tasks = new[]
        {
            Task.FromResult<Operation<int>>(new Operation<int>.Success(1)),
            Task.FromResult<Operation<int>>(new Operation<int>.Success(2)),
            Task.FromResult<Operation<int>>(new Operation<int>.Success(3))
        };

        Operation<IReadOnlyList<int>> result = await OperationParallelExtensions.WhenAllAsync(tasks);

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out IReadOnlyList<int> list));
        Assert.Equal(new[] { 1, 2, 3 }, list);
    }

    [Fact]
    public async Task WhenAllAsync_Params_Failure()
    {
        Error.Validation error = new Error.Validation("bad");

        Task<Operation<int>>[] tasks = new[]
        {
            Task.FromResult<Operation<int>>(new Operation<int>.Success(1)),
            Task.FromResult<Operation<int>>(new Operation<int>.Failure(error)),
            Task.FromResult<Operation<int>>(new Operation<int>.Success(3))
        };

        Operation<IReadOnlyList<int>> result = await OperationParallelExtensions.WhenAllAsync(tasks);

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.Equal(error, e);
    }
}