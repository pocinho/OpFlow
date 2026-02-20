![OpFlow Banner](./img/banner.svg)

# **OpFlow**  
### *A minimal, expressive pipeline framework for C#.*

OpFlow is a small, intentional library for building clear, composable, narrative‑driven pipelines in C#.  
It gives you a single, predictable abstraction — `Operation<T>` — that represents either:

- a **successful** result, or  
- a **failed** result with a structured `Error`.

No magic. No hidden behavior. No alternative syntaxes.  
Just a crisp transform canon that makes pipelines explicit and easy to reason about.

---

## **Why OpFlow?**

Modern C# codebases often struggle with:

- scattered error handling  
- inconsistent validation  
- deeply nested `try/catch` blocks  
- ad‑hoc null checks  
- unpredictable async flows  
- multiple ways to express the same logic  

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

This is the foundation of OpFlow.  
Every transform, validation, and boundary conversion builds on this shape.

---

## **The Transform Canon**

OpFlow defines a small set of operators that form the backbone of every pipeline:

- `Map` — transform a successful value  
- `Bind` — chain dependent operations  
- `MapAsync` — async transform  
- `BindAsync` — async chaining  
- `Tap` — observe without modifying  
- `Recover` — handle failures  
- `Match` — pattern‑match success/failure  
- `Finally` — unify both branches  

These operators live on `Operation<T>` itself — not on the `Op` façade — to ensure a single canonical implementation.

---

## **The `Op` Façade**

`Op` is the recommended entry point for creating operations.  
It exposes only boundary‑level helpers:

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

Validation is a first‑class part of the transform canon.

OpFlow provides:

```csharp
Operation.Validate(...)
Operation.ValidateAll(...)
Operation.ValidateAsync(...)
Operation.ValidateAllAsync(...)
```

These operators aggregate multiple operations into one, collecting errors when needed.

No LINQ sugar.  
No parallel sugar.  
No alternative syntaxes.  
Just explicit, predictable validation.

---

## **A Simple Example**

```csharp
var op =
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
- no hidden behavior  

---

## **Design Principles**

OpFlow is built on a few strong convictions:

### **1. One way to do things**
No LINQ query syntax.  
No parallel sugar.  
No redundant helpers.  
Just the transform canon.

### **2. Explicit over implicit**
Every step is visible.  
Every failure is accounted for.  
Every async boundary is intentional.

### **3. Narrative‑driven pipelines**
Your code should read like a story.  
OpFlow helps you tell it.

### **4. Minimal surface area**
A small API is a powerful API.  
OpFlow stays out of your way.

---

## **What OpFlow Is Not**

- Not a functional programming framework  
- Not a LINQ provider  
- Not a parallel execution engine  
- Not a replacement for exceptions  
- Not a monad‑heavy abstraction layer  

OpFlow is a **pipeline framework** — nothing more, nothing less.

---

## **Installation**

```bash
dotnet add package OpFlow
```

---

## **📚 Documentation**

- Quickstart examples  
- API reference  
- Validation patterns  
- LINQ composition guide  
- Error handling patterns  

See the [doc folder in the repository](https://github.com/pocinho/OpFlow/tree/main/doc).

---

## 📄 License

MIT License

Copyright (c) 2026 Paulo Pocinho.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
