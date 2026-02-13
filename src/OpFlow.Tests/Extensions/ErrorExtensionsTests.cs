// Copyright (c) 2026 Paulo Pocinho.

using OpFlow.Extensions;

namespace OpFlow.Tests.Extensions;

public class ErrorExtensionsTests
{
    // ------------------------------------------------------------
    // 1. AsXxx() helpers
    // ------------------------------------------------------------

    [Fact]
    public void AsValidation_ReturnsValidationCase()
    {
        Error.Validation err = new Error.Validation("bad");

        Error.Validation? v = err.AsValidation();

        Assert.NotNull(v);
        Assert.Equal("bad", v!.Message);
    }

    [Fact]
    public void AsValidation_ReturnsNullForOtherCases()
    {
        Error err = new Error.NotFound("missing");

        Assert.Null(err.AsValidation());
    }

    [Fact]
    public void AsUnexpected_ReturnsUnexpectedCase()
    {
        InvalidOperationException ex = new InvalidOperationException("boom");
        Error.Unexpected err = new Error.Unexpected("fail", ex);

        Error.Unexpected? u = err.AsUnexpected();

        Assert.NotNull(u);
        Assert.Equal("fail", u!.Message);
        Assert.Equal(ex, u.Exception);
    }

    // ------------------------------------------------------------
    // 2. IsXxx() helpers
    // ------------------------------------------------------------

    [Fact]
    public void IsValidation_ReturnsTrueForValidation()
    {
        Error err = new Error.Validation("bad");

        Assert.True(err.IsValidation());
        Assert.False(err.IsNotFound());
        Assert.False(err.IsUnauthorized());
        Assert.False(err.IsUnexpected());
    }

    [Fact]
    public void IsNotFound_ReturnsTrueForNotFound()
    {
        Error err = new Error.NotFound("missing");

        Assert.True(err.IsNotFound());
        Assert.False(err.IsValidation());
    }

    [Fact]
    public void IsUnexpected_ReturnsTrueForUnexpected()
    {
        Error err = new Error.Unexpected("oops");

        Assert.True(err.IsUnexpected());
        Assert.False(err.IsValidation());
    }

    // ------------------------------------------------------------
    // 3. GetMessage()
    // ------------------------------------------------------------

    [Fact]
    public void GetMessage_ReturnsValidationMessage()
    {
        Error err = new Error.Validation("bad");

        Assert.Equal("bad", err.GetMessage());
    }

    [Fact]
    public void GetMessage_ReturnsNotFoundMessage()
    {
        Error err = new Error.NotFound("missing");

        Assert.Equal("missing", err.GetMessage());
    }

    [Fact]
    public void GetMessage_ReturnsUnexpectedMessage()
    {
        Error err = new Error.Unexpected("boom");

        Assert.Equal("boom", err.GetMessage());
    }

    // ------------------------------------------------------------
    // 4. GetException()
    // ------------------------------------------------------------

    [Fact]
    public void GetException_ReturnsExceptionForUnexpected()
    {
        InvalidOperationException ex = new InvalidOperationException("boom");
        Error err = new Error.Unexpected("fail", ex);

        Assert.Equal(ex, err.GetException());
    }

    [Fact]
    public void GetException_ReturnsNullForNonUnexpected()
    {
        Error err = new Error.Validation("bad");

        Assert.Null(err.GetException());
    }

    // ------------------------------------------------------------
    // 5. TryGetXxx() helpers
    // ------------------------------------------------------------

    [Fact]
    public void TryGetValidation_ReturnsTrueForValidation()
    {
        Error err = new Error.Validation("bad");

        Assert.True(err.TryGetValidation(out Error.Validation? v));
        Assert.NotNull(v);
        Assert.Equal("bad", v!.Message);
    }

    [Fact]
    public void TryGetValidation_ReturnsFalseForOtherCases()
    {
        Error err = new Error.NotFound("missing");

        Assert.False(err.TryGetValidation(out Error.Validation? v));
        Assert.Null(v);
    }

    [Fact]
    public void TryGetUnexpected_ReturnsTrueForUnexpected()
    {
        InvalidOperationException ex = new InvalidOperationException("boom");
        Error err = new Error.Unexpected("fail", ex);

        Assert.True(err.TryGetUnexpected(out Error.Unexpected? u));
        Assert.NotNull(u);
        Assert.Equal("fail", u!.Message);
        Assert.Equal(ex, u.Exception);
    }

    // ------------------------------------------------------------
    // 6. Match() helper
    // ------------------------------------------------------------

    [Fact]
    public void Match_HandlesValidationCase()
    {
        Error err = new Error.Validation("bad");

        string result = err.Match(
            v => $"validation:{v.Message}",
            n => "notfound",
            u => "unauthorized",
            x => "unexpected"
        );

        Assert.Equal("validation:bad", result);
    }

    [Fact]
    public void Match_HandlesNotFoundCase()
    {
        Error err = new Error.NotFound("missing");

        string result = err.Match(
            v => "validation",
            n => $"notfound:{n.Message}",
            u => "unauthorized",
            x => "unexpected"
        );

        Assert.Equal("notfound:missing", result);
    }

    [Fact]
    public void Match_HandlesUnexpectedCase()
    {
        Error err = new Error.Unexpected("boom");

        string result = err.Match(
            v => "validation",
            n => "notfound",
            u => "unauthorized",
            x => $"unexpected:{x.Message}"
        );

        Assert.Equal("unexpected:boom", result);
    }

    [Fact]
    public void Error_Match_Void_InvokesCorrectBranch()
    {
        Error.NotFound notFound = new Error.NotFound("missing");
        string? observed = null;

        notFound.Match(
            v => observed = "validation",
            n => observed = $"notfound:{n.Message}",
            u => observed = "unauthorized",
            x => observed = "unexpected"
        );

        Assert.Equal("notfound:missing", observed);
    }
}