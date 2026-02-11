# OpFlow.Converters.NewtonsoftJson

**Newtonsoft.Json converters for OpFlow discriminated unions.**

This package provides high‑quality, contract‑based JSON converters for the OpFlow union types:

- `Operation<T>`
- `Error`

It ensures stable, predictable JSON serialization and deserialization for all OpFlow union cases, without relying on reflection for contract discovery. The converters enforce OpFlow’s invariants and guarantee round‑trip safety.

---

## ✨ Features

- Full support for `Operation<T>`:
  - `Success`
  - `Failure`

- Full support for all `Error` union cases:
  - `Validation`
  - `NotFound`
  - `Unauthorized`
  - `Unexpected`

- Contract‑based JSON schema (no hidden fields, no magic)
- Non‑generic `OpFlowOperationConverter` works for **any** `Operation<T>`
- Structured exception serialization for `Unexpected` errors
- Case‑insensitive JSON parsing
- Clear error messages for malformed payloads
- Zero reflection for property access
- Minimal reflection only for generic type construction (safe and unavoidable)

---

## 📦 Installation

Install via NuGet:

```bash
dotnet add package OpFlow.Converters.NewtonsoftJson
```

Or via the NuGet Package Manager:

```
Install-Package OpFlow.Converters.NewtonsoftJson
```

---

## 🚀 Getting Started

### Register the converters

You can register the converters manually:

```csharp
var settings = new JsonSerializerSettings
{
    Converters =
    {
        new OpFlowErrorConverter(),
        new OpFlowOperationConverter()
    }
};
```

Or use the convenience extension method:

```csharp
var settings = new JsonSerializerSettings()
    .AddOpFlowConverters();
```

---

## 🔄 Usage Examples

### Serialize a successful operation

```csharp
var op = new Operation<string>.Success("Hello world");

string json = JsonConvert.SerializeObject(op, settings);
```

Produces:

```json
{
  "operation": {
    "kind": "success",
    "result": "Hello world"
  }
}
```

### Serialize a failed operation

```csharp
var op = new Operation<string>.Failure(
    new Error.NotFound("User not found")
);

string json = JsonConvert.SerializeObject(op, settings);
```

Produces:

```json
{
  "operation": {
    "kind": "failure",
    "error": {
      "errorType": "notfound",
      "message": "User not found"
    }
  }
}
```

### Deserialize

```csharp
Operation<string> result =
    JsonConvert.DeserializeObject<Operation<string>>(json, settings);
```

Pattern‑match the result:

```csharp
switch (result)
{
    case Operation<string>.Success s:
        Console.WriteLine($"Success: {s.Result}");
        break;

    case Operation<string>.Failure f:
        Console.WriteLine($"Error: {f.Error}");
        break;
}
```

---

## 📐 JSON Contract

### Operation<T>

```json
{
  "operation": {
    "kind": "success" | "failure",
    "result": <T>,          // success only
    "error": <ErrorObject>  // failure only
  }
}
```

### Error

Each error case includes a discriminator:

```json
{
  "error": {
    "errorType": "validation" | "notfound" | "unauthorized" | "unexpected",
    "message": "string",
    "fields": [ "string" ],     // validation only
    "exception": { ... }        // unexpected only
  }
}
```

---

## 🧪 Testing

This package is fully covered by a dedicated test suite:

- Round‑trip serialization for all union cases
- Error handling for malformed JSON
- Case‑insensitive parsing
- Nested error serialization
- Multiple generic types (`Operation<string>`, `Operation<int>`, etc.)

---

## 📄 License

MIT License — see the `LICENSE` file for details.

---

## 🤝 Contributing

Contributions are welcome.  
If you’d like to improve the converters, add new union support, or enhance the JSON contract, feel free to open an issue or submit a pull request.

---

## 📚 Related Packages

- **OpFlow** — the core discriminated union types (`Error`, `Operation<T>`)
- **OpFlow.Converters.SystemTextJson** *(coming soon)* — STJ converters for OpFlow unions

---

## ❤️ About OpFlow

OpFlow is a narrative‑driven pipeline and operation framework designed for clarity, expressiveness, and developer experience.  
These converters ensure that OpFlow’s union types serialize cleanly and predictably across service boundaries.
