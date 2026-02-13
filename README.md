# **OpFlow**
*A lightweight, expressive, functional result type for C#.*

OpFlow is a small, powerful library for representing success or failure without exceptions.  
It brings functional composition, LINQ support, async workflows, and domain‑driven validation into a clean, idiomatic C# API.

If you’ve ever written nested `try/catch`, null checks, or defensive validation, OpFlow gives you a clearer, more expressive alternative.

---

## **✨ Features**

- **Success/Failure result type** with structured error cases  
- **LINQ support** for clean, declarative pipelines  
- **Async‑first design** (`BindAsync`, `SelectManyAsync`, `EnsureAsync`, etc.)  
- **Parallel composition** (`WhenAll`, `WhenAllAsync`)  
- **Domain‑friendly validation** (`Ensure`, `Require`, `Validate`, `ValidateAll`)  
- **Side‑effect helpers** (`Tap`, `OnSuccess`, `OnFailure`)  
- **Error pattern matching**  
- **Safe wrappers** for exceptions, tasks, and nullable values  

OpFlow is intentionally minimal, predictable, and easy to adopt.

---

## **📦 Installation**

```bash
dotnet add package OpFlow
```

---

## **🚀 Quick Start**

### **Success & Failure**

```csharp
var ok = Op.Success(42);
var fail = Op.Failure<int>(new Error.Validation("Invalid input"));
```

---

### **Mapping**

```csharp
var op =
    Op.Success(10)
      .Map(x => x * 2);   // 20
```

---

### **Chaining (Bind)**

```csharp
var op =
    Op.Success(10)
      .Bind(x => Op.Success(x + 5));
```

---

### **LINQ Composition**

```csharp
var op =
    from x in Op.Success(10)
    from y in Op.Success(x + 5)
    select y * 2;
```

---

### **Async Workflows**

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

### **Validation**

```csharp
var op =
    Op.Success(10)
      .Ensure(x => x > 0, x => new Error.Validation("Must be positive"));
```

---

### **Parallel Composition**

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

### **Recover**

```csharp
var safe =
    Op.Failure<int>(new Error.Validation("bad"))
      .Recover(_ => 0);
```

---

### **Pattern Matching**

```csharp
op.Match(
    onSuccess: x => Console.WriteLine($"OK: {x}"),
    onFailure: e => Console.WriteLine($"Error: {e.GetMessage()}")
);
```

---

## **🧩 Error Types**

OpFlow ships with four built‑in error cases:

- `Validation`
- `NotFound`
- `Unauthorized`
- `Unexpected`

Each error type carries structured information and supports pattern matching:

```csharp
error.Match(
    validation: v => Log(v.Message),
    notFound: n => Log("Missing"),
    unauthorized: u => Log("No access"),
    unexpected: x => Log(x.Exception?.Message)
);
```

---

## **🧭 Philosophy**

OpFlow is built around three principles:

1. **Clarity over cleverness**  
2. **Predictable, explicit control flow**  
3. **Functional ergonomics without ceremony**

It aims to feel natural in C#, for real‑world codebases where correctness, readability, and maintainability matter.

---

## **📚 Documentation**

- Quickstart examples  
- API reference  
- Validation patterns  
- LINQ composition guide  
- Error handling patterns  

See the [doc folder in the repository](https://github.com/pocinho/OpFlow/tree/main/doc).

---

## 📄 License

MIT License

Copyright (c) 2026 Paulo Pocinho.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
