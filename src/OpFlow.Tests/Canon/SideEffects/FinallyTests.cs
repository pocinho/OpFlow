// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.SideEffects;

public class FinallyTests
{
    // -------------------------------------------------------------
    // Finally(Action)
    // -------------------------------------------------------------
    [Fact]
    public void Finally_Success_InvokesAction_AndReturnsSameSuccess()
    {
        Operation<int> op = Operation.Success(10);
        bool invoked = false;

        Operation<int> result = op.Finally(() => invoked = true);

        Assert.True(invoked);
        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, success.Result);
    }

    [Fact]
    public void Finally_Failure_InvokesAction_AndReturnsSameFailure()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);
        bool invoked = false;

        Operation<int> result = op.Finally(() => invoked = true);

        Assert.True(invoked);
        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    [Fact]
    public void Finally_ActionThrows_PropagatesException_AndDoesNotChangeResult()
    {
        Operation<int> op = Operation.Success(10);

        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() =>
            op.Finally(() => throw new InvalidOperationException("boom"))
        );

        Assert.Equal("boom", ex.Message);
    }

    [Fact]
    public void Finally_ActionInvokedExactlyOnce_ForSuccess()
    {
        Operation<int> op = Operation.Success(10);
        int count = 0;

        Operation<int> result = op.Finally(() => count++);

        Assert.Equal(1, count);
        Assert.IsType<Operation<int>.Success>(result);
    }

    [Fact]
    public void Finally_ActionInvokedExactlyOnce_ForFailure()
    {
        Error.NotFound error = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(error);
        int count = 0;

        Operation<int> result = op.Finally(() => count++);

        Assert.Equal(1, count);
        Assert.IsType<Operation<int>.Failure>(result);
    }

    // -------------------------------------------------------------
    // FinallyAsync(Func<Task>)
    // -------------------------------------------------------------
    [Fact]
    public async Task FinallyAsync_Success_InvokesAction_AndReturnsSameSuccess()
    {
        Operation<int> op = Operation.Success(10);
        bool invoked = false;

        Operation<int> result = await op.FinallyAsync(async () =>
        {
            await Task.Delay(1);
            invoked = true;
        });

        Assert.True(invoked);
        Operation<int>.Success success = Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, success.Result);
    }

    [Fact]
    public async Task FinallyAsync_Failure_InvokesAction_AndReturnsSameFailure()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);
        bool invoked = false;

        Operation<int> result = await op.FinallyAsync(async () =>
        {
            await Task.Delay(1);
            invoked = true;
        });

        Assert.True(invoked);
        Operation<int>.Failure failure = Assert.IsType<Operation<int>.Failure>(result);
        Assert.Equal(error, failure.Error);
    }

    [Fact]
    public async Task FinallyAsync_ActionThrows_PropagatesException_AndDoesNotChangeResult()
    {
        Operation<int> op = Operation.Success(10);

        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            op.FinallyAsync(async () =>
            {
                await Task.Delay(1);
                throw new InvalidOperationException("boom");
            })
        );

        Assert.Equal("boom", ex.Message);
    }

    [Fact]
    public async Task FinallyAsync_ActionInvokedExactlyOnce_ForSuccess()
    {
        Operation<int> op = Operation.Success(10);
        int count = 0;

        Operation<int> result = await op.FinallyAsync(async () =>
        {
            await Task.Delay(1);
            count++;
        });

        Assert.Equal(1, count);
        Assert.IsType<Operation<int>.Success>(result);
    }

    [Fact]
    public async Task FinallyAsync_ActionInvokedExactlyOnce_ForFailure()
    {
        Error.NotFound error = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(error);
        int count = 0;

        Operation<int> result = await op.FinallyAsync(async () =>
        {
            await Task.Delay(1);
            count++;
        });

        Assert.Equal(1, count);
        Assert.IsType<Operation<int>.Failure>(result);
    }
}