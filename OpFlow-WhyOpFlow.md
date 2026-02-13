# **Why OpFlow?**

OpFlow exists for developers who want the clarity of functional workflows without dragging a full FP ecosystem into their C# codebase. It gives you a single, expressive abstraction — `Operation<T>` — that handles success, failure, validation, and async composition with minimal ceremony and maximum readability.

### **1. One unified abstraction for real‑world workflows**
Instead of juggling `Result<T>`, `Option<T>`, `Either<L,R>`, `Task<T>`, and custom error types, OpFlow gives you a single, predictable type that models:

- success and failure  
- synchronous and asynchronous flows  
- validation with error accumulation  
- branching and matching  

All with the same API surface.  
Examples in the repo show this unification clearly: `Operation<T>`, `IsSuccess`, `TryGet`, `TryGetError`, `Match`, and async variants like `SelectManyAsync`.

### **2. Pipeline‑first design**
OpFlow embraces LINQ and monadic composition to make workflows read like narratives:

```csharp
Operation<string> op =
    from a in new Operation<int>.Success(10)
    from b in new Operation<int>.Success(5)
    select $"sum={a + b}";
```

This is idiomatic C#, not FP jargon — but it still gives you the expressive power of functional pipelines.  
The examples in your README demonstrate this clearly with `Select`, `SelectMany`, and query syntax.

### **3. Validation as a first‑class citizen**
Most libraries bolt validation on as an afterthought.  
OpFlow builds it into the core:

- `Ensure` for simple guards  
- `ValidateAll` for multi‑rule accumulation  
- `AsValidation` and `IsValidation` for switching modes  

Your tab shows examples like:

```csharp
var op = Op.Success(10)
    .ValidateAll(
        x => x > 0 ? null : new Error.Validation("Must be positive"),
        x => x < 100 ? null : new Error.Validation("Too large")
    );
```  

This makes domain validation feel natural and composable.

### **4. Async that doesn’t fight you**
Async is not a wrapper — it’s part of the flow:

- `SelectManyAsync`  
- `AsTask()`  
- async lambdas inside pipelines  

Unit test examples show async composition as a first‑class pattern, not a bolted‑on afterthought

### **5. Parallel composition made simple**
`WhenAll` lets you combine multiple operations into a single typed result:

```csharp
var combined = Op.WhenAll(
    Op.Success(10),
    Op.Success("ok")
);
```  

This gives you structured parallelism without the usual Task‑wrangling.

### **6. Ergonomic, discoverable, and idiomatic**
OpFlow avoids heavy FP constructs like typeclasses, lenses, or custom immutable collections.  
Instead, it focuses on:

- intuitive method names  
- predictable behavior  
- minimal boilerplate  
- strong IntelliSense discoverability  

It feels like C#, not a foreign language.

### **7. Lightweight and dependency‑free**
OpFlow is small, focused, and easy to adopt.  
You’re not pulling in a giant FP framework — just the tools you need to write clearer, safer workflows.

---

# **In one sentence**

**OpFlow gives you the expressive power of functional pipelines with the ergonomics and readability of idiomatic C#.**
