// Copyright (c) 2026 Paulo Pocinho.

namespace OpFlow.Extensions;

public static class ErrorExtensions
{
    // ------------------------------------------------------------
    // 1. AsXxx() helpers — safe extraction of union cases
    // ------------------------------------------------------------

    public static Error.Validation? AsValidation(this Error error)
        => error as Error.Validation;

    public static Error.NotFound? AsNotFound(this Error error)
        => error as Error.NotFound;

    public static Error.Unauthorized? AsUnauthorized(this Error error)
        => error as Error.Unauthorized;

    public static Error.Unexpected? AsUnexpected(this Error error)
        => error as Error.Unexpected;


    // ------------------------------------------------------------
    // 2. IsXxx() helpers — quick type checks
    // ------------------------------------------------------------

    public static bool IsValidation(this Error error)
        => error is Error.Validation;

    public static bool IsNotFound(this Error error)
        => error is Error.NotFound;

    public static bool IsUnauthorized(this Error error)
        => error is Error.Unauthorized;

    public static bool IsUnexpected(this Error error)
        => error is Error.Unexpected;


    // ------------------------------------------------------------
    // 3. GetMessage() — case‑specific message extraction
    // ------------------------------------------------------------

    public static string GetMessage(this Error error) =>
        error switch
        {
            Error.Validation v => v.Message,
            Error.NotFound n => n.Message,
            Error.Unauthorized u => u.Message,
            Error.Unexpected x => x.Message,
            _ => throw new InvalidOperationException("Unknown error case.")
        };


    // ------------------------------------------------------------
    // 4. GetException() — only meaningful for Unexpected
    // ------------------------------------------------------------

    public static Exception? GetException(this Error error) =>
        error is Error.Unexpected u ? u.Exception : null;


    // ------------------------------------------------------------
    // 5. TryGetXxx() helpers — safe extraction with out parameters
    // ------------------------------------------------------------

    public static bool TryGetValidation(this Error error, out Error.Validation? v)
    {
        v = error as Error.Validation;
        return v is not null;
    }

    public static bool TryGetNotFound(this Error error, out Error.NotFound? nf)
    {
        nf = error as Error.NotFound;
        return nf is not null;
    }

    public static bool TryGetUnauthorized(this Error error, out Error.Unauthorized? u)
    {
        u = error as Error.Unauthorized;
        return u is not null;
    }

    public static bool TryGetUnexpected(this Error error, out Error.Unexpected? x)
    {
        x = error as Error.Unexpected;
        return x is not null;
    }


    // ------------------------------------------------------------
    // 6. Match() — functional pattern matching
    // ------------------------------------------------------------

    public static TResult Match<TResult>(
        this Error error,
        Func<Error.Validation, TResult> validation,
        Func<Error.NotFound, TResult> notFound,
        Func<Error.Unauthorized, TResult> unauthorized,
        Func<Error.Unexpected, TResult> unexpected)
    {
        return error switch
        {
            Error.Validation v => validation(v),
            Error.NotFound n => notFound(n),
            Error.Unauthorized u => unauthorized(u),
            Error.Unexpected x => unexpected(x),
            _ => throw new InvalidOperationException("Unknown error case.")
        };
    }

    public static void Match(
        this Error error,
        Action<Error.Validation> validation,
        Action<Error.NotFound> notFound,
        Action<Error.Unauthorized> unauthorized,
        Action<Error.Unexpected> unexpected)
    {
        switch (error)
        {
            case Error.Validation v:
                validation(v);
                break;
            case Error.NotFound n:
                notFound(n);
                break;
            case Error.Unauthorized u:
                unauthorized(u);
                break;
            case Error.Unexpected x:
                unexpected(x);
                break;
            default:
                throw new InvalidOperationException("Unknown error case.");
        }
    }
}