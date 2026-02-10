// Copyright (c) 2026 Paulo Pocinho.

namespace PP.Toolkit.Operation.Tests.Canon.ControlFlow;

public class FlattenTests
{
    // -------------------------------------------------------------
    // Flatten<T>(Operation<Operation<T>>)
    // -------------------------------------------------------------
    [Fact]
    public void Flatten_SuccessOfSuccess_ReturnsInnerSuccess()
    {
        Operation<int> inner = Operation.Success(10);
        Operation<Operation<int>> outer = Operation.Success(inner);

        Operation<int> result = outer.Flatten();

        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public void Flatten_SuccessOfFailure_ReturnsInnerFailure()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> inner = Operation.FailureOf<int>(error);
        Operation<Operation<int>> outer = Operation.Success(inner);

        Operation<int> result = outer.Flatten();

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, ((Operation<int>.Failure)result).Error);
    }

    [Fact]
    public void Flatten_Failure_ReturnsFailure()
    {
        Error.Unexpected error = new Error.Unexpected("outer");
        Operation<Operation<int>> outer = Operation.FailureOf<Operation<int>>(error);

        Operation<int> result = outer.Flatten();

        Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, ((Operation<int>.Failure)result).Error);
    }

    [Fact]
    public void Flatten_DoesNotWrapInnerError()
    {
        Error.Unexpected innerError = new Error.Unexpected("inner");
        Operation<int> inner = Operation.FailureOf<int>(innerError);
        Operation<Operation<int>> outer = Operation.Success(inner);

        Operation<int> result = outer.Flatten();

        Assert.Same(innerError, ((Operation<int>.Failure)result).Error);
    }

    [Fact]
    public void Flatten_DoesNotWrapOuterError()
    {
        Error.Unexpected outerError = new Error.Unexpected("outer");
        Operation<Operation<int>> outer = Operation.FailureOf<Operation<int>>(outerError);

        Operation<int> result = outer.Flatten();

        Assert.Same(outerError, ((Operation<int>.Failure)result).Error);
    }
}