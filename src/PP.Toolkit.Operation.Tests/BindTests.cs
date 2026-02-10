// Copyright (c) 2026 Paulo Pocinho.

namespace PP.Toolkit.Operation.Tests;

public class BindTests
{
    // ------------------------------------------------------------
    //  Helpers
    // ------------------------------------------------------------

    private static Operation<int> Ok(int value)
        => Operation.Ok(value);

    private static Operation<int> Fail(string message)
        => Operation.Fail<int>(new Error.Unexpected(message));

    // ------------------------------------------------------------
    //  Bind (sync)
    // ------------------------------------------------------------

    [Fact]
    public void Bind_OnSuccess_InvokesBinder_AndReturnsBinderResult()
    {
        // Arrange
        Operation<int> op = Ok(10);

        Operation<string> Binder(int x)
            => Operation.Ok($"Value:{x}");

        // Act
        Operation<string> result = op.Bind(Binder);

        // Assert
        Operation<string>.Success success = Assert.IsType<Operation<string>.Success>(result);
        Assert.Equal("Value:10", success.Result);
    }

    [Fact]
    public void Bind_OnFailure_DoesNotInvokeBinder_PropagatesFailure()
    {
        // Arrange
        Operation<int> op = Fail("ERR");
        bool binderCalled = false;

        Operation<string> Binder(int x)
        {
            binderCalled = true;
            return Operation.Ok("ShouldNotRun");
        }

        // Act
        Operation<string> result = op.Bind(Binder);

        // Assert
        Assert.False(binderCalled);

        Operation<string>.Failure failure = Assert.IsType<Operation<string>.Failure>(result);
        Assert.Equal("ERR", failure.Error.Message);
        Assert.Null(failure.Result);
    }

    // ------------------------------------------------------------
    //  BindAsync
    // ------------------------------------------------------------

    [Fact]
    public async Task BindAsync_OnSuccess_InvokesBinder_AndReturnsBinderResult()
    {
        // Arrange
        Operation<int> op = Ok(5);

        async Task<Operation<string>> Binder(int x)
        {
            await Task.Delay(1);
            return Operation.Ok($"Async:{x}");
        }

        // Act
        Operation<string> result = await op.BindAsync(Binder);

        // Assert
        Operation<string>.Success success = Assert.IsType<Operation<string>.Success>(result);
        Assert.Equal("Async:5", success.Result);
    }

    [Fact]
    public async Task BindAsync_OnFailure_DoesNotInvokeBinder_PropagatesFailure()
    {
        // Arrange
        Operation<int> op = Fail("ERR-ASYNC");
        bool binderCalled = false;

        async Task<Operation<string>> Binder(int x)
        {
            binderCalled = true;
            await Task.Delay(1);
            return Operation.Ok("ShouldNotRun");
        }

        // Act
        Operation<string> result = await op.BindAsync(Binder);

        // Assert
        Assert.False(binderCalled);

        Operation<string>.Failure failure = Assert.IsType<Operation<string>.Failure>(result);
        Assert.Equal("ERR-ASYNC", failure.Error.Message);
        Assert.Null(failure.Result);
    }

    // ------------------------------------------------------------
    //  Structural tests
    // ------------------------------------------------------------

    [Fact]
    public void Bind_UnknownCase_Throws()
    {
        // Arrange
        UnknownOp op = new UnknownOp();

        // Act + Assert
        Assert.Throws<InvalidOperationException>(() =>
            op.Bind<int, int>(_ => Ok(1))
        );
    }

    private sealed record UnknownOp : Operation<int> { }
}