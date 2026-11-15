# Error Handling

This document lists common exceptions thrown by `Alchemy.Format`, `Alchemy.Transform`, `AlchemyResult`, and the Fluent API builders, and how to resolve them.  
**Note: This document covers exceptions for `Alchemy.Format`, `Alchemy.Transform`, `AlchemyResult`, and `AlchemyBuilder` / `AlchemyExecutable`.**

## `Alchemy.Format` Exceptions

`Alchemy.Format` handles three categories of operations: **formatting** (`basic`/`fe` as a fallback). Exceptions are thrown immediately when validation fails.

### ArgumentNullException

| Message / Scenario | What it means | How to fix |
|-------------------|---------------|-------------|
| `Input object must not be null.` | You passed `null` as the object to format. | Make sure the object you are formatting is not `null`. |
| `Alchemy instruction cannot be null or empty` | The DSL instruction string is `null` or empty. | Provide a valid instruction, e.g., `"/tostring:F2"`. |
| `Target object cannot be null for 'fe' directive` (or `'foreach'`) | Inside `fe` / `foreach`, the input object is `null`. | Check that your collection is not `null` before formatting. |
| `'dict-format' parameter is required when processing dictionaries.` | You used `fe` on a dictionary but omitted `/dict-format`. | Add `/dict-format:{0}→{1}` (or any format with `{0}` for key, `{1}` for value). |

### ArgumentException

| Message / Scenario | What it means | How to fix |
|--------------------|---------------|-------------|
| `String is not supported for 'fe' directive` (or `'foreach'`) | You tried to format a `string` with `fe`. | Use `basic` instead, or wrap the string in a collection like `new[] { myString }`. |
| `Object must implement IEnumerable for 'fe' directive` | You used `fe` on a non‑collection object. | Use `basic` for single objects. If you intended a collection, ensure it implements `IEnumerable`. |
| `Invalid parameters for basic processing: invalid, unknown` | The DSL instruction contains parameter(s) not allowed for `basic`. | Check the list of allowed parameters for `basic` (`tostring`, `prefix`, `suffix`, `begin`, `end`). Remove unsupported ones. |
| `Invalid parameters for enumerable processing: dict-format` | The DSL contains parameters that are not allowed for sequences (e.g., `/dict-format` used on a list). | Only use dictionary‑specific parameters (`dict-format`, `key-format`, `value-format`) when the input is actually an `IDictionary`. |
| `Invalid parameters for dictionary processing: sort` | The DSL contains parameters not allowed for dictionaries (e.g., `/sort` is not valid for dictionaries). | Use valid dictionary parameters: `dict-format`, `key-format`, `value-format`, `begin`, `end`, `prefix`, `suffix`, `exclude-last-end` (and optionally `fe-opt`). |
| `Collection elements must implement IFormattable for 'tostring'. Found: {TypeName}` | You used `/tostring` but one of the elements (or the object itself in `basic`) does not implement `IFormattable`. Common example: trying to format a `string` with `/tostring:F2`. | Remove `/tostring` if you don't need custom formatting. If you need numeric formatting, ensure the collection contains numeric types (like `int`, `double`, `DateTime`, etc.), not `string`. |
| `Parameter '/xxx:' is specified multiple times.` | The same parameter (e.g., `/tostring`) appears more than once in the same instruction. | Use each parameter only once. Combine or remove duplicates. |

### MissingMethodException

| Message / Scenario | What it means | How to fix |
|--------------------|---------------|-------------|
| `Unknown functions directive: {directive}` | The first word of your instruction is not `basic`, `fe`, `foreach`, and does not start with `/`. For example, `loop /tostring:F2`. | Use a valid function name: `basic`, `fe`, or `foreach`. Or if you omit the function name, start directly with a parameter (e.g., `/tostring:F2`) which defaults to `basic`. |

## `Alchemy.Transform` Exceptions

`Alchemy.Transform` handles three categories of operations: **unit conversion** (`cnv`), **sorting** (`arr`), and **formatting** (`basic`/`fe` as a fallback). Exceptions are thrown immediately when validation fails.

### Root-Level Validation

| Exception | Scenario |
|-----------|----------|
| `ArgumentNullException` | The input object is `null`, or the DSL instruction string is `null` or empty. |
| `ArgumentException` (parameter validation) | The DSL instruction contains parameters not allowed for the requested operation. The error message will indicate which operation (`arr`, `cnv`, `enumerable`, `dictionary`) and list the invalid parameters (e.g., `Invalid parameters for arr processing: sortx, unknown`). |

### Unit Conversion (`/weight`, `/length`, `/temp`)

| Exception | Common Scenarios |
|-----------|------------------|
| `ArgumentException` | Unknown unit code (e.g., `/temp:X->Y`). |
| `ArgumentException` | Negative value for length or weight. |
| `ArgumentException` | Temperature below absolute zero (0 K). |
| `ArgumentException` | Same source and target unit (e.g., `/temp:F->F`, `/temp:FtoF`, `/length:M->M`, `/length:MtoM`, `/weight:Kg->Kg`, or `/weight:KgtoKg`). These pairs are not registered; same-unit conversion is not an identity conversion. |
| `InvalidOperationException` | A collection element is `null` and cannot be converted. |
| `InvalidOperationException` | An element cannot be cast to a numeric type (`double`). |
| `InvalidOperationException` | The conversion result overflows the target element type (e.g., converting a huge `double` to `int`). |
| `OverflowException` | A single-value conversion result exceeds the range of the input type (e.g., `Transform(1e20, "cnv /temp:C->F")` on an `int`). Collections wrap this as `InvalidOperationException` instead. |

> 💡 Same-unit instructions are **not** identity conversions. The API reports `Unknown conversion instruction: ...`; omit the conversion parameter (`/temp`, `/length`, or `/weight`) to leave values unchanged. The typed Fluent helpers (`WithTemp`, `WithLength`, `WithWeight`) skip equal pairs and emit no parameter, so they do not raise this exception. See [Temperature](Transform/Temperature.md#same-unit-conversions), [Length](Transform/Length.md#same-unit-conversions), [Weight](Transform/Weight.md#same-unit-conversions), and the [Fluent API](FluentAPI.md#unit-conversions).

### Sorting (`/sort`)

| Exception | Scenario |
|-----------|----------|
| `ArgumentException` | The input object does not implement `IList`. |
| `ArgumentException` | An unknown sorting algorithm key is used (e.g., `/sort:bubble`). |
| `ArgumentException` | Insertion sort (`/sort:is…`) hit elements that do not implement `IComparable` (`At least one object must implement IComparable.`). |
| `InvalidOperationException` | `Array.Sort` / LINQ sort (`/sort:as…`, `/sort:ls…`) hit elements that cannot be compared (`Failed to compare two elements in the array.`). |
| `NotSupportedException` | The collection rejects in-place writes (`Collection is read-only.`), e.g. sorting a `ReadOnlyCollection<T>`. The original collection is still left untouched. |

### Formatting as a Fallback

When `Transform` receives a formatting instruction (`basic`, `fe`, `foreach`, or an instruction starting with `/`), it internally routes the call to `Alchemy.Format`. In these cases, **all exceptions listed in the [Alchemy.Format Errors](#alchemyformat-errors) section** apply identically (e.g., invalid parameters, unsupported `/tostring` on strings, etc.).

## Fluent API Builder Exceptions

These exceptions are raised by the builder state machine before or alongside the underlying DSL execution.

### InvalidOperationException

| Message / Scenario | What it means | How to fix |
|-------------------|---------------|------------|
| `Cannot add parameters after the builder stage has been completed.` | A `With...` method was called after the builder stage was finalized by `Build()`, `Execute()`, `ToString()`, `Step()`, or `ThenXxx()`. | Add every parameter before finalizing the stage, or start a new stage with `ThenXxx()` or `AlchemyBuilder.SelectXxx()`. |

### ArgumentNullException

| Message / Scenario | What it means | How to fix |
|-------------------|---------------|------------|
| `Alchemy instruction cannot be null or empty` | An `AlchemyExecutable` contains no instructions and `Execute()` was called. This occurs when no builder stage emitted an instruction, or when the only configured conversion used equal source/target units and the typed helper emitted no parameter. | Emit at least one parameter, or avoid executing an empty pipeline. For conversion, use different source and target units. |

> 💡 Builder methods such as `With...` do not validate all DSL semantics. Missing `dict-format`, invalid sort keys, and invalid unit pairs are reported when `Execute()` calls `Alchemy.Format` or `Alchemy.Transform`; see the corresponding sections above. Raw same-unit instructions such as `F->F` throw `ArgumentException`; the typed Fluent conversion helpers skip equal units and emit no parameter. See [Fluent API](FluentAPI.md).

## `AlchemyResult` Exceptions

When extracting data from an `AlchemyResult`, some methods have strict type requirements that can throw exceptions if not met.

> ℹ️ The whole `GetXxx()` / `GetXxxList()` family reports failures as `InvalidCastException`, including parse failures. `InvalidOperationException` is only raised by `ToList<T>()`.

### InvalidCastException

| Scenario | What it means | How to fix |
|----------|---------------|------------|
| `Unable to cast object of type '{ActualType}' to type '{TargetType}'.` | You called `ToObject<T>()` on a wrapped object that is not actually of type `T` (or a derived type). For example, trying `ToObject<int>()` when the wrapped object is a `string`. | Use the correct type for `T`, or use a conversion method (e.g., `GetInt32()`, `GetDouble()`) if you want to parse the value. |
| `Cannot convert {TypeName} to any enumerable type.` | You called a `GetXxxList()` method on a wrapped object that is not an `IEnumerable` (e.g., a single `int`). | Use a single-value conversion method instead, or ensure the object is a collection before calling `GetXxxList()`. |
| `Expected IEnumerable, but got {TypeName}` | You called `GetObjectList()` on a wrapped object that is not an `IEnumerable`. | Use `ToObject<T>()`/`ToList<T>()` for single values or typed collections, or call `GetObjectList()` only on collections. |
| `Failed to convert item at index {i}: '{item}'` | An element inside the collection failed to parse to the target type (e.g., `"abc"` cannot be parsed to `int`). The wrapped inner exception describes the real cause: `Item at index {i} is null and cannot be converted.` for a `null` element, or the parser error (`FormatException` / `OverflowException`). A `FormatException` usually means the text does not match the **current culture** (see [FAQ → Culture and Globalization](FAQ.md#culture-and-globalization)). | Verify the data types of your collection elements. Use `GetStringList()` if you only need string representations, or clean/normalize your data before conversion. |

### InvalidOperationException

| Scenario | What it means | How to fix |
|----------|---------------|------------|
| `The source object of type '{TypeName}' cannot be converted to List<{T}>.` | You called `ToList<T>()` on a wrapped object that does **not** exactly implement `IEnumerable<T>`. Common cases: using `ArrayList` (non-generic), or trying to convert `List<int>` to `List<object>`. | Use the appropriate `GetXxxList()` extension method instead (e.g., `GetInt32List()`, `GetDoubleList()`, `GetStringList()`). These methods iterate and convert each element. Alternatively, use `ToObject<T>()` and manually cast/convert. |

## Additional Notes

- DSL parameters are written with a leading `/` and a `:` separator, e.g., `/tostring:F2`. In error messages, parameter names are shown **without** the leading `/` for brevity.
- `Alchemy.FormatAsync` and `Alchemy.TransformAsync` wraps the synchronous call in `Task.Run` – it throws the same exceptions, just asynchronously.
- In `Transform(obj, params string[])`: `null` → `NullReferenceException`; empty entry → inherits the previous function name, or `MissingMethodException` if there is none. A single empty instruction → `ArgumentNullException`.
- `cnv` and `arr` without their parameters (`/temp`, `/length`, `/weight`, `/sort`) are no-ops: `Alchemy.Transform` still returns a deep clone of the input.
- `null` elements inside a collection are formatted as empty strings – they **do not** cause exceptions.
- `Alchemy.Format` does **not** modify the input object (no cloning). Only `Alchemy.Transform` performs deep cloning.
- If you encounter an error not listed here, examine the full exception message – it often includes the exact parameter name or type that caused the problem.
