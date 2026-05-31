# Basic Formatting

Use the `basic` function for formatting single objects (non‑collections).

## DSL Syntax

```text
basic /param:value /param2:value ...
```

You can omit the function name – `basic` is assumed when the instruction starts with a parameter (e.g., `/tostring:F2`).

## Supported Parameters

| Parameter | Example | Description |
|-----------|---------|-------------|
| `/tostring` | `/tostring:F2` | Format string for `IFormattable` types (e.g., `double.ToString("F2")`). |
| `/begin` | `/begin:[` | String inserted **immediately before the formatted value** (after `prefix`). |
| `/end` | `/end:]` | String inserted **immediately after the formatted value** (before `suffix`). |
| `/prefix` | `/prefix:"Result: "` | String added at the very beginning of the final output. |
| `/suffix` | `/suffix:"."` | String added at the very end of the final output. |

**Order of application:**

```
prefix + begin + (formatted value) + end + suffix
```

> The same order applies to `fe` (collections/dictionaries), where `begin` and `end` are applied to each element/entry.

## Examples

```csharp
using SeanOne.Alchemy;

// Format a number with two decimal places
AlchemyFormatter.Format(5, "/tostring:F2");
// Returns: "5.00"

// Add prefix and suffix
AlchemyFormatter.Format(5, "/tostring:F2 /prefix:\"Value: \" /suffix:\" units\"");
// Returns: "Value: 5.00 units"

// Using begin and end
AlchemyFormatter.Format(5, "/tostring:F2 /begin:[ /end:]");
// Returns: "[5.00]"

// Combine all
AlchemyFormatter.Format(5, "/tostring:F2 /prefix:\"Result: \" /begin:( /end:) /suffix:!");
// Returns: "Result: (5.00)!"
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

- Using `/tostring` on a type that does **not** implement `IFormattable` (e.g., `string`) → `ArgumentException`.
- Unknown parameter name → `ArgumentException`.
- See [ErrorHandling.md](../ErrorHandling.md) for details.
