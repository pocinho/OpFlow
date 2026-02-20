// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.Guards;

public class ValidateTests
{
    // -------------------------------------------------------------
    // Validate<T>(Func<T, bool>, string message, params string[] fields)
    // -------------------------------------------------------------
    [Fact]
    public void Validate_Success_PassesPredicate_ReturnsSuccess()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = op.Validate(
            predicate: x => x > 0,
            message: "invalid"
        );

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, success.Result);
    }

    [Fact]
    public void Validate_Success_FailsPredicate_ReturnsValidationError()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = op.Validate(
            predicate: x => x < 0,
            message: "invalid",
            fields: new[] { "A", "B" }
        );

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Error.Validation validation = Assert.IsType<Error.Validation>(failure.Error);

        Assert.Equal("invalid", validation.Message);
        Assert.Equal(new[] { "A", "B" }, validation.Fields);
    }

    [Fact]
    public void Validate_Failure_DoesNotInvokePredicateOrCreateNewError()
    {
        Error.NotFound error = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(error);

        bool predicateInvoked = false;

        Operation<int> result = op.Validate(
            predicate: _ => { predicateInvoked = true; return true; },
            message: "invalid"
        );

        Assert.False(predicateInvoked);

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    [Fact]
    public void Validate_PredicateThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(5);

        Assert.Throws<InvalidOperationException>(() =>
            op.Validate(
                predicate: _ => throw new InvalidOperationException("boom"),
                message: "invalid"
            )
        );
    }

    // -------------------------------------------------------------
    // Validate<T>(Func<T, bool>, Func<T, Error>)
    // -------------------------------------------------------------
    [Fact]
    public void Validate_WithErrorFactory_FailsPredicate_ReturnsCustomError()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = op.Validate(
            predicate: x => x < 0,
            errorFactory: x => new Error.Unexpected($"bad: {x}")
        );

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Error.Unexpected unexpected = Assert.IsType<Error.Unexpected>(failure.Error);

        Assert.Equal("bad: 10", unexpected.Message);
    }

    [Fact]
    public void Validate_WithErrorFactory_ErrorFactoryThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(10);

        Assert.Throws<InvalidOperationException>(() =>
            op.Validate(
                predicate: _ => false,
                errorFactory: _ => throw new InvalidOperationException("boom")
            )
        );
    }

    // -------------------------------------------------------------
    // ValidateAsync<T>(Func<T, Task<bool>>, string message, params string[] fields)
    // -------------------------------------------------------------
    [Fact]
    public async Task ValidateAsync_Success_PassesPredicate_ReturnsSuccess()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = await op.ValidateAsync(
            predicateAsync: async x =>
            {
                await Task.Delay(1);
                return x > 0;
            },
            message: "invalid"
        );

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, success.Result);
    }

    [Fact]
    public async Task ValidateAsync_Success_FailsPredicate_ReturnsValidationError()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = await op.ValidateAsync(
            predicateAsync: async x =>
            {
                await Task.Delay(1);
                return x < 0;
            },
            message: "invalid",
            fields: new[] { "A", "B" }
        );

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Error.Validation validation = Assert.IsType<Error.Validation>(failure.Error);

        Assert.Equal("invalid", validation.Message);
        Assert.Equal(new[] { "A", "B" }, validation.Fields);
    }

    [Fact]
    public async Task ValidateAsync_Failure_DoesNotInvokePredicate()
    {
        Error.NotFound error = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(error);

        bool predicateInvoked = false;

        Operation<int> result = await op.ValidateAsync(
            predicateAsync: async _ =>
            {
                predicateInvoked = true;
                await Task.Delay(1);
                return true;
            },
            message: "invalid"
        );

        Assert.False(predicateInvoked);

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    [Fact]
    public async Task ValidateAsync_PredicateThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(5);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            op.ValidateAsync(
                predicateAsync: _ => throw new InvalidOperationException("boom"),
                message: "invalid"
            )
        );
    }

    // -------------------------------------------------------------
    // ValidateAsync<T>(Func<T, Task<bool>>, Func<T, Error>)
    // -------------------------------------------------------------
    [Fact]
    public async Task ValidateAsync_WithErrorFactory_FailsPredicate_ReturnsCustomError()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = await op.ValidateAsync(
            predicateAsync: async x =>
            {
                await Task.Delay(1);
                return x < 0;
            },
            errorFactory: x => new Error.Unexpected($"bad: {x}")
        );

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Error.Unexpected unexpected = Assert.IsType<Error.Unexpected>(failure.Error);

        Assert.Equal("bad: 10", unexpected.Message);
    }

    [Fact]
    public async Task ValidateAsync_WithErrorFactory_ErrorFactoryThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(10);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            op.ValidateAsync(
                predicateAsync: async _ =>
                {
                    await Task.Delay(1);
                    return false;
                },
                errorFactory: _ => throw new InvalidOperationException("boom")
            )
        );
    }
}