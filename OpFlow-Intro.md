# **OpFlow — Introduction**

OpFlow is a lightweight functional‑style workflow library for C#. It gives you a single, expressive abstraction — `Operation<T>` — that models success, failure, validation, async flows, and parallel composition. The goal is simple: make everyday domain logic clearer, safer, and more composable without forcing you into a full FP ecosystem.

OpFlow embraces idiomatic C# while borrowing the best ideas from functional programming: pipelines, monadic composition, and explicit error handling. Everything is discoverable, minimal, and designed to read like a narrative.

---

# **Core Concepts at a Glance**

Below are the core features of OpFlow, each illustrated with examples taken directly from the supported API surface and unit tests from [OpFlow](https://github.com/pocinho/OpFlow).

---

## **1. Creating Operations**

An `Operation<T>` represents a computation that may succeed or fail.

```csharp
Operation<int> op = new Operation<int>.Success(10);

if (op.IsSuccess() && op.TryGet(out var value))
    Console.WriteLine(value); // 10
```

Failures carry structured errors:

```csharp
// var op = new Operation<int>("Invalid input"); // incorrect.

Operation<int> result = new Error.Validation("validation message");

if (result.IsFailure() && result.TryGetError(out Error error))
{
    string msg = error switch
    {
        Error.Validation v => v.Message,
        Error.NotFound n => n.Message,
        Error.Unauthorized a => a.Message,
        Error.Unexpected u => u.Message,
        _ => "unknown"
    };
    Console.WriteLine(msg); // "validation message."
}
```

---

## **2. Composing Workflows with LINQ**

OpFlow supports LINQ query syntax through `Select` and `SelectMany`, making pipelines read like stories.

```csharp
Operation<string> op =
    from a in new Operation<int>.Success(10)
    from b in new Operation<int>.Success(5)
    select $"sum={a + b}";
```

---

## **3. Validation Built In**

### **Simple guard validation**

```csharp
var op = new Operation<int>.Success(10)
    .Ensure(x => x < 5, x => new Error.Validation("Too large"));

op.IfFailure(error =>
    Console.WriteLine(error.GetMessage()));
```

### **Multi‑rule validation with accumulation**

```csharp
var op = new Operation<int>.Success(10);

var validation = op.ValidateAll(
    x => x == 10 ? new Error.Validation("Must not be 10") : null,
    x => x > 9 ? new Error.Validation("Too large") : null
);

validation.IfFailure(error =>
    Console.WriteLine(error.GetMessage())); // "Multiple validation errors"
```
---

## **4. Async Pipelines**

Async composition is first‑class:

```csharp
// Fake async operations returning Operation<T>
Task<Operation<int>> GetUserIdAsync() =>
    Task.FromResult<Operation<int>>(new Operation<int>.Success(42));

Task<Operation<string>> GetUserProfileAsync(int userId) =>
    Task.FromResult<Operation<string>>(new Operation<string>.Success($"Profile for {userId}"));
    
// Using SelectManyAsync to compose the workflow
var operation =
    GetUserIdAsync().SelectManyAsync(
        bindAsync: async userId =>
        {
            // simulate async work
            await Task.Delay(50);
            return await GetUserProfileAsync(userId);
        },
        project: (userId, profile) => new
        {
            Id = userId,
            Name = profile
        }
    );

// Consume the result
var resultOp = await operation;

resultOp.Match(
    onSuccess: result => Console.WriteLine($"Loaded: {result.Name}"),
    onFailure: error => Console.WriteLine($"Error: {error.GetMessage()}")
);
```

This keeps async workflows clean and linear.

---

## **5. Parallel Composition with `WhenAll`**

Combine multiple operations into a single typed result:

```csharp
 Operation<int>.Success op1 = new Operation<int>.Success(10);
 Operation<string>.Success op2 = new Operation<string>.Success("ok");

 Operation<(int, string)> result = OperationParallelExtensions.WhenAll(op1, op2);
```

---

## **6. Explicit Branching with `Match`**

Handle success and failure in a structured way:

```csharp
op.Match(
    onSuccess: v => Console.WriteLine(v),
    onFailure: e => Console.WriteLine(e.Message)
);
```

---

# **What OpFlow Gives You**

- A unified abstraction for success, failure, validation, and async  
- Declarative pipelines using LINQ  
- Built‑in validation with error accumulation  
- Natural async composition  
- Parallel workflows via `WhenAll`  
- Clear branching with `Match`  
- Minimal, dependency‑free design  

OpFlow is functional where it helps, idiomatic where it matters, and lightweight everywhere else.
