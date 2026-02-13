# **OpFlow LINQ Cheatsheet**
*A practical guide to using LINQ query syntax with `Operation<T>`*

OpFlow fully supports LINQ query expressions through:

- `Select` → map  
- `SelectMany` → bind  
- `Where` → filter  

This gives you a fluent, expressive way to compose workflows.

---

# **1. Basic LINQ Composition**

```csharp
var op =
    from x in Op.Success(10)
    select x * 2;
```

Equivalent to:

```csharp
Op.Success(10).Select(x => x * 2);
```

---

# **2. Chaining Multiple Operations**

```csharp
var op =
    from a in Op.Success(10)
    from b in Op.Success(5)
    select a + b;
```

Equivalent to:

```csharp
Op.Success(10)
  .SelectMany(a => Op.Success(5),
              (a, b) => a + b);
```

---

# **3. Async LINQ Composition**

```csharp
var op =
    await (
        from id in loadUserId().AsTask()
        from profile in loadProfile(id)
        select new { id, profile }
    );
```

This uses `SelectManyAsync` under the hood.

---

# **4. Mixing Sync + Async**

```csharp
var op =
    await (
        from x in Op.Success(10).AsTask()
        from y in Task.FromResult(Op.Success(x + 5))
        select y
    );
```

---

# **5. Using Where (Filtering)**

```csharp
var op =
    from x in Op.Success(10)
    where x > 5
    select x;
```

Equivalent to:

```csharp
Op.Success(10).Where(x => x > 5);
```

If the predicate fails, OpFlow returns:

```
Error.Validation("Predicate failed")
```

---

# **6. Combining LINQ with Validation**

```csharp
var op =
    from x in Op.Success(10)
    where x != 10
    select x;
```

Fails with:

```
Error.Validation("Predicate failed")
```

---

# **7. Using Let (Intermediate Values)**

```csharp
var op =
    from x in Op.Success(10)
    let y = x * 2
    select y + 1;
```

Equivalent to:

```csharp
Op.Success(10).Select(x => {
    var y = x * 2;
    return y + 1;
});
```

---

# **8. Using Into (Continuation)**

```csharp
var op =
    from x in Op.Success(10)
    select x * 2
    into doubled
    from y in Op.Success(doubled + 1)
    select y;
```

---

# **9. Full Example: Async + Validation + Projection**

```csharp
var op =
    await (
        from id in loadUserId().AsTask()
        where id > 0
        from profile in loadProfile(id)
        select new UserDto(id, profile)
    );
```

---

# **10. When to Use LINQ vs Bind/Map**

| Goal | LINQ | Bind/Map |
|------|------|----------|
| Simple transformations | ✔ | ✔ |
| Multi-step workflows | ✔ | ✔ |
| Async composition | ✔ | ✔ |
| Combining values `(x, y)` | ✔ | ✔ |
| Most readable pipelines | ✔ | — |
| Fine-grained control | — | ✔ |

If you want the cleanest expression of intent, LINQ is usually the winner.

---

# **11. Common Pitfalls**

### ❌ Missing `project` argument in `SelectManyAsync`
Fix: use LINQ or supply both parameters.

### ❌ Forgetting `.AsTask()` for async LINQ
LINQ requires `Task<Operation<T>>`, not `Operation<T>`.

### ❌ Returning `Operation<T>` instead of `Task<Operation<T>>` in async binds
Wrap with `Task.FromResult(...)` or use `async`.

---

# **12. Quick Reference Table**

| LINQ keyword | OpFlow method | Meaning |
|--------------|---------------|---------|
| `select` | `Select` | Transform success value |
| `from` | `SelectMany` | Bind / chain operations |
| `where` | `Where` | Filter with validation |
| `let` | `Select` | Introduce intermediate value |
| `into` | `SelectMany` | Continue query |

---

# **13. Idiomatic OpFlow LINQ Pipeline**

```csharp
var result =
    await (
        from userId in loadUserId().AsTask()
        where userId > 0
        from profile in loadProfile(userId)
        from settings in loadSettings(userId)
        select new UserSummary(userId, profile, settings)
    );
```

This is the kind of expressive, narrative pipeline OpFlow was born for.
