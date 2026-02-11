// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Tests.Canon.Representation;

public class ErrorToStringTests
{
    // -------------------------------------------------------------
    // Validation
    // -------------------------------------------------------------
    [Fact]
    public void ToString_Validation_FormatsCorrectly()
    {
        Error.Validation error = new Error.Validation("invalid", new[] { "Name", "Email" });

        string result = error.ToString();

        Assert.Equal("Validation: invalid", result);
    }

    [Fact]
    public void ToString_Validation_WithoutFields_FormatsCorrectly()
    {
        Error.Validation error = new Error.Validation("missing");

        string result = error.ToString();

        Assert.Equal("Validation: missing", result);
    }

    // -------------------------------------------------------------
    // NotFound
    // -------------------------------------------------------------
    [Fact]
    public void ToString_NotFound_FormatsCorrectly()
    {
        Error.NotFound error = new Error.NotFound("item not found");

        string result = error.ToString();

        Assert.Equal("NotFound: item not found", result);
    }

    // -------------------------------------------------------------
    // Unauthorized
    // -------------------------------------------------------------
    [Fact]
    public void ToString_Unauthorized_FormatsCorrectly()
    {
        Error.Unauthorized error = new Error.Unauthorized("no access");

        string result = error.ToString();

        Assert.Equal("Unauthorized: no access", result);
    }

    // -------------------------------------------------------------
    // Unexpected
    // -------------------------------------------------------------
    [Fact]
    public void ToString_Unexpected_FormatsCorrectly()
    {
        Error.Unexpected error = new Error.Unexpected("boom");

        string result = error.ToString();

        Assert.Equal("Unexpected: boom", result);
    }

    [Fact]
    public void ToString_Unexpected_WithException_FormatsCorrectly()
    {
        InvalidOperationException ex = new InvalidOperationException("fail");
        Error.Unexpected error = new Error.Unexpected("boom", ex);

        string result = error.ToString();

        Assert.Equal("Unexpected: boom", result);
    }
}