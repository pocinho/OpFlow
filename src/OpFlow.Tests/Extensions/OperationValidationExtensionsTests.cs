// Copyright (c) 2026 Paulo Pocinho.

using OpFlow.Extensions;

namespace OpFlow.Tests.Extensions;

public class OperationValidationExtensionsTests
{
    // ------------------------------------------------------------
    // 1. Ensure (sync)
    // ------------------------------------------------------------

    [Fact]
    public void Ensure_Success_WhenPredicateTrue()
    {
        Operation<int>.Success op = new Operation<int>.Success(10);

        Operation<int> result = op.Ensure(
            v => v > 5,
            v => new Error.Validation("too small"));

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out int value));
        Assert.Equal(10, value);
    }

    [Fact]
    public void Ensure_Failure_WhenPredicateFalse()
    {
        Operation<int>.Success op = new Operation<int>.Success(3);

        Operation<int> result = op.Ensure(
            v => v > 5,
            v => new Error.Validation("too small"));

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Error.Validation validation = Assert.IsType<Error.Validation>(e);
        Assert.Equal("too small", validation.Message);
    }

    [Fact]
    public void Ensure_PropagatesFailure()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int>.Failure op = new Operation<int>.Failure(error);

        Operation<int> result = op.Ensure(
            v => v > 5,
            v => new Error.Validation("too small"));

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.Equal(error, e);
    }

    // ------------------------------------------------------------
    // 2. EnsureAsync (async predicate)
    // ------------------------------------------------------------

    [Fact]
    public async Task EnsureAsync_Success_WhenPredicateTrue()
    {
        Task<Operation<int>> task =
            Task.FromResult<Operation<int>>(new Operation<int>.Success(10));

        Operation<int> result = await OperationValidationExtensions.EnsureAsync(
            task,
            async v =>
            {
                await Task.Delay(1);
                return v > 5;
            },
            v => new Error.Validation("too small"));

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out int value));
        Assert.Equal(10, value);
    }

    [Fact]
    public async Task EnsureAsync_Failure_WhenPredicateFalse()
    {
        Task<Operation<int>> task =
            Task.FromResult<Operation<int>>(new Operation<int>.Success(3));

        Operation<int> result = await OperationValidationExtensions.EnsureAsync(
            task,
            async v =>
            {
                await Task.Delay(1);
                return v > 5;
            },
            v => new Error.Validation("too small"));

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Error.Validation validation = Assert.IsType<Error.Validation>(e);
        Assert.Equal("too small", validation.Message);
    }

    [Fact]
    public async Task EnsureAsync_PropagatesFailure()
    {
        Error.Validation error = new Error.Validation("bad");

        Task<Operation<int>> task =
            Task.FromResult<Operation<int>>(new Operation<int>.Failure(error));

        Operation<int> result = await OperationValidationExtensions.EnsureAsync(
            task,
            async v =>
            {
                await Task.Delay(1);
                return v > 5;
            },
            v => new Error.Validation("too small"));

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.Equal(error, e);
    }

    // ------------------------------------------------------------
    // 3. Require (sync)
    // ------------------------------------------------------------

    [Fact]
    public void Require_Failure_WhenPredicateFalse()
    {
        Operation<int>.Success op = new Operation<int>.Success(3);
        Error.Validation error = new Error.Validation("bad");

        Operation<int> result = op.Require(v => v > 5, error);

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.Equal(error, e);
    }

    [Fact]
    public void Require_Success_WhenPredicateTrue()
    {
        Operation<int>.Success op = new Operation<int>.Success(10);
        Error.Validation error = new Error.Validation("bad");

        Operation<int> result = op.Require(v => v > 5, error);

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out int value));
        Assert.Equal(10, value);
    }

    // ------------------------------------------------------------
    // 4. RequireAsync (async)
    // ------------------------------------------------------------

    [Fact]
    public async Task RequireAsync_Failure_WhenPredicateFalse()
    {
        Error.Validation error = new Error.Validation("bad");

        Task<Operation<int>> task =
            Task.FromResult<Operation<int>>(new Operation<int>.Success(3));

        Operation<int> result = await OperationValidationExtensions.RequireAsync(
            task,
            async v =>
            {
                await Task.Delay(1);
                return v > 5;
            },
            error);

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.Equal(error, e);
    }

    [Fact]
    public async Task RequireAsync_Success_WhenPredicateTrue()
    {
        Error.Validation error = new Error.Validation("bad");

        Task<Operation<int>> task =
            Task.FromResult<Operation<int>>(new Operation<int>.Success(10));

        Operation<int> result = await OperationValidationExtensions.RequireAsync(
            task,
            async v =>
            {
                await Task.Delay(1);
                return v > 5;
            },
            error);

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out int value));
        Assert.Equal(10, value);
    }

    // ------------------------------------------------------------
    // 5. Validate (short-circuiting)
    // ------------------------------------------------------------

    [Fact]
    public void Validate_AllValidatorsPass()
    {
        Operation<int>.Success op = new Operation<int>.Success(10);

        Operation<int> result = op.Validate(
            v => new Operation<int>.Success(v),
            v => new Operation<int>.Success(v + 1));

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out int value));
        Assert.Equal(11, value);
    }

    [Fact]
    public void Validate_StopsOnFirstFailure()
    {
        Error.Validation error = new Error.Validation("bad");

        Operation<int>.Success op = new Operation<int>.Success(10);

        Operation<int> result = op.Validate(
            v => new Operation<int>.Failure(error),
            v => new Operation<int>.Success(v + 1)); // never executed

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.Equal(error, e);
    }

    // ------------------------------------------------------------
    // 6. ValidateAsync (short-circuiting async)
    // ------------------------------------------------------------

    [Fact]
    public async Task ValidateAsync_AllValidatorsPass()
    {
        Task<Operation<int>> task =
            Task.FromResult<Operation<int>>(new Operation<int>.Success(10));

        Operation<int> result = await OperationValidationExtensions.ValidateAsync(
            task,
            async v =>
            {
                await Task.Delay(1);
                return new Operation<int>.Success(v);
            },
            async v =>
            {
                await Task.Delay(1);
                return new Operation<int>.Success(v + 1);
            });

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out int value));
        Assert.Equal(11, value);
    }

    [Fact]
    public async Task ValidateAsync_StopsOnFirstFailure()
    {
        Error.Validation error = new Error.Validation("bad");

        Task<Operation<int>> task =
            Task.FromResult<Operation<int>>(new Operation<int>.Success(10));

        Operation<int> result = await OperationValidationExtensions.ValidateAsync(
            task,
            async v =>
            {
                await Task.Delay(1);
                return new Operation<int>.Failure(error);
            },
            async v =>
            {
                await Task.Delay(1);
                return new Operation<int>.Success(v + 1); // never executed
            });

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.Equal(error, e);
    }

    // ------------------------------------------------------------
    // 7. ValidateAll (accumulate errors)
    // ------------------------------------------------------------

    [Fact]
    public void ValidateAll_NoErrors()
    {
        Operation<int>.Success op = new Operation<int>.Success(10);

        Operation<int> result = op.ValidateAll(
            v => new Operation<int>.Success(v),
            v => new Operation<int>.Success(v + 1));

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out int value));
        Assert.Equal(10, value);
    }

    [Fact]
    public void ValidateAll_OneError()
    {
        Error.Validation error = new Error.Validation("bad");

        Operation<int>.Success op = new Operation<int>.Success(10);

        Operation<int> result = op.ValidateAll(
            v => new Operation<int>.Failure(error));

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.Equal(error, e);
    }

    [Fact]
    public void ValidateAll_MultipleErrors()
    {
        Operation<int>.Success op = new Operation<int>.Success(10);

        Operation<int> result = op.ValidateAll(
            v => new Operation<int>.Failure(new Error.Validation("e1")),
            v => new Operation<int>.Failure(new Error.Validation("e2")));

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.IsType<Error.Validation>(e);
        Error.Validation validation = Assert.IsType<Error.Validation>(e);
        Assert.Equal("Multiple validation errors", validation.Message);
    }

    // ------------------------------------------------------------
    // 8. ValidateAllAsync (accumulate async errors)
    // ------------------------------------------------------------

    [Fact]
    public async Task ValidateAllAsync_NoErrors()
    {
        Task<Operation<int>> task =
            Task.FromResult<Operation<int>>(new Operation<int>.Success(10));

        Operation<int> result = await OperationValidationExtensions.ValidateAllAsync(
            task,
            async v =>
            {
                await Task.Delay(1);
                return new Operation<int>.Success(v);
            });

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out int value));
        Assert.Equal(10, value);
    }

    [Fact]
    public async Task ValidateAllAsync_MultipleErrors()
    {
        Task<Operation<int>> task =
            Task.FromResult<Operation<int>>(new Operation<int>.Success(10));

        Operation<int> result = await OperationValidationExtensions.ValidateAllAsync(
            task,
            async v =>
            {
                await Task.Delay(1);
                return new Operation<int>.Failure(new Error.Validation("e1"));
            },
            async v =>
            {
                await Task.Delay(1);
                return new Operation<int>.Failure(new Error.Validation("e2"));
            });

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Assert.IsType<Error.Validation>(e);
        Error.Validation validation = Assert.IsType<Error.Validation>(e);
        Assert.Equal("Multiple validation errors", validation.Message);
    }

    // ------------------------------------------------------------
    // 9. NotNull / NotEmpty
    // ------------------------------------------------------------

    [Fact]
    public void NotNull_Success()
    {
        Operation<string?> op = new Operation<string?>.Success("hello");

        Operation<string> result = op.NotNull("field");

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out string value));
        Assert.Equal("hello", value);
    }

    [Fact]
    public void NotNull_Failure()
    {
        Operation<string?> op = new Operation<string?>.Success(null);

        Operation<string> result = op.NotNull("field");

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Error.Validation validation = Assert.IsType<Error.Validation>(e);
        Assert.Contains("field must not be null", validation.Message);
    }

    [Fact]
    public void NotEmpty_Failure()
    {
        Operation<string> op = new Operation<string>.Success("");

        Operation<string> result = op.NotEmpty("name");

        Assert.True(result.IsFailure());
        Assert.True(result.TryGetError(out Error e));
        Error.Validation validation = Assert.IsType<Error.Validation>(e);
        Assert.Contains("name must not be empty", validation.Message);
    }

    [Fact]
    public void NotEmpty_Success()
    {
        Operation<string> op = new Operation<string>.Success("hello");

        Operation<string> result = op.NotEmpty("name");

        Assert.True(result.IsSuccess());
        Assert.True(result.TryGet(out string value));
        Assert.Equal("hello", value);
    }
}