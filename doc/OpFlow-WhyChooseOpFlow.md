# **Why OpFlow?**  

OpFlow is built around a single, powerful abstraction — `Operation<T>` — that unifies success/failure handling, validation, async workflows, and composable pipelines. The code in the repository shows a library that is intentionally small, expressive, and ergonomic, giving you functional‑style clarity without the weight of a full FP framework.

---

## **1. A single type that models success, failure, and validation**  
The core of OpFlow is the `Operation<T>` type, which exposes:

- `IsSuccess` / `IsFailure`  
- `TryGet(out T)`  
- `TryGetError(out Error)`  
- `GetMessage()`  
- `AsValidation()` / `IsValidation`  

This means you don’t need separate types for results, errors, validation, or async wrappers — everything flows through one consistent API.

---

## **2. Pipelines that read like C# — thanks to LINQ and `SelectMany`**  
The repo shows true monadic composition:

```csharp
var op =
    from a in new Operation<int>(10)
    from b in new Operation<int>(5)
    select $"sum={a + b}";
```  

This isn’t a simulation of functional programming — it’s real LINQ‑powered workflow composition, with the ergonomics of idiomatic C#.

---

## **3. Built‑in validation with error accumulation**  
The code demonstrates two validation patterns:

### **Simple guards**
```csharp
var op = new Operation<int>(10)
    .Ensure(x => x > 5, x => new Error("Too small"));
```  

### **Multi‑rule validation**
```csharp
var op = new Operation<int>(10)
    .ValidateAll(
        x => x > 0 ? null : new Error("Must be positive"),
        x => x < 100 ? null : new Error("Too large")
    );
```  

This is a rare feature: validation is not bolted on — it’s part of the core abstraction.

---

## **4. Async composition that feels natural**  
The repo includes async‑aware combinators:

```csharp
var op = await new Operation<int>(10)
    .AsTask()
    .SelectManyAsync(async x => new Operation<int>(x + 5));
```  

Async is not a wrapper around the library — it’s a first‑class part of the pipeline model.

---

## **5. Parallel composition with `WhenAll`**  
The code includes a clean, typed parallel combinator:

```csharp
var combined = WhenAll(
    new Operation<int>(10),
    new Operation<string>("ok")
);
// combined is Operation<(int, string)>
```  

This gives you structured concurrency without juggling `Task.WhenAll` and manual error handling.

---

## **6. Clear branching with `Match`**  
The library exposes a simple, expressive pattern‑matching API:

```csharp
op.Match(
    onSuccess: v => ...,
    onFailure: e => ...
);
```  

This makes the end of a pipeline explicit and readable.

---

## **7. Small, focused, and dependency‑free**  
The codebase is intentionally minimal:

- no typeclasses  
- no custom collections  
- no heavy FP machinery  
- no external dependencies  

Just a clean, composable workflow abstraction.

---

# **In short**  
**OpFlow gives you a unified, ergonomic workflow type (`Operation<T>`) with built‑in validation, async composition, LINQ pipelines, and parallel flows — all visible directly in the code.**  
It’s functional where it matters, idiomatic where it counts, and lightweight everywhere else.
