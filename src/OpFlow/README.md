# **OpFlow**
*A lightweight, expressive, functional result type for C#.*

OpFlow provides a simple, powerful abstraction for representing success or failure without exceptions.  
It is designed for **clean pipelines**, **LINQ composition**, **async workflows**, and **domain‑driven validation**.

If you’ve ever written:

- nested `try/catch`  
- null checks  
- early returns  
- defensive validation  
- async workflows with branching logic  

…OpFlow gives you a cleaner, more expressive way to do it.

---

# **Why OpFlow?**

✔ **No exceptions for control flow**  
✔ **No nulls**  
✔ **No nested awaits**  
✔ **No boilerplate**  
✔ **LINQ support**  
✔ **Async‑first design**  
✔ **Parallel composition**  
✔ **Domain‑friendly validation**  
✔ **Clear error types** (`Validation`, `NotFound`, `Unauthorized`, `Unexpected`)

OpFlow is intentionally small, predictable, and easy to adopt — perfect for both application code and libraries.

---

# **Installation**

```bash
dotnet add package OpFlow
```

---

# **Basic Usage**

## **Success & Failure**

```csharp
var ok = Op.Success(42);
var fail = Op.Failure<int>(new Error.Validation("Invalid input"));
```

---

# **Mapping**

```csharp
var op =
    Op.Success(10)
      .Map(x => x * 2);   // 20
```

---

# **Chaining (Bind)**

```csharp
var op =
    Op.Success(10)
      .Bind(x => Op.Success(x + 5));
```

---

# **LINQ Composition**

```csharp
var op =
    from x in Op.Success(10)
    from y in Op.Success(x + 5)
    select y * 2;
```

---

# **Async Workflows**

```csharp
var op =
    await Op.Success(10)
        .AsTask()
        .BindAsync(async x =>
        {
            await Task.Delay(1);
            return Op.Success(x + 5);
        });
```

---

# **Validation**

```csharp
var op =
    Op.Success(10)
      .Ensure(x => x > 0, x => new Error.Validation("Must be positive"));
```

---

# **Parallel Composition**

```csharp
var combined =
    Op.WhenAll(
        Op.Success(10),
        Op.Success("hello")
    );
```

Async:

```csharp
var combined =
    await Op.WhenAllAsync(
        LoadUser(id),
        LoadOrders(id)
    );
```

---

# **Recover**

```csharp
var safe =
    Op.Failure<int>(new Error.Validation("bad"))
      .Recover(_ => 0);
```

---

# **Pattern Matching**

```csharp
op.Match(
    onSuccess: x => Console.WriteLine($"OK: {x}"),
    onFailure: e => Console.WriteLine($"Error: {e.GetMessage()}")
);
```

---

# **Error Types**

OpFlow ships with four built‑in error cases:

- `Validation`
- `NotFound`
- `Unauthorized`
- `Unexpected`

Each error type carries structured information and can be matched cleanly:

```csharp
error.Match(
    validation: v => Log(v.Message),
    notFound: n => Log("Missing"),
    unauthorized: u => Log("No access"),
    unexpected: x => Log(x.Exception?.Message)
);
```

---

# **Nullable & Exception Helpers**

```csharp
var op1 = Op.FromNullable(user, "User was null");
var op2 = Op.From(() => File.ReadAllText("config.json"));
var op3 = await Op.FromAsync(() => http.GetStringAsync(url));
```

---

# **Philosophy**

OpFlow is built around three principles:

1. **Clarity over cleverness**  
2. **Predictable, explicit control flow**  
3. **Functional ergonomics without ceremony**

It aims to feel natural in C#.
