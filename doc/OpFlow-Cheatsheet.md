# **OpFlow Cheatsheet**
*A quick guide to the core features of OpFlow, with minimal examples.*

---

## **1. Creating Operations**

```csharp
var ok = Op.Success(42);
var fail = Op.Failure<int>(new Error.Validation("Invalid"));
var from = Op.From(() => File.ReadAllText("config.json"));
var asyncFrom = await Op.FromAsync(() => http.GetStringAsync(url));
var nullable = Op.FromNullable(user, "User was null");
```

---

## **2. Mapping (Select / Map)**

```csharp
var op =
    Op.Success(10)
      .Map(x => x * 2);   // 20
```

LINQ version:

```csharp
var op =
    from x in Op.Success(10)
    select x * 2;
```

---

## **3. Chaining (Bind / SelectMany)**

```csharp
var op =
    Op.Success(10)
      .Bind(x => Op.Success(x + 5));
```

LINQ version:

```csharp
var op =
    from x in Op.Success(10)
    from y in Op.Success(x + 5)
    select y;
```

---

## **4. Async Composition**

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

## **5. Validation**

### **Ensure**

```csharp
var op =
    Op.Success(10)
      .Ensure(x => x > 0, x => new Error.Validation("Must be positive"));
```

### **Validate (short‑circuit)**

```csharp
var op =
    Op.Validate(
        Op.Success(10),
        x => x > 0 ? null : new Error.Validation("Too small"),
        x => x < 20 ? null : new Error.Validation("Too big")
    );
```

### **ValidateAll (accumulate)**

```csharp
var op =
    Op.ValidateAll(
        Op.Success(10),
        x => x == 10 ? new Error.Validation("Must not be 10") : null,
        x => x > 9 ? new Error.Validation("Too large") : null
    );
```

---

## **6. Filtering (Where)**

```csharp
var op =
    from x in Op.Success(10)
    where x > 5
    select x;
```

---

## **7. Parallel Composition**

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

## **8. Side Effects (Tap / OnSuccess / OnFailure)**

```csharp
Op.Success(10)
  .Tap(x => Console.WriteLine($"Value: {x}"))
  .TapError(e => Console.WriteLine($"Error: {e.GetMessage()}"));
```

---

## **9. Recover**

```csharp
var safe =
    Op.Failure<int>(new Error.Validation("bad"))
      .Recover(_ => 0);   // fallback
```

---

## **10. Branching (Match)**

```csharp
op.Match(
    onSuccess: x => Console.WriteLine($"OK: {x}"),
    onFailure: e => Console.WriteLine($"Error: {e.GetMessage()}")
);
```

Async:

```csharp
await op.MatchAsync(
    onSuccess: async x => await LogAsync(x),
    onFailure: async e => await LogErrorAsync(e)
);
```

---

## **11. LINQ + Async Example**

```csharp
var op =
    await (
        from id in LoadUserId().AsTask()
        from profile in LoadProfile(id)
        select new { id, profile }
    );
```

---

## **12. Error Helpers**

```csharp
error.Match(
    validation: v => Console.WriteLine(v.Message),
    notFound: n => Console.WriteLine("Missing"),
    unauthorized: u => Console.WriteLine("No access"),
    unexpected: x => Console.WriteLine(x.Exception?.Message)
);
```
