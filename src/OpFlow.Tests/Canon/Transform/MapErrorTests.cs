// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.Transform;

public class MapErrorTests
{
    // -------------------------------------------------------------
    // MapError(Func<Error, Error>)
    // -------------------------------------------------------------
    [Fact]
    public void MapError_Failure_TransformsError()
    {
        Error.Validation original = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(original);

        Operation<int> result = op.MapError(err =>
            new Error.Unexpected($"wrapped:{err.Message}")
        );

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal("wrapped:bad", failure.Error.Message);
        Assert.IsType<Error.Unexpected>(failure.Error);
    }

    [Fact]
    public void MapError_Success_DoesNotInvokeMapper()
    {
        Operation<int> op = Operation.Success(10);
        bool invoked = false;

        Operation<int> result = op.MapError(err =>
        {
            invoked = true;
            return err;
        });

        Assert.False(invoked);
        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, success.Result);
    }

    [Fact]
    public void MapError_MapperThrows_PropagatesException()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        Assert.Throws<InvalidOperationException>(() =>
            op.MapError(_ => throw new InvalidOperationException("boom"))
        );
    }

    [Fact]
    public void MapError_MapperInvokedExactlyOnce()
    {
        Error.NotFound error = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(error);

        int count = 0;

        Operation<int> result = op.MapError(err =>
        {
            count++;
            return err;
        });

        Assert.Equal(1, count);
        Assert.IsType<Operation<int>.Failure>(result);
    }

    // -------------------------------------------------------------
    // MapErrorAsync(Func<Error, Task<Error>>)
    // -------------------------------------------------------------
    [Fact]
    public async Task MapErrorAsync_Failure_TransformsError()
    {
        Error.Validation original = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(original);

        Operation<int> result = await op.MapErrorAsync(async err =>
        {
            await Task.Delay(1);
            return new Error.Unexpected($"wrapped:{err.Message}");
        });

        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal("wrapped:bad", failure.Error.Message);
        Assert.IsType<Error.Unexpected>(failure.Error);
    }

    [Fact]
    public async Task MapErrorAsync_Success_DoesNotInvokeMapper()
    {
        Operation<int> op = Operation.Success(10);
        bool invoked = false;

        Operation<int> result = await op.MapErrorAsync(async err =>
        {
            invoked = true;
            await Task.Delay(1);
            return err;
        });

        Assert.False(invoked);
        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, success.Result);
    }

    [Fact]
    public async Task MapErrorAsync_MapperThrows_PropagatesException()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            op.MapErrorAsync(async _ =>
            {
                await Task.Delay(1);
                throw new InvalidOperationException("boom");
            })
        );
    }

    [Fact]
    public async Task MapErrorAsync_MapperInvokedExactlyOnce()
    {
        Error.NotFound error = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(error);

        int count = 0;

        Operation<int> result = await op.MapErrorAsync(async err =>
        {
            await Task.Delay(1);
            count++;
            return err;
        });

        Assert.Equal(1, count);
        Assert.IsType<Operation<int>.Failure>(result);
    }
}