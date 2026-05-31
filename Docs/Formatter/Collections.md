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
| `/fe-opt` | `/fe-opt:true` | Enables optimized formatter (~1.5x faster) but may have compatibility issues with custom collections. |

## Examples

### Simple comma‑separated list

```csharp
var numbers = new List<int> { 1, 2, 3, 4, 5 };
AlchemyFormatter.Format(numbers, "fe /end:\", \"");
// Returns: "1, 2, 3, 4, 5, "
```

### Excluding trailing separator

```csharp
var items = new[] { "apple", "banana", "cherry" };
AlchemyFormatter.Format(items, "fe /end:\", \" /exclude-last-end:true");
// Returns: "apple, banana, cherry"
```

### Using final‑pair‑separator

```csharp
var items = new[] { "apple", "banana", "cherry" };
AlchemyFormatter.Format(items, "fe /end:\", \" /final-pair-separator:\" and \" /exclude-last-end:true");
// Returns: "apple, banana and cherry"
```

### Formatting numbers with two decimals

```csharp
var doubles = new List<double> { 1.0, 2.5, 3.14 };
AlchemyFormatter.Format(doubles, "fe /tostring:F2 /end:\", \"");
// Returns: "1.00, 2.50, 3.14, "
```

### Date collection with newlines

```csharp
var dates = new List<DateTime> { new DateTime(2024,1,1), new DateTime(2024,2,1) };
AlchemyFormatter.Format(dates, "fe /tostring:yyyy-MM-dd /end:\\n");
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
