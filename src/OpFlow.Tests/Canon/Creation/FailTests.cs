// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.Creation;

public class FailTests
{
    // -------------------------------------------------------------
    // Fail<T>(Error)
    // -------------------------------------------------------------
    [Fact]
    public void Fail_WithError_ReturnsFailure()
    {
        Error.NotFound error = new Error.NotFound("missing");
        Operation<int> op = Operation.Fail<int>(error);

        Assert.IsType<Operation<int>.Failure>(op);
        Assert.Equal(error, ((Operation<int>.Failure)op).Error);
    }

    // -------------------------------------------------------------
    // Fail<T>(string message)
    // -------------------------------------------------------------
    [Fact]
    public void Fail_WithMessage_CreatesUnexpectedError()
    {
        Operation<int> op = Operation.Fail<int>("boom");

        Assert.IsType<Operation<int>.Failure>(op);

        Error err = ((Operation<int>.Failure)op).Error;

        Error.Unexpected unexpected = Assert.IsType<Error.Unexpected>(err);
        Assert.Equal("boom", unexpected.Message);
        Assert.Null(unexpected.Exception);
    }

    // -------------------------------------------------------------
    // Fail<T>(string message, params string[] fields)
    // -------------------------------------------------------------
    [Fact]
    public void Fail_WithMessageAndFields_CreatesValidationError()
    {
        Operation<int> op = Operation.Fail<int>("invalid", "Name", "Email");

        Assert.IsType<Operation<int>.Failure>(op);

        Error err = ((Operation<int>.Failure)op).Error;

        Error.Validation validation = Assert.IsType<Error.Validation>(err);
        Assert.Equal("invalid", validation.Message);
        Assert.Equal(new[] { "Name", "Email" }, validation.Fields);
    }

    // -------------------------------------------------------------
    // Fail<T>(Exception ex)
    // -------------------------------------------------------------
    [Fact]
    public void Fail_WithException_CreatesUnexpectedError()
    {
        InvalidOperationException ex = new InvalidOperationException("bad");
        Operation<int> op = Operation.Fail<int>(ex);

        Assert.IsType<Operation<int>.Failure>(op);

        Error err = ((Operation<int>.Failure)op).Error;

        Error.Unexpected unexpected = Assert.IsType<Error.Unexpected>(err);
        Assert.Equal("bad", unexpected.Message);
        Assert.Equal(ex, unexpected.Exception);
    }
}