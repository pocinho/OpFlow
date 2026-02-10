// Copyright (c) 2026 Paulo Pocinho.

namespace PP.Toolkit.Operation.Tests.Canon.ControlFlow;

public class SwitchTests
{
    // -------------------------------------------------------------
    // Switch<T>(Action<T>, Action<Error>)
    // -------------------------------------------------------------
    [Fact]
    public void Switch_Success_InvokesOnSuccess()
    {
        Operation<int> op = Operation.Success(10);
        int tapped = 0;

        op.Switch(
            onSuccess: x => tapped = x,
            onFailure: _ => tapped = -1
        );

        Assert.Equal(10, tapped);
    }

    [Fact]
    public void Switch_Failure_InvokesOnFailure()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.FailureOf<int>(error);

        Error? tapped = null;

        op.Switch(
            onSuccess: _ => tapped = null,
            onFailure: err => tapped = err
        );

        Assert.Equal(error, tapped);
    }

    [Fact]
    public void Switch_OnSuccessThrows_PropagatesException()
    {
        Operation<int> op = Operation.Success(5);

        Assert.Throws<Exception>(() =>
            op.Switch(
                onSuccess: _ => throw new Exception("fail"),
                onFailure: _ => { }
            )
        );
    }

    [Fact]
    public void Switch_OnFailureThrows_PropagatesException()
    {
        Operation<int> op = Operation.FailureOf<int>(new Error.Unexpected("boom"));

        Assert.Throws<Exception>(() =>
            op.Switch(
                onSuccess: _ => { },
                onFailure: _ => throw new Exception("fail")
            )
        );
    }

    [Fact]
    public void Switch_DoesNotReturnValue()
    {
        Operation<int> op = Operation.Success(42);

        // This test ensures Switch is void-returning and compiles correctly.
        op.Switch(
            onSuccess: _ => { },
            onFailure: _ => { }
        );
    }
}