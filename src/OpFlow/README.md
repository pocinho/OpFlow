# OpFlow  
### Functional pipelines, discriminated unions, and expressive error handling for C#

OpFlow brings lightweight functional programming ergonomics to C#.  
It provides:

- A powerful `Operation<T>` discriminated union (`Success` / `Failure`)
- A rich `Error` union with multiple cases
- LINQ support (`Select`, `SelectMany`, `Where`)
- Async combinators (`SelectAsync`, `BindAsync`, `ValidateAsync`)
- Validation helpers (`Ensure`, `Require`, `ValidateAll`)
- Parallel composition (`WhenAll`, `WhenAllAsync`)
- Ergonomic helper APIs for error inspection
- Zero dependencies

OpFlow is designed to feel natural in C#, while giving you the clarity and safety of functional pipelines.

---

## ✨ Features

- **Discriminated unions** for results and errors  
- **LINQ query comprehension** support  
- **Async‑first API** for real‑world workflows  
- **Validation pipelines** (short‑circuiting or accumulating)  
- **Parallel composition** for independent operations  
- **Error helpers** for ergonomic inspection and matching  
- **Zero dependencies**  
- **Fully unit‑tested**

---

## 📦 Installation

```bash
dotnet add package OpFlow
```

---

## 🚀 Quick Start

### Basic success/failure

```csharp
Operation<int> op = new Operation<int>.Success(10);

if (op.IsSuccess() && op.TryGet(out var value))
{
    Console.WriteLine(value); // 10
}
```

### Failure with error

```csharp
Operation<int> op = new Error.Validation("Invalid input");

if (op.IsFailure() && op.TryGetError(out var error))
{
    Console.WriteLine(error.GetMessage());
}
```

---

## 🔗 LINQ Pipelines

```csharp
Operation<string> result =
    from x in new Operation<int>.Success(10)
    from y in new Operation<int>.Success(5)
    select $"sum={x + y}";
```

---

## 🧪 Validation

### Ensure

```csharp
var result = new Operation<int>.Success(10)
    .Ensure(v => v > 5, v => new Error.Validation("Too small"));
```

### ValidateAll (accumulate errors)

```csharp
var result = new Operation<int>.Success(10)
    .ValidateAll(
        v => v > 0 ? v : new Error.Validation("Must be positive"),
        v => v < 100 ? v : new Error.Validation("Too large")
    );
```

---

## ⚡ Async Pipelines

```csharp
var result = await new Operation<int>.Success(10)
    .AsTask()
    .SelectManyAsync(
        async v => new Operation<int>.Success(v + 5),
        (x, y) => x + y
    );
```

---

## 🤝 Parallel Composition

```csharp
var combined = OperationParallelExtensions.WhenAll(
    new Operation<int>.Success(10),
    new Operation<string>.Success("ok")
);

// combined is Operation<(int, string)>
```

---

## 🧭 Error Helpers

```csharp
if (error.IsValidation())
{
    var v = error.AsValidation();
    Console.WriteLine(v.Message);
}

string message = error.GetMessage();

error.Match(
    validation => Handle(validation),
    notFound   => Handle(notFound),
    unauthorized => Handle(unauthorized),
    unexpected => Handle(unexpected)
);
```

---

## 🧱 Philosophy

OpFlow is built around three principles:

1. **Clarity** — pipelines should read like intent, not ceremony  
2. **Safety** — failures must be explicit and typed  
3. **Ergonomics** — functional patterns should feel natural in C#  

It’s designed for real‑world codebases where correctness, readability, and maintainability matter.

---

## 📄 License

MIT License.
