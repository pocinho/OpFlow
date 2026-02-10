// Copyright (c) 2026 Paulo Pocinho.

namespace PP.Toolkit.Operation.Tests;

public class ErrorMatchTests
{
    // ------------------------------------------------------------
    //  Helpers
    // ------------------------------------------------------------

    private static Error.Validation Validation(string msg, params string[] fields)
        => new(msg, fields);

    private static Error.NotFound NotFound(string msg)
        => new(msg);

    private static Error.Unauthorized Unauthorized(string msg)
        => new(msg);

    private static Error.Unexpected Unexpected(string msg)
        => new(msg);

    // ------------------------------------------------------------
    //  Match (sync)
    // ------------------------------------------------------------

    [Fact]
    public void Match_Validation_InvokesValidationDelegate()
    {
        Error.Validation error = Validation("Invalid", "FieldA");

        string result = error.Match(
            validation: v => $"VALID:{v.Message}",
            notFound: _ => "NF",
            unauthorized: _ => "UNAUTH",
            unexpected: _ => "UNEXP",
            fallback: _ => "FALLBACK"
        );

        Assert.Equal("VALID:Invalid", result);
    }

    [Fact]
    public void Match_NotFound_InvokesNotFoundDelegate()
    {
        Error.NotFound error = NotFound("Missing");

        string result = error.Match(
            validation: _ => "VALID",
            notFound: nf => $"NF:{nf.Message}",
            unauthorized: _ => "UNAUTH",
            unexpected: _ => "UNEXP",
            fallback: _ => "FALLBACK"
        );

        Assert.Equal("NF:Missing", result);
    }

    [Fact]
    public void Match_Unauthorized_InvokesUnauthorizedDelegate()
    {
        Error.Unauthorized error = Unauthorized("No access");

        string result = error.Match(
            validation: _ => "VALID",
            notFound: _ => "NF",
            unauthorized: u => $"UNAUTH:{u.Message}",
            unexpected: _ => "UNEXP",
            fallback: _ => "FALLBACK"
        );

        Assert.Equal("UNAUTH:No access", result);
    }

    [Fact]
    public void Match_Unexpected_InvokesUnexpectedDelegate()
    {
        Error.Unexpected error = Unexpected("Boom");

        string result = error.Match(
            validation: _ => "VALID",
            notFound: _ => "NF",
            unauthorized: _ => "UNAUTH",
            unexpected: u => $"UNEXP:{u.Message}",
            fallback: _ => "FALLBACK"
        );

        Assert.Equal("UNEXP:Boom", result);
    }

    [Fact]
    public void Match_UnknownCase_InvokesFallback()
    {
        UnknownError error = new UnknownError();

        string result = error.Match(
            validation: _ => "VALID",
            notFound: _ => "NF",
            unauthorized: _ => "UNAUTH",
            unexpected: _ => "UNEXP",
            fallback: fb => $"FALLBACK:{fb.GetType().Name}"
        );

        Assert.Equal("FALLBACK:UnknownError", result);
    }

    private sealed record UnknownError(string Message = "Unknown") : Error(Message);

    // ------------------------------------------------------------
    //  MatchAsync
    // ------------------------------------------------------------

    [Fact]
    public async Task MatchAsync_Validation_InvokesValidationDelegate()
    {
        Error.Validation error = Validation("Invalid");

        string result = await error.MatchAsync(
            validation: async v => { await Task.Delay(1); return $"VALID:{v.Message}"; },
            notFound: _ => Task.FromResult("NF"),
            unauthorized: _ => Task.FromResult("UNAUTH"),
            unexpected: _ => Task.FromResult("UNEXP"),
            fallback: _ => Task.FromResult("FALLBACK")
        );

        Assert.Equal("VALID:Invalid", result);
    }

    [Fact]
    public async Task MatchAsync_Fallback_InvokedForUnknownCase()
    {
        UnknownError error = new UnknownError();

        string result = await error.MatchAsync(
            validation: _ => Task.FromResult("VALID"),
            notFound: _ => Task.FromResult("NF"),
            unauthorized: _ => Task.FromResult("UNAUTH"),
            unexpected: _ => Task.FromResult("UNEXP"),
            fallback: fb => Task.FromResult($"FALLBACK:{fb.GetType().Name}")
        );

        Assert.Equal("FALLBACK:UnknownError", result);
    }
}