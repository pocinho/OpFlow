// Copyright (c) 2026 Paulo Pocinho.

namespace PP.Toolkit.Operation.Tests;

public class ValidateTests
{
    // -------------------------------------------------------------
    // Validate<T>(Func<T, bool>, string)
    // -------------------------------------------------------------
    [Fact]
    public void Validate_PredicateTrue_ReturnsSuccess()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = op.Validate(x => x > 5, "should not fail");

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public void Validate_PredicateFalse_ReturnsValidationFailure()
    {
        Operation<int> op = Operation.Success(3);

        Operation<int> result = op.Validate(x => x > 5, "too small");

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal("too small", ((Operation<int>.Failure)result).Error.Message);
    }

    [Fact]
    public void Validate_Failure_ShortCircuits()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<int> result = op.Validate(x => x > 0, "ignored");

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, ((Operation<int>.Failure)result).Error);
    }

    // -------------------------------------------------------------
    // Validate<T>(Func<T, bool>, Error)
    // -------------------------------------------------------------
    [Fact]
    public void Validate_CustomError_ReturnsFailure()
    {
        Operation<int> op = Operation.Success(2);
        Error.Validation custom = new Error.Validation("bad value");

        Operation<int> result = op.Validate(x => x > 5, custom);

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal("bad value", ((Operation<int>.Failure)result).Error.Message);
    }

    // -------------------------------------------------------------
    // ValidateAsync<T>(Func<T, Task<bool>>, string)
    // -------------------------------------------------------------
    [Fact]
    public async Task ValidateAsync_PredicateTrue_ReturnsSuccess()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = await op.ValidateAsync(async x =>
        {
            await Task.Delay(10);
            return x > 5;
        }, "should not fail");

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public async Task ValidateAsync_PredicateFalse_ReturnsValidationFailure()
    {
        Operation<int> op = Operation.Success(3);

        Operation<int> result = await op.ValidateAsync(async x =>
        {
            await Task.Delay(10);
            return x > 5;
        }, "too small");

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal("too small", ((Operation<int>.Failure)result).Error.Message);
    }

    [Fact]
    public async Task ValidateAsync_AsyncPredicate_Throws_PropagatesException()
    {
        Operation<int> op = Operation.Success(1);

        await Assert.ThrowsAsync<Exception>(() =>
            op.ValidateAsync<int>(async _ =>
            {
                await Task.Delay(10);
                throw new Exception("async fail");
            }, "ignored")
        );
    }

    [Fact]
    public async Task ValidateAsync_Failure_ShortCircuits()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<int> result = await op.ValidateAsync(x =>
            Task.FromResult(true), "ignored");

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, ((Operation<int>.Failure)result).Error);
    }

    // -------------------------------------------------------------
    // ValidateAsync<T>(Func<T, Task<bool>>, Error)
    // -------------------------------------------------------------
    [Fact]
    public async Task ValidateAsync_CustomError_ReturnsFailure()
    {
        Operation<int> op = Operation.Success(2);
        Error.Validation custom = new Error.Validation("bad value");

        Operation<int> result = await op.ValidateAsync(async x =>
        {
            await Task.Delay(10);
            return x > 5;
        }, custom);

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal("bad value", ((Operation<int>.Failure)result).Error.Message);
    }
}