# Example: Using OpFlow with DTOs

## 1. Define your DTOs

```csharp
public sealed record CreateUserRequest(
    string Email,
    string Name,
    int Age
);

public sealed record User(
    Guid Id,
    string Email,
    string Name,
    int Age
);

public sealed record CreateUserResponse(
    Guid Id,
    string Message
);
```

---

## 2. Define your domain operations

These are plain methods returning `Operation<T>` or `Task<Operation<T>>`.

```csharp
using static OpFlow.Op;

public static class UserOps
{
    public static Operation<CreateUserRequest> Validate(CreateUserRequest dto) =>
        Success(dto)
            .Ensure(x => !string.IsNullOrWhiteSpace(x.Email), new Error.Validation("Email is required"))
            .Ensure(x => x.Email.Contains("@"), new Error.Validation("Email must be valid"))
            .Ensure(x => !string.IsNullOrWhiteSpace(x.Name), new Error.Validation("Name is required"))
            .Ensure(x => x.Age >= 18, new Error.Validation("User must be an adult"));

    public static Operation<User> ToDomain(CreateUserRequest dto) =>
        Success(new User(
            Id: Guid.NewGuid(),
            Email: dto.Email.Trim(),
            Name: dto.Name.Trim(),
            Age: dto.Age
        ));

    public static async Task<Operation<User>> SaveAsync(User user)
    {
        await Task.Delay(1); // substitute with DB write
        return Success(user);
    }

    public static Operation<CreateUserResponse> ToResponse(User user) =>
        Success(new CreateUserResponse(
            Id: user.Id,
            Message: $"User {user.Name} created successfully"
        ));
}
```

---

## 3. Compose everything with OpFlow

This is where OpFlow shines: expressive, linear, and safe.

```csharp
public static async Task<Operation<CreateUserResponse>> CreateUserAsync(CreateUserRequest request)
{
    return await UserOps
        .Validate(request)              // Operation<CreateUserRequest>
        .Bind(UserOps.ToDomain)         // Operation<User>
        .BindAsync(UserOps.SaveAsync)   // Task<Operation<User>>
        .BindAsync(UserOps.ToResponse); // Operation<CreateUserResponse>
}
```

---

## 4. Consume the pipeline with `Match`

```csharp
[HttpPost]
[Route("CreateUser")]
public async Task<IResult> CreateUser(CreateUserRequest request, CancellationToken cancellationToken)
{
    try
    {
        Operation<CreateUserResponse> result = await UserOps.CreateUserAsync(request);

        return result.Match<CreateUserResponse, IResult>(
            ok => TypedResults.Ok(ok),
            err => TypedResults.BadRequest(new { error = err.GetMessage() }));

    }
    catch (Exception)
    {
        return TypedResults.InternalServerError("server error");
    }
}
```
