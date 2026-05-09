# Error Handling

This document lists common exceptions thrown by `AlchemyFormatter` and how to resolve them.  
*Note: `AlchemyConverter` error handling is under development and not covered here.*

## ArgumentNullException

| Message / Scenario | What it means | How to fix |
|-------------------|---------------|-------------|
| `Input object must not be null.` | You passed `null` as the object to format. | Make sure the object you are formatting is not `null`. |
| `Alchemy instruction cannot be null or empty` | The DSL instruction string is `null` or empty. | Provide a valid instruction, e.g., `"/tostring:F2"`. |
| `Target object cannot be null for 'fe' directive` (or `'foreach'`) | Inside `fe` / `foreach`, the input object is `null`. | Check that your collection is not `null` before formatting. |
| `'dict-format' parameter is required when processing dictionaries.` | You used `fe` on a dictionary but omitted `/dict-format`. | Add `/dict-format:{0}→{1}` (or any format with `{0}` for key, `{1}` for value). |

## ArgumentException

| Message / Scenario | What it means | How to fix |
|--------------------|---------------|-------------|
| `String is not supported for 'fe' directive` (or `'foreach'`) | You tried to format a `string` with `fe`. | Use `basic` instead, or wrap the string in a collection like `new[] { myString }`. |
| `Object must implement IEnumerable for 'fe' directive` | You used `fe` on a non‑collection object. | Use `basic` for single objects. If you intended a collection, ensure it implements `IEnumerable`. |
| `Invalid parameters for basic processing: ...` | The DSL instruction contains parameter(s) not allowed for `basic`. | Check the list of allowed parameters for `basic` (`/tostring`, `/prefix`, `/suffix`, `/begin`, `/end`). Remove unsupported ones. |
| `Invalid parameters for enumerable processing: ...` | The DSL contains parameters that are not allowed for sequences (e.g., `/dict-format` used on a list). | Only use dictionary‑specific parameters (`/dict-format`, `/key-format`, `/value-format`) when the input is actually an `IDictionary`. |
| `Invalid parameters for dictionary processing: ...` | The DSL contains parameters not allowed for dictionaries (e.g., `/final-pair-separator` might not be applicable – check current parameter table). | Use valid dictionary parameters: `/dict-format`, `/key-format`, `/value-format`, `/begin`, `/end`, `/prefix`, `/suffix`, `/exclude-last-end` (and optionally `/fe-opt`). |
| `Collection elements must implement IFormattable for 'tostring'. Found: {TypeName}` | You used `/tostring` but one of the elements (or the object itself in `basic`) does not implement `IFormattable`. Common example: trying to format a `string` with `/tostring:F2`. | Remove `/tostring` if you don't need custom formatting. If you need numeric formatting, ensure the collection contains numeric types (like `int`, `double`, `DateTime`, etc.), not `string`. |
| `Parameter '/xxx:' is specified multiple times.` | The same parameter (e.g., `/tostring`) appears more than once in the same instruction. | Use each parameter only once. Combine or remove duplicates. |

## MissingMethodException

| Message / Scenario | What it means | How to fix |
|--------------------|---------------|-------------|
| `Unknown functions directive: {directive}` | The first word of your instruction is not `basic`, `fe`, `foreach`, and does not start with `/`. For example, `loop /tostring:F2`. | Use a valid function name: `basic`, `fe`, or `foreach`. Or if you omit the function name, start directly with a parameter (e.g., `/tostring:F2`) which defaults to `basic`. |

## Additional Notes

- `AlchemyFormatter.FormatAsync` wraps the synchronous call in `Task.Run` – it throws the same exceptions, just asynchronously.
- `null` elements inside a collection are formatted as empty strings – they **do not** cause exceptions.
- `AlchemyFormatter` does **not** modify the input object (no cloning). Only `AlchemyConverter` performs deep cloning.
- If you encounter an error not listed here, examine the full exception message – it often includes the exact parameter name or type that caused the problem.

For `AlchemyConverter` errors (sorting, temperature conversion, etc.), please note that exception handling is still under development and will be documented in a future release.
