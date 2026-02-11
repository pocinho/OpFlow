// Copyright (c) 2026 Paulo Pocinho.

using OpFlow.Extensions;

namespace OpFlow.Tests.Core;

public class OperationErrorTests
{
    // ------------------------------------------------------------
    // 1. Validation error
    // ------------------------------------------------------------

    [Fact]
    public void ValidationError_CreatesCorrectFields()
    {
        List<string> fields = new List<string> { "Name", "Email" };
        Error.Validation err = new Error.Validation("Invalid input", fields);

        Error.Validation v = Assert.IsType<Error.Validation>(err);
        Assert.Equal("Invalid input", v.Message);
        Assert.Equal(fields, v.Fields);
    }

    [Fact]
    public void ValidationError_ToString_ContainsMessage()
    {
        Error.Validation err = new Error.Validation("Too short");

        string text = err.ToString();

        Assert.Contains("Too short", text);
        Assert.Contains("Validation", text);
    }

    // ------------------------------------------------------------
    // 2. NotFound error
    // ------------------------------------------------------------

    [Fact]
    public void NotFoundError_CreatesCorrectFields()
    {
        Error.NotFound err = new Error.NotFound("User not found");

        Error.NotFound nf = Assert.IsType<Error.NotFound>(err);
        Assert.Equal("User not found", nf.Message);
    }

    [Fact]
    public void NotFoundError_ToString_ContainsMessage()
    {
        Error.NotFound err = new Error.NotFound("Missing");

        string text = err.ToString();

        Assert.Contains("Missing", text);
        Assert.Contains("NotFound", text);
    }

    // ------------------------------------------------------------
    // 3. Unauthorized error
    // ------------------------------------------------------------

    [Fact]
    public void UnauthorizedError_CreatesCorrectFields()
    {
        Error.Unauthorized err = new Error.Unauthorized("Access denied");

        Error.Unauthorized u = Assert.IsType<Error.Unauthorized>(err);
        Assert.Equal("Access denied", u.Message);
    }

    [Fact]
    public void UnauthorizedError_ToString_ContainsMessage()
    {
        Error.Unauthorized err = new Error.Unauthorized("Denied");

        string text = err.ToString();

        Assert.Contains("Denied", text);
        Assert.Contains("Unauthorized", text);
    }

    // ------------------------------------------------------------
    // 4. Unexpected error
    // ------------------------------------------------------------

    [Fact]
    public void UnexpectedError_CreatesCorrectFields()
    {
        InvalidOperationException ex = new InvalidOperationException("boom");
        Error.Unexpected err = new Error.Unexpected("Unexpected failure", ex);

        Error.Unexpected u = Assert.IsType<Error.Unexpected>(err);
        Assert.Equal("Unexpected failure", u.Message);
        Assert.Equal(ex, u.Exception);
    }

    [Fact]
    public void UnexpectedError_ToString_ContainsMessage()
    {
        Error.Unexpected err = new Error.Unexpected("Oops");

        string text = err.ToString();

        Assert.Contains("Oops", text);
        Assert.Contains("Unexpected", text);
    }

    // ------------------------------------------------------------
    // 5. Equality semantics (records)
    // ------------------------------------------------------------

    [Fact]
    public void Errors_WithSameInstanceData_AreEqual()
    {
        string[] fields = new[] { "Field" };

        Error.Validation e1 = new Error.Validation("bad", fields);
        Error.Validation e2 = new Error.Validation("bad", fields);

        Assert.Equal(e1, e2);
        Assert.True(e1 == e2);
    }

    [Fact]
    public void Errors_WithSameData_AreStructurallyEqual()
    {
        Error.Validation e1 = new Error.Validation("bad", new[] { "Field" });
        Error.Validation e2 = new Error.Validation("bad", new[] { "Field" });

        Assert.Equal(e1.Message, e2.Message);
        Assert.Equal(e1.Fields, e2.Fields); // this uses sequence equality in xUnit
    }

    [Fact]
    public void Errors_WithDifferentData_AreNotEqual()
    {
        Error.Validation e1 = new Error.Validation("bad");
        Error.Validation e2 = new Error.Validation("worse");

        Assert.NotEqual(e1, e2);
    }

    [Fact]
    public void DifferentErrorTypes_AreNotEqual()
    {
        Error e1 = new Error.Validation("bad");
        Error e2 = new Error.NotFound("bad");

        Assert.NotEqual(e1, e2);
    }

    // ------------------------------------------------------------
    // 6. Implicit conversions into Operation<T>
    // ------------------------------------------------------------

    [Fact]
    public void ImplicitConversion_FromError_ToOperation()
    {
        Error.Validation err = new Error.Validation("bad");

        Operation<int> op = err; // implicit conversion

        Assert.True(op.IsFailure());
        Assert.True(op.TryGetError(out Error e));
        Assert.Equal(err, e);
    }

    [Fact]
    public void ImplicitConversion_FromException_ToOperation()
    {
        InvalidOperationException ex = new InvalidOperationException("boom");

        Operation<int> op = ex; // implicit conversion

        Assert.True(op.IsFailure());
        Assert.True(op.TryGetError(out Error e));

        Error.Unexpected unexpected = Assert.IsType<Error.Unexpected>(e);
        Assert.Equal("boom", unexpected.Message);
        Assert.Equal(ex, unexpected.Exception);
    }

    // ------------------------------------------------------------
    // 7. Pattern matching correctness
    // ------------------------------------------------------------

    [Fact]
    public void PatternMatching_WorksForEachCase()
    {
        Error[] errors =
        [
            new Error.Validation("v"),
            new Error.NotFound("n"),
            new Error.Unauthorized("u"),
            new Error.Unexpected("x")
        ];

        foreach (Error err in errors)
        {
            string result = err switch
            {
                Error.Validation => "validation",
                Error.NotFound => "notfound",
                Error.Unauthorized => "unauthorized",
                Error.Unexpected => "unexpected",
                _ => "unknown"
            };

            Assert.NotEqual("unknown", result);
        }
    }
}