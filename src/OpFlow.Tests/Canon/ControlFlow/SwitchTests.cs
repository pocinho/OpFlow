// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.ControlFlow;

public class SwitchTests
{
    // -------------------------------------------------------------
    // Switch<T>(Action<T>, Action<Error>)
    // -------------------------------------------------------------
    [Fact]
    public void Switch_OnSuccess_InvokesOnSuccess()
    {
        Operation<int> op = Operation.Success(10);

        int captured = 0;

        op.Switch(
            onSuccess: x => captured = x * 2,
            onFailure: _ => captured = -1
        );

        Assert.Equal(20, captured);
    }

    [Fact]
    public void Switch_OnFailure_InvokesOnFailure()
    {
        Error.Validation error = new Error.Validation("bad");
        Operation<int> op = Operation.FailureOf<int>(error);

        Error? captured = null;

        op.Switch(
            onSuccess: _ => captured = null,
            onFailure: err => captured = err
        );

        Assert.Equal(error, captured);
    }

    [Fact]
    public void Switch_OnSuccessThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(5);

        Assert.Throws<InvalidOperationException>(() =>
            op.Switch(
                onSuccess: _ => throw new InvalidOperationException("boom"),
                onFailure: _ => { }
            )
        );
    }

    [Fact]
    public void Switch_OnFailureThrows_PropagatesException()
    {
        Error.NotFound error = new Error.NotFound("missing");
        Operation<int> op = Operation.FailureOf<int>(error);

        Assert.Throws<InvalidOperationException>(() =>
            op.Switch(
                onSuccess: _ => { },
                onFailure: _ => throw new InvalidOperationException("boom")
            )
        );
    }

    [Fact]
    public void Switch_DoesNotReturnValue()
    {
        Operation<int> op = Operation.Success(42);

        // Just ensure it compiles and runs
        op.Switch(
            onSuccess: _ => { },
            onFailure: _ => { }
        );
    }
}