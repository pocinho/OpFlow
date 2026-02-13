# 🧭 How OpFlow compares to existing functional libraries for C#

Below is a structured comparison across the libraries that developers typically reach for when they want FP‑style ergonomics in C#.

---

# 1. **OpFlow vs. LanguageExt**

| Aspect | OpFlow | LanguageExt |
|-------|--------|-------------|
| **Core abstraction** | `Operation<T>` with success/failure, validation mode, async variants, and pipeline ergonomics. | Rich algebraic types: `Option`, `Either`, `Try`, `Validation`, `Aff`, `Eff`, etc. |
| **Design philosophy** | Lightweight, pipeline‑first, ergonomic, minimal ceremony. | Full FP ecosystem: monads, typeclasses, immutable collections, lenses. |
| **Error handling** | Unified success/failure with `Ensure`, `ValidateAll`, `Match`, async `SelectManyAsync`. (  [OpFlow](https://github.com/pocinho/OpFlow)) | Many monads for different error semantics; more powerful but more complex. |
| **Learning curve** | Very low — feels like idiomatic C#. | High — requires FP mental model. |
| **Use cases** | Pipelines, domain operations, expressive error flows, async composition. | FP‑heavy applications, domain modeling, pure functional workflows. |

**Summary:**  
LanguageExt is a full FP universe; OpFlow is a focused, ergonomic pipeline engine. If LanguageExt is “Haskell‑in‑C#,” OpFlow is “C# with the friction removed.”

---

# 2. **OpFlow vs. OneOf / Discriminated Unions libraries**

| Aspect | OpFlow | OneOf / DU libs |
|--------|--------|------------------|
| **Primary goal** | Pipelines + error handling + async + validation. | Representing sum types. |
| **Composition** | LINQ query syntax, `SelectMany`, async flows, `WhenAll`. (  [OpFlow](https://github.com/pocinho/OpFlow)) | No pipeline semantics; just type‑safe unions. |
| **Error modeling** | Built‑in success/failure semantics. | You must model errors manually. |
| **Validation** | Built‑in validation combinators. | Not provided. |

**Summary:**  
OpFlow is far more expressive for workflows; DU libraries are building blocks, not pipelines.

---

# 3. **OpFlow vs. FluentResults / Result<T> libraries**

| Aspect | OpFlow | FluentResults / Result<T> |
|--------|--------|----------------------------|
| **Abstraction** | `Operation<T>` with LINQ, async, validation, and combinators. | `Result<T>` with success/failure and messages. |
| **Composition** | True monadic composition (`Select`, `SelectMany`, async). | Composition exists but is not LINQ‑native or pipeline‑centric. |
| **Validation** | `Ensure`, `ValidateAll`, `AsValidation`. (  [OpFlow](https://github.com/pocinho/OpFlow)) | Basic error accumulation; not pipeline‑first. |
| **Async** | First‑class async monadic flows. | Async support but not deeply integrated. |

**Summary:**  
FluentResults is a structured error container; OpFlow is a workflow engine with error semantics built in.

---

# 4. **OpFlow vs. Mediator‑style libraries (MediatR, etc.)**

| Aspect | OpFlow | MediatR |
|--------|--------|---------|
| **Purpose** | Functional pipelines and domain operations. | Decoupled request/response messaging. |
| **Error handling** | Built‑in. | Must be modeled manually. |
| **Composition** | LINQ pipelines, async chaining. | No pipeline semantics. |

**Summary:**  
MediatR is architectural plumbing; OpFlow is domain logic composition.

---

# 5. **OpFlow vs. C# built‑in Task / ValueTask pipelines**

| Aspect | OpFlow | Raw Tasks |
|--------|--------|-----------|
| **Error semantics** | Explicit, typed, composable. | Exceptions or manual error types. |
| **Composition** | Declarative, LINQ‑friendly. | Imperative, exception‑driven. |
| **Validation** | Built‑in. | Manual. |

**Summary:**  
OpFlow gives structure and clarity where Tasks give flexibility but no semantics.

---

# 🧩 What makes OpFlow distinct?

Based on the content visible in your repo tab, OpFlow’s differentiators are:

### **1. A unified abstraction (`Operation<T>`) for success, failure, validation, and async**
You’re not juggling `Result<T>`, `Option<T>`, `Either<L,R>`, `Task<T>`, etc.  
Everything flows through one consistent type.

### **2. First‑class pipeline ergonomics**
The unit test examples show:

- LINQ query syntax  
- `SelectMany` and `SelectManyAsync`  
- `WhenAll` for parallel composition  
- `Ensure` and `ValidateAll` for domain validation  
- `Match` for final branching  

This is a coherent, narrative‑driven workflow model. ([OpFlow](https://github.com/pocinho/OpFlow))

### **3. Validation mode as a first‑class citizen**
`AsValidation`, `IsValidation`, and error accumulation are built into the same abstraction — something most libraries bolt on.

### **4. Async is not an afterthought**
`SelectManyAsync` and async `Operation<T>` creation are core features, not wrappers.

### **5. Ergonomics over purity**
OpFlow embraces idiomatic C#:

- No typeclasses  
- No Haskell‑style purity constraints  
- No heavy FP jargon  

It’s functional‑inspired, not functional‑dogmatic.

---

# 🧠 The bottom line

**OpFlow sits in a sweet spot between “full FP ecosystem” and “simple Result<T> container.”**  
It gives you:

- Pipelines  
- Async composition  
- Validation  
- Error modeling  
- Discriminated‑union‑like behavior  

…all through one ergonomic, discoverable API.

It’s not trying to be LanguageExt; it’s trying to make everyday domain workflows elegant, expressive, and predictable.
