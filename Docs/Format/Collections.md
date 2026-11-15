# Formatting Collections (IEnumerable)

Use the `fe` (or `foreach`) function to format sequences.

## DSL Syntax

```
fe /param:value /param2:value ...
```

## Parameters

### Common Parameters (same as basic)

| Parameter | Example | Description |
|-----------|---------|-------------|
| `/tostring` | `/tostring:F2` | Format applied to each element (must implement `IFormattable`). |
| `/begin` | `/begin:"* "` | Prepended **before each element**. |
| `/end` | `/end:", "` | Appended **after each element**. |
| `/prefix` | `/prefix:[` | Added before the whole sequence result. |
| `/suffix` | `/suffix:]` | Added after the whole sequence result. |
| `/exclude-last-end` | `/exclude-last-end:true` | If `true`, the `/end` string is **not** appended after the last element. |
| `/final-pair-separator` | `/final-pair-separator:" and "` | Replaces the `/end` separator between the last two elements. Useful for English lists (e.g., "A, B and C"). |
| `/fe-opt` | `/fe-opt:true` | Enables optimized formatter (~1.5x faster). See [FAQ](../FAQ.md#what-are-the-compatibility-risks-of-fe-opttrue) for compatibility notes. |

## Examples

### Simple comma‑separated list

```csharp
using SeanOne.Alchemy;

var numbers = new List<int> { 1, 2, 3, 4, 5 };
Alchemy.Format(numbers, "fe /end:\", \"");
// Returns: "1, 2, 3, 4, 5, "
```

### Excluding trailing separator

```csharp
using SeanOne.Alchemy;

var items = new[] { "apple", "banana", "cherry" };
Alchemy.Format(items, "fe /end:\", \" /exclude-last-end:true");
// Returns: "apple, banana, cherry"
```

### Using final‑pair‑separator

```csharp
using SeanOne.Alchemy;

var items = new[] { "apple", "banana", "cherry" };
Alchemy.Format(items, "fe /end:\", \" /final-pair-separator:\" and \" /exclude-last-end:true");
// Returns: "apple, banana and cherry"
```

### Formatting numbers with two decimals

```csharp
using SeanOne.Alchemy;

var doubles = new List<double> { 1.0, 2.5, 3.14 };
Alchemy.Format(doubles, "fe /tostring:F2 /end:\", \"");
// Returns: "1.00, 2.50, 3.14, "
```

### Date collection with newlines

```csharp
using SeanOne.Alchemy;

var dates = new List<DateTime> { new DateTime(2024,1,1), new DateTime(2024,2,1) };
Alchemy.Format(dates, "fe /tostring:yyyy-MM-dd /end:\\n");
// Returns:
// 2024-01-01
// 2024-02-01
```

## Fluent API Equivalent

```csharp
AlchemyFormatBuilder.SelectFeSeq()
    .With(FeSeqParam.ToString, "F2")
    .With(FeSeqParam.End, ", ")
    .With(FeSeqParam.FinalPairSeparator, " and ")
    .With(FeSeqParam.ExcludeLastEnd, true)
    .BuildRun(myList);
```

## Notes

- `string` is **not** treated as an enumerable (throws `ArgumentException`).
- For dictionaries, see [Dictionaries.md](Dictionaries.md).
- For performance tips, see [FAQ.md](../FAQ.md) (when to use `/fe-opt`).
- An **empty sequence** returns only `prefix + suffix` – `/begin`, `/end`, `/tostring` and the separators are never emitted.
- `/final-pair-separator` replaces the separator that follows the **second‑to‑last** element. The last element still receives `/end` unless `/exclude-last-end:true`, so `fe /end:", " /final-pair-separator:" and "` on `[a, b, c]` returns `"a, b and c, "`.
- Boolean flags (`/exclude-last-end`, `/fe-opt`) only accept `true` / `false`; any other value (`yes`, `1`, …) is silently treated as `false` instead of raising an error.
- `/tostring` follows the current culture (`CultureInfo.CurrentCulture`); see [FAQ → Culture and Globalization](../FAQ.md#culture-and-globalization).
