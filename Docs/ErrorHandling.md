# Error Handling

This document lists common exceptions thrown by `Alchemy.Format` and how to resolve them.  
*Note: This document covers exceptions for both `Alchemy.Format` and `Alchemy.Transform`. Errors are grouped by operation type to help you quickly locate the cause.*

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
| `Invalid parameters for basic processing: ...` | The DSL instruction contains parameter(s) not allowed for `basic`. | Check the list of allowed parameters for `basic` (`/tostring`, `/prefix`, `/suffix`, `/begin`, `/end`). Remove unsupported ones. |
| `Invalid parameters for enumerable processing: ...` | The DSL contains parameters that are not allowed for sequences (e.g., `/dict-format` used on a list). | Only use dictionary‑specific parameters (`/dict-format`, `/key-format`, `/value-format`) when the input is actually an `IDictionary`. |
| `Invalid parameters for dictionary processing: ...` | The DSL contains parameters not allowed for dictionaries (e.g., `/final-pair-separator` might not be applicable – check current parameter table). | Use valid dictionary parameters: `/dict-format`, `/key-format`, `/value-format`, `/begin`, `/end`, `/prefix`, `/suffix`, `/exclude-last-end` (and optionally `/fe-opt`). |
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

### Unit Conversion (`/weight`, `/length`, `/temp`)

| Exception | Common Scenarios |
|-----------|------------------|
| `ArgumentException` | Unknown unit code (e.g., `/temp:X->Y`). <br> Negative value for length or weight. <br> Temperature below absolute zero (0 K). <br> Unable to determine element type of the list (non-generic collections without clear type). |
| `InvalidOperationException` | A collection element is `null` and cannot be converted. <br> An element cannot be cast to a numeric type (`double`). <br> The conversion result overflows the target element type (e.g., converting a huge `double` to `int`). |

### Sorting (`/sort`)

| Exception | Scenario |
|-----------|----------|
| `ArgumentException` | The input object does not implement `IList`. <br> An unknown sorting algorithm key is used (e.g., `/sort:bubble`). |
| `InvalidOperationException` | The collection contains elements that do not implement `IComparable` and thus cannot be compared. |

### Formatting as a Fallback

When `Transform` receives a formatting instruction (`basic`, `fe`, `foreach`, or an instruction starting with `/`), it internally routes the call to `Alchemy.Format`. In these cases, **all exceptions listed in the [Alchemy.Format Errors](#alchemyformat-errors) section** apply identically (e.g., invalid parameters, unsupported `/tostring` on strings, etc.).

## Additional Notes

- `Alchemy.FormatAsync` and `Alchemy.TransformAsync` wraps the synchronous call in `Task.Run` – it throws the same exceptions, just asynchronously.
- `null` elements inside a collection are formatted as empty strings – they **do not** cause exceptions.
- `Alchemy.Format` does **not** modify the input object (no cloning). Only `Alchemy.Transform` performs deep cloning.
- If you encounter an error not listed here, examine the full exception message – it often includes the exact parameter name or type that caused the problem.
