# **OpFlow Design Philosophy**  

OpFlow is built on a simple idea: **C# developers deserve expressive, composable workflows without adopting a foreign programming paradigm.** The library embraces functional principles where they improve clarity and correctness, but it stays firmly rooted in idiomatic C#.

This philosophy shapes every API, every method name, and every design decision in the code.

---

## **1. One Abstraction, Many Capabilities**  
At the heart of OpFlow is a single type: `Operation<T>`.  
It models:

- success and failure  
- synchronous and asynchronous flows  
- validation with error accumulation  
- branching and matching  

All without requiring separate types like `Result<T>`, `Option<T>`, `Either<L,R>`, or `Task<T>`.

This unification is visible throughout the code:  
`IsSuccess`, `TryGet`, `TryGetError`, `GetMessage`, `AsValidation`, and `IsValidation` all belong to the same abstraction.

**Philosophy:**  
> *A workflow should not require a zoo of types. One abstraction should carry the narrative from start to finish.*

---

## **2. Pipelines Should Read Like Stories**  
OpFlow embraces LINQ and monadic composition to make workflows feel like narratives:

```csharp
var op =
    from a in Op.Success(10)
    from b in Op.Success(5)
    select $"sum={a + b}";
```  
  [github.com](https://github.com/pocinho/OpFlow)

This is not FP cosplay — it’s idiomatic C# that happens to be expressive.

**Philosophy:**  
> *If a workflow is hard to read, it’s hard to trust. Pipelines should be declarative, linear, and intention‑revealing.*

---

## **3. Validation Is a First‑Class Concern**  
Real systems spend enormous effort validating inputs and domain rules.  
OpFlow treats validation as a core capability, not an add‑on.

The code shows two patterns:

### **Simple guards**
```csharp
var op = Op.Success(10)
    .Ensure(x => x > 5, x => new Error.Validation("Too small"));
```  

### **Multi‑rule accumulation**
```csharp
var operation = Op.Success(10)
    .ValidateAll(
        x => x > 0 ? null : new Error.Validation("Must be positive."),
        x => x < 100 ? null : new Error.Validation("Too large.")
    );
```  

**Philosophy:**  
> *Validation should be composable, expressive, and built into the workflow — not bolted on as a separate subsystem.*

---

## **4. Async Should Be Natural, Not Noisy**  
The codebase includes async‑aware combinators like `SelectManyAsync` and `AsTask()`:

```csharp
var op1 = await Op.Success(10)
    .AsTask()
    .SelectManyAsync(async x =>
    {
        await Task.Delay(1);
        return Op.Success(x + 5);
    },
    (x, y) => y);
```

Or better, using BindAsync:

```csharp
var op2 = await Op.Success(10)
    .AsTask()
    .BindAsync(async x =>
    {
        await Task.Delay(1);
        return Op.Success(x + 5);
    });
```

Async is not a wrapper around OpFlow — it’s part of the flow.

**Philosophy:**  
> *Async should not break the pipeline. Asynchronous steps should compose just as naturally as synchronous ones.*

---

## **5. Parallel Composition Should Be Simple and Typed**  
OpFlow includes a clean `WhenAll` combinator:

```csharp
var combined = Op.WhenAll(
    Op.Success(10),
    Op.Success("ok")
);
// combined is Operation<(int, string)>
```  

This gives you structured concurrency without juggling `Task.WhenAll` and manual error handling.

**Philosophy:**  
> *Parallelism should not require ceremony. If two operations can run together, the API should make that obvious and safe.*

---

## **6. Errors Should Be Explicit and Meaningful**  
The library exposes:

- `TryGetError(out Error)`  
- `GetMessage()`  
- `Match(onSuccess, onFailure)`  

These make error handling explicit and intentional.

**Philosophy:**  
> *Errors are part of the domain story. They deserve structure, clarity, and type safety.*

---

## **7. Minimalism Over Machinery**  
OpFlow intentionally avoids:

- typeclasses  
- lenses  
- custom immutable collections  
- complex algebraic hierarchies  
- FP jargon  

The codebase is small, focused, and dependency‑free.

**Philosophy:**  
> *A library should solve real problems with minimal cognitive overhead. Power comes from clarity, not complexity.*

---

## **8. Discoverability Is a Feature**  
Every method name (`Ensure`, `ValidateAll`, `Match`, `AsValidation`, `SelectManyAsync`) is chosen to be intuitive and self‑explanatory.

**Philosophy:**  
> *A developer should be able to explore the API through IntelliSense and understand it without reading a manual.*

---

# **In Summary**

OpFlow’s design philosophy is simple:

**Unify workflows under one expressive abstraction.  
Make pipelines readable.  
Make validation natural.  
Make async and parallelism effortless.  
Stay idiomatic.  
Stay minimal.  
Stay clear.**

Everything in the codebase reflects these principles.  
And that’s what makes OpFlow feel like C# — just better.
