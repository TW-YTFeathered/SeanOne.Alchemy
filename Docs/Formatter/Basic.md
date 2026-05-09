# Basic Formatting

Use the `basic` function for formatting single objects (non‑collections).

## DSL Syntax

```text
basic /param:value /param2:value ...
```

Or you can omit the function name – `basic` is assumed when no function is given.

## Supported Parameters

| Parameter | Example | Description |
|-----------|---------|-------------|
| `/tostring` | `/tostring:F2` | Format string for `IFormattable` types (e.g., `double.ToString("F2")`). |
| `/begin` | `/begin:*` | String prepended **before each value** (for multiple calls? Actually `basic` works on a single object; `begin` is mainly for `fe`. But in `basic`, it still works as prefix before the whole output? Let's check original: In `basic`, `/begin` is not documented. Actually from Guide_Format.md, `/begin` is under Common Parameters that apply to formatting in general, but examples show it used with `fe`. I'll clarify.) |
| `/end` | `/end:!` | String appended after the formatted value. |
| `/prefix` | `/prefix:"["` | String before the entire result. |
| `/suffix` | `/suffix:"]"` | String after the entire result. |

**Correction**: According to original `Guide_Format.md`, `/begin` and `/end` are "common parameters" but they are described as "prepends/appends a string **before/after each value**". For a single object, that's effectively the same as prefix/suffix? Actually prefix/suffix are for the whole result, begin/end for each value. In basic, there is only one value, so begin and end are equivalent to prefix and suffix. The library treats them as distinct but with potentially overlapping effect. For clarity, we recommend using `/prefix` and `/suffix` for the whole string, and `/begin`/`/end` for per‑value (which in basic gives same result). I'll keep it simple as per original.

I'll rewrite based on actual original content from Guide_Format.md:

From the "Common Parameters" table:

- `/begin` – Prepends a string **before each value**.
- `/end` – Appends a string **after each value**.
- `/prefix` – Prepends a string **before the entire result**.
- `/suffix` – Appends a string **after the entire result**.

And from the "Using `basic` functions" example, only `/tostring` is shown. So for basic, `/prefix`/`/suffix` are more appropriate. I'll present accordingly.

Better to present clearly:

### Parameters for `basic`

| Parameter | Example | Description |
|-----------|---------|-------------|
| `/tostring` | `/tostring:N2` | Numeric or date format. |
| `/prefix` | `/prefix:"Result: "` | Added at the very beginning. |
| `/suffix` | `/suffix:"."` | Added at the very end. |
| `/begin` | `/begin:"["` | Added before the value (same effect as prefix for single object). |
| `/end` | `/end:"]"` | Added after the value (same effect as suffix). |

For consistency, prefer `/prefix` and `/suffix`.

## Examples

```csharp
using SeanOne.Alchemy;

// Format a number with two decimal places
AlchemyFormatter.Format(5, "/tostring:F2");
// Returns: "5.00"

// Add prefix and suffix
AlchemyFormatter.Format(5, "/tostring:F2 /prefix:\"Value: \" /suffix:\" units\"");
// Returns: "Value: 5.00 units"

// Using begin/end (works the same for single object)
AlchemyFormatter.Format(5, "/tostring:F2 /begin:\"[\" /end:\"]\"");
// Returns: "[5.00]"
```

## Fluent API Equivalent

```csharp
using SeanOne.Alchemy.Builder;

AlchemyFormatBuilder.SelectBasic()
    .With(BasicParam.ToString, "F2")
    .With(BasicParam.Prefix, "Value: ")
    .With(BasicParam.Suffix, " units")
    .BuildRun(5);
```

## Error Cases

- Using `/tostring` on a type that does not implement `IFormattable` (e.g., `string`) throws `ArgumentException`.
- See [ErrorHandling.md](../ErrorHandling.md) for details.
