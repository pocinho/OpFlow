// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.Guards;

public class EnsureTests
{
    // -------------------------------------------------------------
    // Ensure<T>(Func<T, bool>, Func<T, Error>)
    // -------------------------------------------------------------
    [Fact]
    public void Ensure_Success_PassesPredicate_ReturnsSuccess()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = op.Ensure(
            predicate: x => x > 0,
            errorFactory: _ => new Error.Validation("should-not-run")
        );

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, success.Result);
    }

    [Fact]
    public void Ensure_Success_FailsPredicate_ReturnsFailure()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = op.Ensure(
            predicate: x => x < 0,
            errorFactory: x => new Error.Validation($"invalid: {x}")
        );

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Error.Validation validation = Assert.IsType<Error.Validation>(failure.Error);

        Assert.Equal("invalid: 10", validation.Message);
    }

    [Fact]
    public void Ensure_Failure_DoesNotInvokePredicateOrFactory()
    {
        Error.NotFound error = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(error);

        bool predicateInvoked = false;
        bool factoryInvoked = false;

        Operation<int> result = op.Ensure(
            predicate: _ => { predicateInvoked = true; return true; },
            errorFactory: _ => { factoryInvoked = true; return new Error.Validation("x"); }
        );

        Assert.False(predicateInvoked);
        Assert.False(factoryInvoked);

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    [Fact]
    public void Ensure_PredicateThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(5);

        Assert.Throws<InvalidOperationException>(() =>
            op.Ensure(
                predicate: _ => throw new InvalidOperationException("boom"),
                errorFactory: _ => new Error.Validation("x")
            )
        );
    }

    [Fact]
    public void Ensure_ErrorFactoryThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(5);

        Assert.Throws<InvalidOperationException>(() =>
            op.Ensure(
                predicate: _ => false,
                errorFactory: _ => throw new InvalidOperationException("boom")
            )
        );
    }

    // -------------------------------------------------------------
    // EnsureAsync<T>(Func<T, Task<bool>>, Func<T, Error>)
    // -------------------------------------------------------------
    [Fact]
    public async Task EnsureAsync_Success_PassesPredicate_ReturnsSuccess()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = await op.EnsureAsync(
            predicateAsync: async x =>
            {
                await Task.Delay(1);
                return x > 0;
            },
            errorFactory: _ => new Error.Validation("should-not-run")
        );

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, success.Result);
    }

    [Fact]
    public async Task EnsureAsync_Success_FailsPredicate_ReturnsFailure()
    {
        Operation<int> op = Operation.Success(10);

        Operation<int> result = await op.EnsureAsync(
            predicateAsync: async x =>
            {
                await Task.Delay(1);
                return x < 0;
            },
            errorFactory: x => new Error.Validation($"invalid: {x}")
        );

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Error.Validation validation = Assert.IsType<Error.Validation>(failure.Error);

        Assert.Equal("invalid: 10", validation.Message);
    }

    [Fact]
    public async Task EnsureAsync_Failure_DoesNotInvokePredicateOrFactory()
    {
        Error.NotFound error = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(error);

        bool predicateInvoked = false;
        bool factoryInvoked = false;

        Operation<int> result = await op.EnsureAsync(
            predicateAsync: async _ =>
            {
                predicateInvoked = true;
                await Task.Delay(1);
                return true;
            },
            errorFactory: _ =>
            {
                factoryInvoked = true;
                return new Error.Validation("x");
            }
        );

        Assert.False(predicateInvoked);
        Assert.False(factoryInvoked);

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    [Fact]
    public async Task EnsureAsync_PredicateThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(5);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            op.EnsureAsync(
                predicateAsync: _ => throw new InvalidOperationException("boom"),
                errorFactory: _ => new Error.Validation("x")
            )
        );
    }

    [Fact]
    public async Task EnsureAsync_ErrorFactoryThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(5);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            op.EnsureAsync(
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