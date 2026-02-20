// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.Creation;

public class FailTests
{
    // -------------------------------------------------------------
    // Fail<T>(Func<Error, Error>)
    // -------------------------------------------------------------
    [Fact]
    public void Fail_TransformsError_WhenOperationIsFailure()
    {
        Error.NotFound original = new Error.NotFound("missing");
        Operation<int> op = Operation.FromError<int>(original);

        Operation<int> result = op.Fail(err =>
            new Error.Validation("invalid", new[] { "Field" }));

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Error.Validation validation = Assert.IsType<Error.Validation>(failure.Error);

        Assert.Equal("invalid", validation.Message);
        Assert.Equal(new[] { "Field" }, validation.Fields);
    }

    [Fact]
    public void Fail_DoesNotTransform_WhenOperationIsSuccess()
    {
        Operation<int> op = Operation.FromValue(10);

        Operation<int> result = op.Fail(err =>
            new Error.Validation("should-not-run"));

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, success.Result);
    }

    // -------------------------------------------------------------
    // FailAsync<T>(Func<Error, Task<Error>>)
    // -------------------------------------------------------------
    [Fact]
    public async Task FailAsync_TransformsError_WhenOperationIsFailure()
    {
        Error.NotFound original = new Error.NotFound("missing");
        Operation<int> op = Operation.FromError<int>(original);

        Operation<int> result = await op.FailAsync(async err =>
        {
            await Task.Delay(1);
            return new Error.Validation("invalid", new[] { "Field" });
        });

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Error.Validation validation = Assert.IsType<Error.Validation>(failure.Error);

        Assert.Equal("invalid", validation.Message);
        Assert.Equal(new[] { "Field" }, validation.Fields);
    }

    [Fact]
    public async Task FailAsync_DoesNotTransform_WhenOperationIsSuccess()
    {
        Operation<int> op = Operation.FromValue(10);

        Operation<int> result = await op.FailAsync(async err =>
        {
            await Task.Delay(1);
            return new Error.Validation("should-not-run");
        });

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, success.Result);
    }
}