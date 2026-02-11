// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.Guards;

public class EnsureTests
{
    // -------------------------------------------------------------
    // Ensure<T>(Func<T, bool>, string)
    // -------------------------------------------------------------
    [Fact]
    public void Ensure_WithMessage_ReturnsOriginal_WhenPredicateTrue()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = op.Ensure(x => x > 5, "too small");

        Assert.Same(op, result);
    }

    [Fact]
    public void Ensure_WithMessage_ReturnsValidationError_WhenPredicateFalse()
    {
        Operation<int> op = Operation.Success(3);

        Operation<int> result = op.Ensure(x => x > 5, "too small");

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Error.Validation validation = Assert.IsType<Error.Validation>(failure.Error);

        Assert.Equal("too small", validation.Message);
        Assert.Null(validation.Fields);
    }

    [Fact]
    public void Ensure_WithMessage_DoesNotOverrideExistingFailure()
    {
        Error.NotFound error = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<int> result = op.Ensure(x => true, "ignored");

        Assert.Same(op, result);
    }

    // -------------------------------------------------------------
    // Ensure<T>(Func<T, bool>, Error)
    // -------------------------------------------------------------
    [Fact]
    public void Ensure_WithError_ReturnsOriginal_WhenPredicateTrue()
    {
        Operation<int> op = Operation.Success(10);
        Error.Unauthorized custom = new Error.Unauthorized("nope");

        Operation<int> result = op.Ensure(x => x > 5, custom);

        Assert.Same(op, result);
    }

    [Fact]
    public void Ensure_WithError_ReturnsProvidedError_WhenPredicateFalse()
    {
        Operation<int> op = Operation.Success(3);
        Error.Unauthorized custom = new Error.Unauthorized("nope");

        Operation<int> result = op.Ensure(x => x > 5, custom);

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(custom, failure.Error);
    }

    [Fact]
    public void Ensure_WithError_DoesNotOverrideExistingFailure()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<int> result = op.Ensure(x => true, new Error.NotFound("ignored"));

        Assert.Same(op, result);
    }

    // -------------------------------------------------------------
    // EnsureAsync<T>(Func<T, Task<bool>>, string)
    // -------------------------------------------------------------
    [Fact]
    public async Task EnsureAsync_WithMessage_ReturnsOriginal_WhenPredicateTrue()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = await op.EnsureAsync(async x =>
        {
            await Task.Delay(1);
            return x > 5;
        }, "too small");

        Assert.Same(op, result);
    }

    [Fact]
    public async Task EnsureAsync_WithMessage_ReturnsValidationError_WhenPredicateFalse()
    {
        Operation<int> op = Operation.Success(3);

        Operation<int> result = await op.EnsureAsync(async x =>
        {
            await Task.Delay(1);
            return x > 5;
        }, "too small");

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Error.Validation validation = Assert.IsType<Error.Validation>(failure.Error);

        Assert.Equal("too small", validation.Message);
        Assert.Null(validation.Fields);
    }

    [Fact]
    public async Task EnsureAsync_WithMessage_DoesNotOverrideExistingFailure()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<int> result = await op.EnsureAsync(async x =>
        {
            await Task.Delay(1);
            return true;
        }, "ignored");

        Assert.Same(op, result);
    }

    // -------------------------------------------------------------
    // EnsureAsync<T>(Func<T, Task<bool>>, Error)
    // -------------------------------------------------------------
    [Fact]
    public async Task EnsureAsync_WithError_ReturnsOriginal_WhenPredicateTrue()
    {
        Operation<int> op = Operation.Success(10);
        Error.Validation custom = new Error.Validation("bad");

        Operation<int> result = await op.EnsureAsync(async x =>
        {
            await Task.Delay(1);
            return x > 5;
        }, custom);

        Assert.Same(op, result);
    }

    [Fact]
    public async Task EnsureAsync_WithError_ReturnsProvidedError_WhenPredicateFalse()
    {
        Operation<int> op = Operation.Success(3);
        Error.Validation custom = new Error.Validation("bad");

        Operation<int> result = await op.EnsureAsync(async x =>
        {
            await Task.Delay(1);
            return x > 5;
        }, custom);

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(custom, failure.Error);
    }

    [Fact]
    public async Task EnsureAsync_WithError_DoesNotOverrideExistingFailure()
    {
        Error.NotFound error = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<int> result = await op.EnsureAsync(async x =>
        {
            await Task.Delay(1);
            return true;
        }, new Error.Validation("ignored"));

        Assert.Same(op, result);
    }
}