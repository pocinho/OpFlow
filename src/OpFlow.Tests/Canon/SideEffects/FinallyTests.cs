// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.SideEffects;

public class FinallyTests
{
    // -------------------------------------------------------------
    // Finally<T>(Action)
    // -------------------------------------------------------------
    [Fact]
    public void Finally_Success_InvokesAction()
    {
        Operation<int> op = Operation.Success(10);
        bool invoked = false;

        Operation<int> result = op.Finally(() => invoked = true);

        Assert.True(invoked);
        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public void Finally_Failure_InvokesAction()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("boom"));
        bool invoked = false;

        Operation<int> result = op.Finally(() => invoked = true);

        Assert.True(invoked);
        Assert.IsType<Operation<int>.Failure>(result);
    }

    [Fact]
    public void Finally_ActionThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(1);

        Assert.Throws<Exception>(() =>
            op.Finally(() => throw new Exception("fail"))
        );
    }

    // -------------------------------------------------------------
    // FinallyAsync<T>(Func<Task>)
    // -------------------------------------------------------------
    [Fact]
    public async Task FinallyAsync_Success_InvokesAction()
    {
        Operation<int> op = Operation.Success(10);
        bool invoked = false;

        Operation<int> result = await op.FinallyAsync(() => Task.Run(() =>
        {
            invoked = true;
        }));

        Assert.True(invoked);
        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public async Task FinallyAsync_Failure_InvokesAction()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("boom"));
        bool invoked = false;

        Operation<int> result = await op.FinallyAsync(() => Task.Run(() =>
        {
            invoked = true;
        }));

        Assert.True(invoked);
        Assert.IsType<Operation<int>.Failure>(result);
    }

    [Fact]
    public async Task FinallyAsync_ActionThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(1);

        await Assert.ThrowsAsync<Exception>(() =>
            op.FinallyAsync(() => Task.FromException(new Exception("fail")))
        );
    }
}