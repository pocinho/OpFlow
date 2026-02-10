// Copyright (c) 2026 Paulo Pocinho.

namespace PP.Toolkit.Operation.Tests.Canon.Creation;

public class FailTests
{
    // -------------------------------------------------------------
    // Fail<T>(Error error)
    // -------------------------------------------------------------
    [Fact]
    public void Fail_Error_ReturnsFailure()
    {
        Error.Unexpected error = new Error.Unexpected("boom");
        Operation<int> op = Operation.Fail<int>(error);

        Assert.IsType<Operation<int>.Failure>(op);
        Assert.Equal("boom", ((Operation<int>.Failure)op).Error.Message);
    }

    // -------------------------------------------------------------
    // Fail<T>(string message)
    // -------------------------------------------------------------
    [Fact]
    public void Fail_Message_ReturnsUnexpectedError()
    {
        Operation<int> op = Operation.Fail<int>("oops");

        Assert.IsType<Operation<int>.Failure>(op);
        Assert.Equal("oops", ((Operation<int>.Failure)op).Error.Message);
        Assert.IsType<Error.Unexpected>(((Operation<int>.Failure)op).Error);
    }

    // -------------------------------------------------------------
    // Fail<T>(string message, params string[] fields)
    // -------------------------------------------------------------
    [Fact]
    public void Fail_Validation_ReturnsValidationError()
    {
        Operation<int> op = Operation.Fail<int>("invalid", "Email", "Password");

        Assert.IsType<Operation<int>.Failure>(op);

        Error error = ((Operation<int>.Failure)op).Error;
        Assert.IsType<Error.Validation>(error);

        Assert.Equal("invalid", error.Message);
        Assert.Equal(new[] { "Email", "Password" }, ((Error.Validation)error).Fields);
    }

    // -------------------------------------------------------------
    // Fail<T>(Exception ex)
    // -------------------------------------------------------------
    [Fact]
    public void Fail_Exception_ReturnsUnexpectedError()
    {
        InvalidOperationException ex = new InvalidOperationException("bad");
        Operation<int> op = Operation.Fail<int>(ex);

        Assert.IsType<Operation<int>.Failure>(op);

        Error error = ((Operation<int>.Failure)op).Error;
        Assert.IsType<Error.Unexpected>(error);

        Assert.Equal("bad", error.Message);
        Assert.Equal(ex, ((Error.Unexpected)error).Exception);
    }
}