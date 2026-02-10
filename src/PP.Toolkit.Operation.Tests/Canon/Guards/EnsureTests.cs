// Copyright (c) 2026 Paulo Pocinho.

namespace PP.Toolkit.Operation.Tests.Canon.Guards;

public class EnsureTests
{
    // -------------------------------------------------------------
    // Ensure<T>(Func<T, bool>, string)
    // -------------------------------------------------------------
    [Fact]
    public void Ensure_PredicateTrue_ReturnsSuccess()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = op.Ensure(x => x > 5, "should not fail");

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public void Ensure_PredicateFalse_ReturnsValidationFailure()
    {
        Operation<int> op = Operation.Success(3);

        Operation<int> result = op.Ensure(x => x > 5, "too small");

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal("too small", ((Operation<int>.Failure)result).Error.Message);
    }

    [Fact]
    public void Ensure_Failure_ShortCircuits()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<int> result = op.Ensure(x => x > 0, "ignored");

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, ((Operation<int>.Failure)result).Error);
    }

    // -------------------------------------------------------------
    // Ensure<T>(Func<T, bool>, Error)
    // -------------------------------------------------------------
    [Fact]
    public void Ensure_CustomError_ReturnsFailure()
    {
        Operation<int> op = Operation.Success(2);
        Error.Validation custom = new Error.Validation("bad value");

        Operation<int> result = op.Ensure(x => x > 5, custom);

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal("bad value", ((Operation<int>.Failure)result).Error.Message);
    }

    // -------------------------------------------------------------
    // EnsureAsync<T>(Func<T, Task<bool>>, string)
    // -------------------------------------------------------------
    [Fact]
    public async Task EnsureAsync_PredicateTrue_ReturnsSuccess()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = await op.EnsureAsync(async x =>
        {
            await Task.Delay(1);
            return x > 5;
        }, "should not fail");

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public async Task EnsureAsync_PredicateFalse_ReturnsValidationFailure()
    {
        Operation<int> op = Operation.Success(3);

        Operation<int> result = await op.EnsureAsync(async x =>
        {
            await Task.Delay(1);
            return x > 5;
        }, "too small");

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal("too small", ((Operation<int>.Failure)result).Error.Message);
    }

    [Fact]
    public async Task EnsureAsync_AsyncPredicate_Throws_PropagatesException()
    {
        Operation<int> op = Operation.Success(1);

        await Assert.ThrowsAsync<Exception>(() =>
            op.EnsureAsync<int>(async _ =>
            {
                await Task.Delay(1);
                throw new Exception("async fail");
            }, "ignored")
        );
    }

    [Fact]
    public async Task EnsureAsync_Failure_ShortCircuits()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<int> result = await op.EnsureAsync(x =>
            Task.FromResult(true), "ignored");

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, ((Operation<int>.Failure)result).Error);
    }

    // -------------------------------------------------------------
    // EnsureAsync<T>(Func<T, Task<bool>>, Error)
    // -------------------------------------------------------------
    [Fact]
    public async Task EnsureAsync_CustomError_ReturnsFailure()
    {
        Operation<int> op = Operation.Success(2);
        Error.Validation custom = new Error.Validation("bad value");

        Operation<int> result = await op.EnsureAsync(async x =>
        {
            await Task.Delay(1);
            return x > 5;
        }, custom);

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal("bad value", ((Operation<int>.Failure)result).Error.Message);
    }
}