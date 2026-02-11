// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.Transform;

public class BindTests
{
    // -------------------------------------------------------------
    // Bind<T, U>(Func<T, Operation<U>>)
    // -------------------------------------------------------------
    [Fact]
    public void Bind_OnSuccess_BindsToNewOperation()
    {
        Operation<int> op = Operation.Success(10);

        Operation<string> result = op.Bind(x => Operation.Success($"Value: {x}"));

        Operation<string>.Success success = Assert.IsType<Operation<string>.Success>(result);
        Assert.Equal("Value: 10", success.Result);
    }

    [Fact]
    public void Bind_OnSuccess_CanReturnFailure()
    {
        Operation<int> op = Operation.Success(10);
        Error.Validation error = new Error.Validation("bad");

        Operation<string> result = op.Bind(x => Operation.FailureOf<string>(error));

        Operation<string>.Failure failure = Assert.IsType<Operation<string>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    [Fact]
    public void Bind_OnFailure_PreservesError()
    {
        Error.NotFound error = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<string> result = op.Bind(x => Operation.Success($"Value: {x}"));

        Operation<string>.Failure failure = Assert.IsType<Operation<string>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }
}