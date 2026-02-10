// Copyright (c) 2026 Paulo Pocinho.

namespace PP.Toolkit.Operation.Tests;

public class IfFailureTests
{
    // -------------------------------------------------------------
    // IfFailure<T>(Action<Error>)
    // -------------------------------------------------------------
    [Fact]
    public void IfFailure_Failure_InvokesAction()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(error);

        Error? tapped = null;

        Operation<int> result = op.IfFailure(err => tapped = err);

        Assert.Equal(error, tapped);
        Assert.IsType<Operation<int>.Failure>(result);
    }

    [Fact]
    public void IfFailure_Success_DoesNotInvokeAction()
    {
        Operation<int> op = Operation.Success(10);
        bool invoked = false;

        Operation<int> result = op.IfFailure(err => invoked = true);

        Assert.False(invoked);
        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public void IfFailure_ActionThrows_PropagatesException()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("boom"));

        Assert.Throws<Exception>(() =>
            op.IfFailure(_ => throw new Exception("fail"))
        );
    }

    // -------------------------------------------------------------
    // IfFailureAsync<T>(Func<Error, Task>)
    // -------------------------------------------------------------
    [Fact]
    public async Task IfFailureAsync_Failure_InvokesAction()
    {
        Error.Unexpected error = new Error.Unexpected("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        Error? tapped = null;

        Operation<int> result = await op.IfFailureAsync(async err =>
        {
            await Task.Delay(1);
            tapped = err;
        });

        Assert.Equal(error, tapped);
        Assert.IsType<Operation<int>.Failure>(result);
    }

    [Fact]
    public async Task IfFailureAsync_Success_DoesNotInvokeAction()
    {
        Operation<int> op = Operation.Success(5);
        bool invoked = false;

        Operation<int> result = await op.IfFailureAsync(err =>
        {
            invoked = true;
            return Task.CompletedTask;
        });

        Assert.False(invoked);
        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(5, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public async Task IfFailureAsync_ActionThrows_PropagatesException()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("boom"));

        await Assert.ThrowsAsync<Exception>(() =>
            op.IfFailureAsync<int>(async _ =>
            {
                await Task.Delay(1);
                throw new Exception("async fail");
            })
        );
    }
}