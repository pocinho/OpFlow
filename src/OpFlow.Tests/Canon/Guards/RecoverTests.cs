// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.Guards;

public class RecoverTests
{
    // -------------------------------------------------------------
    // Recover<T>(Func<Error, T>)
    // -------------------------------------------------------------
    [Fact]
    public void Recover_Failure_InvokesRecoveryFunction()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<int> result = op.Recover(err => 42);

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(42, success.Result);
    }

    [Fact]
    public void Recover_Success_DoesNotInvokeRecoveryFunction()
    {
        Operation<int> op = Operation.Success(10);
        bool invoked = false;

        Operation<int> result = op.Recover(err =>
        {
            invoked = true;
            return 0;
        });

        Assert.False(invoked);
        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, success.Result);
    }

    [Fact]
    public void Recover_RecoveryFunctionThrows_PropagatesException()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("boom"));

        Assert.Throws<InvalidOperationException>(() =>
            op.Recover(_ => throw new InvalidOperationException("fail"))
        );
    }

    // -------------------------------------------------------------
    // RecoverAsync<T>(Func<Error, Task<T>>)
    // -------------------------------------------------------------
    [Fact]
    public async Task RecoverAsync_Failure_InvokesRecoveryFunction()
    {
        Error.NotFound error = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(error);

        Operation<int> result = await op.RecoverAsync(async err =>
        {
            await Task.Delay(1);
            return 42;
        });

        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(42, success.Result);
    }

    [Fact]
    public async Task RecoverAsync_Success_DoesNotInvokeRecoveryFunction()
    {
        Operation<int> op = Operation.Success(10);
        bool invoked = false;

        Operation<int> result = await op.RecoverAsync(async err =>
        {
            invoked = true;
            await Task.Delay(1);
            return 0;
        });

        Assert.False(invoked);
        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, success.Result);
    }

    [Fact]
    public async Task RecoverAsync_RecoveryFunctionThrows_PropagatesException()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("boom"));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            op.RecoverAsync(async _ =>
            {
                await Task.Delay(1);
                throw new InvalidOperationException("fail");
            })
        );
    }
}