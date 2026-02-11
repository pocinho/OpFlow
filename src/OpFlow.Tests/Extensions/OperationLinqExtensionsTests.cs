// Copyright (c) 2026 Paulo Pocinho.

using OpFlow.Extensions;

namespace OpFlow.Tests.Extensions;

public class OperationLinqExtensionsTests
{
    // ------------------------------------------------------------
    // 1. Select (Map)
    // ------------------------------------------------------------

    [Fact]
    public void Select_TransformsSuccess()
    {
        Operation<int> op = new Operation<int>.Success(10);

        Operation<string> result = op.Select(v => $"value={v}");

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out string value));
        Assert.Equal("value=10", value);
    }

    [Fact]
    public void Select_PropagatesFailure()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = new Operation<int>.Failure(error);

        Operation<string> result = op.Select(v => $"value={v}");

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.Equal(error, e);
    }

    // ------------------------------------------------------------
    // 1. SelectAsync (MapAsync)
    // ------------------------------------------------------------

    [Fact]
    public async Task SelectAsync_TransformsSuccess()
    {
        Task<Operation<int>> task =
            Task.FromResult<Operation<int>>(new Operation<int>.Success(10));

        Operation<string> result = await OperationLinqExtensions.SelectAsync(
            task,
            async v =>
            {
                await Task.Delay(1);
                return $"value={v}";
            });

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out string value));
        Assert.Equal("value=10", value);
    }

    [Fact]
    public async Task SelectAsync_PropagatesFailure()
    {
        Error.Validation error = new Error.Validation("bad");
        Task<Operation<int>> task =
            Task.FromResult<Operation<int>>(new Operation<int>.Failure(error));

        Operation<string> result = await OperationLinqExtensions.SelectAsync(
            task,
            async v =>
            {
                await Task.Delay(1);
                return $"value={v}";
            });

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.Equal(error, e);
    }

    // ------------------------------------------------------------
    // 2. SelectMany (Bind)
    // ------------------------------------------------------------

    [Fact]
    public void SelectMany_ChainsSuccess()
    {
        Operation<int> op = new Operation<int>.Success(10);

        Operation<string> result = op.SelectMany(
            v => new Operation<int>.Success(v + 5),
            (v, w) => $"sum={v + w}");

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out string value));
        Assert.Equal("sum=25", value);
    }

    [Fact]
    public void SelectMany_PropagatesFailure_FromFirst()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = new Operation<int>.Failure(error);

        Operation<string> result = op.SelectMany(
            v => new Operation<int>.Success(v + 5),
            (v, w) => $"sum={v + w}");

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.Equal(error, e);
    }

    [Fact]
    public void SelectMany_PropagatesFailure_FromSecond()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = new Operation<int>.Success(10);

        Operation<string> result = op.SelectMany(
            v => new Operation<int>.Failure(error),
            (v, w) => $"sum={v + w}");

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.Equal(error, e);
    }

    // ------------------------------------------------------------
    // 2. SelectManyAsync (BindAsync)
    // ------------------------------------------------------------

    [Fact]
    public async Task SelectManyAsync_ChainsSuccess()
    {
        Task<Operation<int>> task =
            Task.FromResult<Operation<int>>(new Operation<int>.Success(10));

        Operation<string> result = await OperationLinqExtensions.SelectManyAsync<int, int, string>(
            task,
            async v =>
            {
                await Task.Delay(1);
                return new Operation<int>.Success(v + 5);
            },
            (v, w) => $"sum={v + w}");

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out string value));
        Assert.Equal("sum=25", value);
    }

    [Fact]
    public async Task SelectManyAsync_PropagatesFailure_FromFirst()
    {
        Error.Validation error = new Error.Validation("bad");
        Task<Operation<int>> task =
            Task.FromResult<Operation<int>>(new Operation<int>.Failure(error));

        Operation<string> result = await OperationLinqExtensions.SelectManyAsync<int, int, string>(
            task,
            async v =>
            {
                await Task.Delay(1);
                return new Operation<int>.Success(v + 5);
            },
            (v, w) => $"sum={v + w}");

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.Equal(error, e);
    }

    [Fact]
    public async Task SelectManyAsync_PropagatesFailure_FromSecond()
    {
        Error.Validation error = new Error.Validation("bad");
        Task<Operation<int>> task =
            Task.FromResult<Operation<int>>(new Operation<int>.Success(10));

        Operation<string> result = await OperationLinqExtensions.SelectManyAsync<int, int, string>(
            task,
            async v =>
            {
                await Task.Delay(1);
                return new Operation<int>.Failure(error);
            },
            (v, w) => $"sum={v + w}");

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.Equal(error, e);
    }

    // ------------------------------------------------------------
    // 3. Where (filter)
    // ------------------------------------------------------------

    [Fact]
    public void Where_FiltersSuccess()
    {
        Operation<int> op = new Operation<int>.Success(10);

        Operation<int> result = op.Where(v => v > 5);

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out int value));
        Assert.Equal(10, value);
    }

    [Fact]
    public void Where_FailsPredicate()
    {
        Operation<int> op = new Operation<int>.Success(3);

        Operation<int> result = op.Where(v => v > 5);

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.IsType<Error.Validation>(e);
    }

    [Fact]
    public void Where_PropagatesFailure()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = new Operation<int>.Failure(error);

        Operation<int> result = op.Where(v => v > 5);

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.Equal(error, e);
    }

    // ------------------------------------------------------------
    // 3. WhereAsync (async filter)
    // ------------------------------------------------------------

    [Fact]
    public async Task WhereAsync_FiltersSuccess()
    {
        Task<Operation<int>> task =
            Task.FromResult<Operation<int>>(new Operation<int>.Success(10));

        Operation<int> result = await OperationLinqExtensions.WhereAsync(
            task,
            async v =>
            {
                await Task.Delay(1);
                return v > 5;
            });

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out int value));
        Assert.Equal(10, value);
    }

    [Fact]
    public async Task WhereAsync_FailsPredicate()
    {
        Task<Operation<int>> task =
            Task.FromResult<Operation<int>>(new Operation<int>.Success(3));

        Operation<int> result = await OperationLinqExtensions.WhereAsync(
            task,
            async v =>
            {
                await Task.Delay(1);
                return v > 5;
            });

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.IsType<Error.Validation>(e);
    }

    [Fact]
    public async Task WhereAsync_PropagatesFailure()
    {
        Error.Validation error = new Error.Validation("bad");
        Task<Operation<int>> task =
            Task.FromResult<Operation<int>>(new Operation<int>.Failure(error));

        Operation<int> result = await OperationLinqExtensions.WhereAsync(
            task,
            async v =>
            {
                await Task.Delay(1);
                return v > 5;
            });

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.Equal(error, e);
    }
}