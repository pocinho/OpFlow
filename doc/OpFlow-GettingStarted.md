# **Getting Started with OpFlow**

OpFlow is a lightweight workflow and validation library for C#.  
It gives you a single, expressive abstraction — `Operation<T>` — that models success, failure, validation, async flows, and parallel composition.

The `Op` facade provides a clean, discoverable entry point for all core operations.

---

## **Install via NuGet**

```bash
dotnet add package OpFlow
```

---

## **1. Creating Operations**

```csharp
var ok = Op.Success(42);
var fail = Op.Failure<int>(new Error.Validation("Invalid value"));
```

You can also wrap existing values:

```csharp
var op = Op.From("hello");
```

---

## **2. Composing Workflows**

OpFlow supports LINQ-style composition through `Select` and `SelectMany`:

```csharp
var op =
    from a in Op.From(10)
    from b in Op.From(5)
    select a + b;
```

Or using the facade directly:

```csharp
var result = Op.Map(Op.From(10), x => x * 2);
```

---

## **3. Validation**

### **Short‑circuiting validation**

```csharp
var op = Op.From(10);

var validated = Op.Validate(
    op,
    x => x > 0 ? null : new Error.Validation("Must be positive"),
    x => x < 100 ? null : new Error.Validation("Too large")
);
```

### **Accumulating validation**

```csharp
var op = Op.From(10);

var validated = Op.ValidateAll(
    op,
    x => x == 10 ? new Error.Validation("Must not be 10") : null,
    x => x > 9 ? new Error.Validation("Too large") : null
);
```

---

## **4. Async Workflows**

```csharp
Task<Operation<int>> loadUserId() =>
    Task.FromResult(Op.Success(42));

Task<Operation<string>> loadProfile(int id) =>
    Task.FromResult(Op.Success($"Profile for {id}"));

var op =
    loadUserId().SelectManyAsync(
        bindAsync: loadProfile,
        project: (id, profile) => new { id, profile }
    );

var result = await op;
```

---

## **5. Parallel Composition**

```csharp
var a = Op.From(10);
var b = Op.From("ok");

var combined = Op.WhenAll(a, b);
// Operation<(int, string)>
```

Async version:

```csharp
var combined = await Op.WhenAllAsync(loadUserId(), loadProfile(42));
```

---

## **6. Branching with Match**

### **Void-returning (ergonomic)**

```csharp
result.Match(
    onSuccess: r => Console.WriteLine($"Loaded: {r.profile}"),
    onFailure: e => Console.WriteLine($"Error: {e.GetMessage()}")
);
```

### **Value-returning**

```csharp
string message = result.Match(
    onSuccess: r => $"Loaded {r.profile}",
    onFailure: e => $"Error: {e.GetMessage()}"
);
```

---

## **7. Fluent Side Effects**

```csharp
Op.From(10)
  .Tap(x => Console.WriteLine($"Value: {x}"))
  .TapError(e => Console.WriteLine($"Error: {e.GetMessage()}"));
```

---

## **8. Recovering from Failures**

```csharp
var safe = Op.Recover(
    Op.Failure<int>(new Error.Validation("bad")),
    fallback: _ => 0
);
```

Async version:

```csharp
var safe = await Op.RecoverAsync(loadUserId(), _ => Task.FromResult(0));
```

---

# **Why OpFlow?**

- One unified abstraction (`Operation<T>`)
- Built‑in validation (short‑circuiting + accumulation)
- Natural async composition
- Parallel workflows with `WhenAll`
- Clean branching with `Match`
- Fluent side effects (`Tap`, `OnSuccess`, `OnFailure`)
- Lightweight, dependency‑free, idiomatic C#
