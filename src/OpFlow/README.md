# **OpFlow**  
### *A minimal, expressive pipeline framework for C#.*

OpFlow provides a clean, predictable way to compose operations in C#.  
It centers around a single abstraction — `Operation<T>` — representing either:

- a **successful** result, or  
- a **failed** result with a structured `Error`.

No magic. No hidden behavior. No alternative syntaxes.  
Just a crisp transform canon for building narrative‑driven pipelines.

---

## **Why OpFlow?**

C# applications often accumulate:

- scattered error handling  
- inconsistent validation  
- deeply nested `try/catch` blocks  
- ad‑hoc null checks  
- unpredictable async flows  

OpFlow solves this by giving you:

- **One canonical way** to compose operations  
- **One predictable error model**  
- **One narrative** for how data flows through your system  

Everything is explicit.  
Everything is discoverable.  
Everything is composable.

---

## **The Core Abstraction: `Operation<T>`**

An `Operation<T>` is either:

```csharp
Success(T result)
Failure(Error error)
```

This shape is the foundation of OpFlow’s transform canon.

---

## **The Transform Canon**

OpFlow defines a small, intentional set of operators:

- `Map` — transform a successful value  
- `Bind` — chain dependent operations  
- `MapAsync` — async transform  
- `BindAsync` — async chaining  
- `Tap` — observe without modifying  
- `Recover` — handle failures  
- `Match` — unify success/failure  
- `Finally` — run cleanup logic  

These operators live on `Operation<T>` itself to ensure a single canonical implementation.

---

## **The `Op` Façade**

`Op` is the recommended entry point for creating operations:

```csharp
Op.Success(value)
Op.Failure<T>(error)
Op.From(value)
Op.From(func)
Op.FromAsync(func)
Op.FromException<T>(ex)
Op.Try(func)
Op.TryAsync(func)
```

These helpers wrap raw values, exceptions, and tasks into operations so your pipeline can begin cleanly.

---

## **Validation**

Validation is a first‑class part of the transform canon:

```csharp
Operation.Validate(...)
Operation.ValidateAll(...)
Operation.ValidateAsync(...)
Operation.ValidateAllAsync(...)
```

These operators aggregate multiple operations into one, collecting errors when needed.

---

## **Example**

```csharp
var result =
    Op.From(() => File.ReadAllText("config.json"))
      .Map(ParseConfig)
      .Bind(ValidateConfig)
      .BindAsync(SaveConfigAsync)
      .Tap(_ => Log("Config saved"))
      .Recover(error => LogError(error))
      .Match(
          onSuccess: _ => "OK",
          onFailure: error => $"Failed: {error.Message}"
      );
```

This is the OpFlow story:

- clear  
- sequential  
- explicit  
- readable  

---

## **Installation**

```bash
dotnet add package OpFlow
```

---

## **Design Principles**

- **One way to do things** — no LINQ sugar, no parallel sugar, no duplicates  
- **Explicit over implicit** — every step is visible  
- **Narrative‑driven pipelines** — code that reads like a story  
- **Minimal surface area** — a small API is a powerful API  

---

## **License**

MIT
