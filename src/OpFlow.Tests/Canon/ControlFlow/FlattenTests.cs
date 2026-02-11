// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.ControlFlow;

public class FlattenTests
{
    // -------------------------------------------------------------
    // Flatten<T>(Operation<Operation<T>>)
    // -------------------------------------------------------------
    [Fact]
    public void Flatten_SuccessContainingSuccess_ReturnsInnerSuccess()
    {
        Operation<Operation<int>> outer = Operation.Success(Operation.Success(10));

        Operation<int> result = outer.Flatten();

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, success.Result);
    }

    [Fact]
    public void Flatten_SuccessContainingFailure_ReturnsInnerFailure()
    {
        Error.Validation error = new Error.Validation("bad");

        Operation<Operation<int>> outer = Operation.Success(Operation.FailureOf<int>(error));

        Operation<int> result = outer.Flatten();

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    [Fact]
    public void Flatten_Failure_ReturnsFailureWithSameError()
    {
        Error.NotFound error = new Error.NotFound("missing");

        Operation<Operation<int>> outer = Operation.FailureOf<Operation<int>>(error);

        Operation<int> result = outer.Flatten();

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    [Fact]
    public void Flatten_PropagatesUnexpectedError()
    {
        Error.Unexpected error = new Error.Unexpected("boom");

        Operation<Operation<int>> outer = Operation.FailureOf<Operation<int>>(error);

        Operation<int> result = outer.Flatten();

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }
}