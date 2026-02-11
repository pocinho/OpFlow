// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.ControlFlow;

public class IfSuccessTests
{
    // -------------------------------------------------------------
    // IfSuccess<T>(Action<T>)
    // -------------------------------------------------------------
    [Fact]
    public void IfSuccess_Success_InvokesAction()
    {
        Operation<int> op = Operation.Success(10);
        int tapped = 0;

        Operation<int> result = op.IfSuccess(x => tapped = x);

        Assert.Equal(10, tapped);
        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(10, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public void IfSuccess_Failure_DoesNotInvokeAction()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("boom"));
        bool invoked = false;

        Operation<int> result = op.IfSuccess(x => invoked = true);

        Assert.False(invoked);
        Assert.IsType<Operation<int>.Failure>(result);
    }

    [Fact]
    public void IfSuccess_ActionThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(1);

        Assert.Throws<Exception>(() =>
            op.IfSuccess(_ => throw new Exception("fail"))
        );
    }

    // -------------------------------------------------------------
    // IfSuccessAsync<T>(Func<T, Task>)
    // -------------------------------------------------------------
    [Fact]
    public async Task IfSuccessAsync_Success_InvokesAction()
    {
        Operation<int> op = Operation.Success(5);
        int tapped = 0;

        Operation<int> result = await op.IfSuccessAsync(async x =>
        {
            await Task.Delay(1);
            tapped = x;
        });

        Assert.Equal(5, tapped);
        Assert.IsType<Operation<int>.Success>(result);
        Assert.Equal(5, ((Operation<int>.Success)result).Result);
    }

    [Fact]
    public async Task IfSuccessAsync_Failure_DoesNotInvokeAction()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("boom"));
        bool invoked = false;

        Operation<int> result = await op.IfSuccessAsync(x =>
        {
            invoked = true;
            return Task.CompletedTask;
        });

        Assert.False(invoked);
        Assert.IsType<Operation<int>.Failure>(result);
    }

    [Fact]
    public async Task IfSuccessAsync_ActionThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(1);

        await Assert.ThrowsAsync<Exception>(() =>
            op.IfSuccessAsync<int>(async _ =>
            {
                await Task.Delay(1);
                throw new Exception("async fail");
            })
        );
    }
}