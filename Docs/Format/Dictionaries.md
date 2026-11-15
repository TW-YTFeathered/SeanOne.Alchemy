# Formatting Dictionaries (IDictionary)

Use the `fe` (or `foreach`) function to format dictionaries.

## DSL Syntax

```text
fe /dict-format:formatString /param:value ...
```

## Required Parameter

| Parameter | Example | Description |
|-----------|---------|-------------|
| `/dict-format` | `/dict-format:{0} => {1}` | Format string where `{0}` is the key, `{1}` is the value. Must be provided; cannot be empty. |

> Only `{0}` and `{1}` are substituted. Any other placeholder (for example `{2}`) is emitted literally.

## Optional Parameters

All parameters below work with dictionaries exactly as they do with sequences (`IEnumerable`), except where noted.

| Parameter | Example | Description |
|-----------|---------|-------------|
| `/key-format` | `/key-format:F2` | Format string applied to each key. Only used when the key implements `IFormattable`; for other key types the format is silently ignored (no exception). |
| `/value-format` | `/value-format:yyyy-MM-dd` | Format string applied to each value. Only used when the value implements `IFormattable`; for other value types the format is silently ignored (no exception). |
| `/begin` | `/begin:[` | Prepended **before each dictionary entry**. |
| `/end` | `/end:", "` | Appended **after each dictionary entry**. |
| `/prefix` | `/prefix:{` | Added before the whole result. |
| `/suffix` | `/suffix:}` | Added after the whole result. |
| `/exclude-last-end` | `/exclude-last-end:true` | If `true`, the `/end` string is **not** appended after the last entry. |
| `/final-pair-separator` | `/final-pair-separator:" and "` | Replaces the `/end` separator between the last two entries. |
| `/fe-opt` | `/fe-opt:true` | Enables optimized formatter (~1.5x faster). See [FAQ](../FAQ.md#what-are-the-compatibility-risks-of-fe-opttrue) for compatibility notes. |

## Examples

### Simple key‑value pair

```csharp
using SeanOne.Alchemy;

var dict = new Dictionary<int, string> { { 1, "one" }, { 2, "two" } };
Alchemy.Format(dict, "fe /dict-format:{0}={1} /end:\", \"");
// Returns: "1=one, 2=two, "
```

### Using `/exclude-last-end` and `/final-pair-separator`

```csharp
using SeanOne.Alchemy;

var dict = new Dictionary<string, int> { { "apple", 1 }, { "banana", 2 }, { "cherry", 3 } };
Alchemy.Format(dict, "fe /dict-format:{0}:{1} /end:\", \" /final-pair-separator:\" and \" /exclude-last-end:true");
// Returns: "apple:1, banana:2 and cherry:3"
```

### Applying format to keys and values

```csharp
using SeanOne.Alchemy;

var dict = new Dictionary<double, double> { { 32.0, 212.0 }, { 0.0, 273.15 } };
Alchemy.Format(dict, "fe /dict-format:\"{0}°C → {1}°F\" /key-format:F1 /value-format:F1 /end:\\n");
// Returns:
// 32.0°C → 212.0°F
// 0.0°C → 273.1°F
```

### Using only values (ignore keys)

```csharp
using SeanOne.Alchemy;

var dict = new Dictionary<double, double> { { 32.0, 212.0 }, { 0.0, 273.15 } };
Alchemy.Format(dict, "fe /dict-format:{1} /value-format:F2 /end:\", \"");
// Returns: "212.00, 273.15, "
```

### With prefix and suffix

```csharp
using SeanOne.Alchemy;

var dict = new Dictionary<int, string> { { 1, "one" }, { 2, "two" } };
Alchemy.Format(dict, "fe /dict-format:{0}-{1} /prefix:[ /suffix:] /end:\"; \"");
// Returns: "[1-one; 2-two; ]"
```

### Using optimized formatter

```csharp
using SeanOne.Alchemy;

Alchemy.Format(dict, "fe /dict-format:{0}:{1} /fe-opt:true /end:\\n");
```

## Fluent API Equivalent

```csharp
using SeanOne.Alchemy.Builder;

AlchemyFormatBuilder.SelectFeDict()
    .With(FeDictParam.DictFormat, "{0} -> {1}")
    .With(FeDictParam.KeyFormat, "F2")
    .With(FeDictParam.ValueFormat, "F2")
    .With(FeDictParam.End, ", ")
    .With(FeDictParam.ExcludeLastEnd, true)
    .With(FeDictParam.FinalPairSeparator, " and ")
    .BuildRun(myDictionary);
```

## Notes

- An **empty dictionary** returns only `prefix + suffix`. The `/dict-format` check still runs first, so the parameter stays required.
- `/final-pair-separator` replaces the separator after the **second‑to‑last** entry; the last entry still receives `/end` unless `/exclude-last-end:true`.
- Boolean flags (`/exclude-last-end`, `/fe-opt`) only accept `true` / `false`; any other value is silently treated as `false` instead of raising an error.
- `/key-format`, `/value-format` and the `{0}` / `{1}` substitutions follow the current culture (`CultureInfo.CurrentCulture`); see [FAQ → Culture and Globalization](../FAQ.md#culture-and-globalization).
- Dictionary entries are `KeyValuePair` values, which never implement `IFormattable`. That is why `/tostring` is not a valid dictionary parameter – use `/key-format` and `/value-format`.

## Common Errors

| Error | Cause | Solution |
|-------|-------|----------|
| `'dict-format' parameter is required when processing dictionaries.` | Missing or empty `/dict-format`. | Add `/dict-format:{0}={1}` (or any format with `{0}` and `{1}`). |
| `Collection elements must implement IFormattable for 'tostring'. Found: KeyValuePair`2` | Using `/tostring` on a dictionary. Each entry is a `KeyValuePair`, which does not implement `IFormattable`. | Use `/key-format` and `/value-format` instead. |
| `Invalid parameters for dictionary processing: ...` | Using any other parameter that is not supported for dictionaries. | Check your parameter names against the table above. |

See [ErrorHandling.md](../ErrorHandling.md) for more details.
