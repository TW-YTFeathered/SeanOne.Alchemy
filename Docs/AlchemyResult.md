# AlchemyResult

`AlchemyResult` is the wrapper returned by `Alchemy.Transform`. It holds a transformed object and exposes methods for extraction, chaining, and asynchronous helpers.

> 💡 For the common extraction patterns (`ToObject<T>`, `ToList<T>`, `GetXxx()`, `GetXxxList()`), see [Getting Started → Extracting Results](GettingStarted.md#extracting-results) first. This page covers the rest of the API.

## Extracting the Wrapped Object

### `ToObject<T>()`

Returns the wrapped object directly as type `T`. No conversion is performed — this is a plain cast.

```csharp
using SeanOne.Alchemy;

var sorted = Alchemy.Transform(numbers, "arr /sort:is").ToObject<List<int>>();
```

Throws `InvalidCastException` if the wrapped object is not assignable to `T`.

### `ToList<T>()`

Returns the wrapped object as a `List<T>`. Works only when the wrapped object **exactly** implements `IEnumerable<T>`.

```csharp
using SeanOne.Alchemy;

var list = Alchemy.Transform(numbers, "arr /sort:is").ToList<int>();
```

If you need per-element conversion (e.g., `ArrayList` → `List<int>`), use the `GetXxxList()` extension methods instead.

Throws `InvalidOperationException` when the wrapped object does not implement `IEnumerable<T>`.

### `ToString()`

Returns a string representation of the wrapped object.

- If the wrapped object is a `string`, that string is returned directly.
- Otherwise, the base `object.ToString()` is used — which typically yields the **type name**, not a rendered value.

> ⚠️ `ToString()` does **not** call the wrapped object's own `ToString()`. For that, use the `GetString()` extension method.

### `ToStringAsync()`

Asynchronous variant of `ToString()`. Simply offloads to `Task.Run`; there is no true async I/O.

```csharp
using SeanOne.Alchemy;

string s = await result.ToStringAsync();
```

## Chaining Transformations

An `AlchemyResult` can be transformed again without unwrapping.

### `Transform(string)`

Applies a single DSL instruction to the wrapped object.

```csharp
using SeanOne.Alchemy;

var result = Alchemy.Transform(25, "cnv /temp:C->F")
                    .Transform("/tostring:F1 /suffix:\" °F\"");
```

### `Transform(params string[])`

Applies multiple DSL instructions sequentially.

```csharp
using SeanOne.Alchemy;

var result = Alchemy.Transform(data, "arr /sort:is")
                    .Transform("cnv /temp:F->C", "/tostring:F2");
```

### `TransformAsync(...)`

Asynchronous variants of the above. Both overloads exist.

```csharp
using SeanOne.Alchemy;

var result = await Alchemy.TransformAsync(data, "arr /sort:is");
var final  = await result.TransformAsync("cnv /temp:F->C");
```

## Asynchronous Helpers (`AlchemyResultTaskExtensions`)

Extension methods for `Task<AlchemyResult>` — useful when you want to keep a transformation chain in async style without unwrapping between steps.

| Method | Equivalent to |
|--------|---------------|
| `TransformAsync(this Task<AlchemyResult>, string)` | `await task` → `.TransformAsync(instruction)` |
| `ToListAsync<T>()` | `await task` → `.ToList<T>()` |
| `ToObjectAsync<T>()` | `await task` → `.ToObject<T>()` |
| `ToStringAsync()` | `await task` → `.ToStringAsync()` |

```csharp
using SeanOne.Alchemy;

var final = await Alchemy.TransformAsync(data, "arr /sort:is")
                         .TransformAsync("cnv /temp:F->C");

var values = await Alchemy.TransformAsync(data, "arr /sort:is")
                          .ToListAsync<double>();
```

## Construction (Internal-Oriented)

> ⚠️ The members below are **publicly accessible** but are intended primarily for internal use by the library. Their signature and behavior may change in future versions without notice. Prefer the `Alchemy.Transform` / `Alchemy.Format` entry points instead of calling `Parse` / `TryParse` directly.

### `Parse(object sourceObj)`

Wraps `sourceObj` in a new `AlchemyResult`. Throws `ArgumentNullException` if `sourceObj` is `null`.

```csharp
using SeanOne.Alchemy;

var result = AlchemyResult.Parse(someObject);
```

### `TryParse(object sourceObj, out AlchemyResult result)`

Non-throwing variant. Returns `false` and sets `result` to `null` when `sourceObj` is `null`; otherwise returns `true`.

```csharp
using SeanOne.Alchemy;

if (AlchemyResult.TryParse(someObject, out var result))
{
    // result is usable
}
```

### `RawSource`

Gets the unwrapped source object.

```csharp
using SeanOne.Alchemy;

object raw = result.RawSource;
```

> ⚠️ Internal-oriented. Prefer `ToObject<T>()`, `ToList<T>()`, or the `GetXxx()` extension methods in application code. `RawSource` exists mainly so that extension methods (e.g., `AlchemyConverterExpansions`) and the library itself can reach the underlying object without casting.

## See Also

- [Getting Started → Extracting Results](GettingStarted.md#extracting-results) — `ToObject<T>`, `ToList<T>`, `GetXxx()`, `GetXxxList()`
- [Error Handling](ErrorHandling.md#alchemyresult-exceptions) — exceptions thrown by extraction methods
- [FAQ](FAQ.md#what-does-alchemyresulttostring-return) — `ToString()` behavior details
