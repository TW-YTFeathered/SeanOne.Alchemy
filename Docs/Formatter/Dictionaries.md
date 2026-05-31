# Formatting Dictionaries (IDictionary)

Use the `fe` function to format dictionaries.

## DSL Syntax

```text
fe /dict-format:formatString /param:value ...
```

## Required Parameter

| Parameter | Example | Description |
|-----------|---------|-------------|
| `/dict-format` | `/dict-format:{0} => {1}` | Format string where `{0}` is the key, `{1}` is the value. Must be provided; cannot be empty. |

## Optional Parameters

All parameters below work with dictionaries exactly as they do with sequences (`IEnumerable`), except where noted.

| Parameter | Example | Description |
|-----------|---------|-------------|
| `/key-format` | `/key-format:F2` | Format string applied to each key (keys must implement `IFormattable`). |
| `/value-format` | `/value-format:yyyy-MM-dd` | Format string applied to each value (values must implement `IFormattable`). |
| `/begin` | `/begin:[` | Prepended **before each dictionary entry**. |
| `/end` | `/end:", "` | Appended **after each dictionary entry**. |
| `/prefix` | `/prefix:{` | Added before the whole result. |
| `/suffix` | `/suffix:}` | Added after the whole result. |
| `/exclude-last-end` | `/exclude-last-end:true` | If `true`, the `/end` string is **not** appended after the last entry. |
| `/final-pair-separator` | `/final-pair-separator:" and "` | Replaces the `/end` separator between the last two entries. |
| `/fe-opt` | `/fe-opt:true` | Enables optimized formatter (~1.5x faster) but may have compatibility issues with custom dictionaries. |

## Examples

### Simple key‑value pair

```csharp
var dict = new Dictionary<int, string> { { 1, "one" }, { 2, "two" } };
AlchemyFormatter.Format(dict, "fe /dict-format:{0}={1} /end:\", \"");
// Returns: "1=one, 2=two, "
```

### Using `/exclude-last-end` and `/final-pair-separator`

```csharp
var dict = new Dictionary<string, int> { { "apple", 1 }, { "banana", 2 }, { "cherry", 3 } };
AlchemyFormatter.Format(dict, "fe /dict-format:{0}:{1} /end:\", \" /final-pair-separator:\" and \" /exclude-last-end:true");
// Returns: "apple:1, banana:2 and cherry:3"
```

### Applying format to keys and values

```csharp
var dict = new Dictionary<double, double> { { 32.0, 212.0 }, { 0.0, 273.15 } };
AlchemyFormatter.Format(dict, "fe /dict-format:\"{0}°C → {1}°F\" /key-format:F1 /value-format:F1 /end:\\n");
// Returns:
// 32.0°C → 212.0°F
// 0.0°C → 273.1°F
```

### Using only values (ignore keys)

```csharp
var dict = new Dictionary<double, double> { { 32.0, 212.0 }, { 0.0, 273.15 } };
AlchemyFormatter.Format(dict, "fe /dict-format:{1} /value-format:F2 /end:\", \"");
// Returns: "212.00, 273.15, "
```

### With prefix and suffix

```csharp
var dict = new Dictionary<int, string> { { 1, "one" }, { 2, "two" } };
AlchemyFormatter.Format(dict, "fe /dict-format:{0}-{1} /prefix:[ /suffix:] /end:\"; \"");
// Returns: "[1-one; 2-two; ]"
```

### Using optimized formatter

```csharp
AlchemyFormatter.Format(dict, "fe /dict-format:{0}:{1} /fe-opt:true /end:\\n");
```

## Fluent API Equivalent

```csharp
using SeanOne.Alchemy.Builder;

AlchemyFormatBuilder.SelectFeDict()
    .With(FeDictParam.DictFormat, "{0} -> {1}")
    .With(FeDictParam.KeyFormat, "F2")
    .With(FeDictParam.ValueFormat, "F2")
    .With(FeSeqParam.End, ", ")
    .With(FeSeqParam.ExcludeLastEnd, true)
    .With(FeSeqParam.FinalPairSeparator, " and ")
    .BuildRun(myDictionary);
```

## Common Errors

| Error | Cause | Solution |
|-------|-------|----------|
| `'dict-format' parameter is required when processing dictionaries.` | Missing or empty `/dict-format`. | Add `/dict-format:{0}={1}` (or any format with `{0}` and `{1}`). |
| `Invalid parameters for dictionary processing: ...` | Using a parameter not supported for dictionaries (e.g., trying to use `/tostring` directly – use `/key-format` and `/value-format` instead). | Check your parameter names against the table above. |

See [ErrorHandling.md](../ErrorHandling.md) for more details.
