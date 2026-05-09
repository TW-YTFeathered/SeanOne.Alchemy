# Sorting with AlchemyConverter

The `/sort` parameter sorts collections **in‑place** on a deep‑cloned copy. Input must implement `IList` (e.g., arrays, `List<T>`, `ArrayList`).

## DSL Syntax

```
cnv /sort:algorithm
```

Where `algorithm` is one of the supported algorithm keys, optionally followed by `d` or `desc` for descending order.

## Supported Algorithms

| Algorithm | Ascending Keys | Descending Keys |
|-----------|----------------|------------------|
| Insertion Sort | `is`, `insertion`, `insertionsort` | `isd`, `insertiondesc`, `insertiondescending` |
| Array.Sort (built‑in) | `as`, `arraysort` | `asd`, `arraysortdesc`, `arraysortdescending` |
| LINQ Sort | `ls`, `linq`, `linqsort` | `lsd`, `linqsortdesc`, `linqsortdescending` |

## Examples

### Ascending insertion sort

```csharp
var numbers = new List<int> { 5, 2, 8, 1 };
AlchemyConverter.Convert(numbers, "cnv /sort:is");
// Result list: [1, 2, 5, 8]
```

### Descending array sort

```csharp
int[] array = { 9, 3, 7, 2 };
AlchemyConverter.Convert(array, "cnv /sort:asd");
// Result: [9, 7, 3, 2]
```

### LINQ sort ascending

```csharp
var list = new ArrayList { 3, 1, 4, 1, 5 };
AlchemyConverter.Convert(list, "cnv /sort:ls");
// Result: [1, 1, 3, 4, 5]
```

## Important Notes

- Sorting is performed on a **deep clone** of the input collection; the original object is unchanged.
- The collection must be modifiable (non‑readonly) and implement `IList`.
- Elements must be mutually comparable (implement `IComparable`), otherwise an exception is thrown.
- For performance, `Array.Sort` (O(n log n)) is generally fastest; Insertion sort (O(n²)) is suitable for small collections.

## Combining with Temperature Conversion

You can combine `/sort` and `/temp` in one instruction. The execution order is **always sorting first, then temperature conversion** (regardless of parameter order).

```csharp
var data = new List<double> { 32.0, 212.0, 0.0 };
AlchemyConverter.Convert(data, "cnv /sort:is /temp:F->C");
// Sorts to [0, 32, 212] then converts to [-17.777..., 0, 100]
```

If you need a different order, use multi‑instruction array (see [DSLSyntax.md](../DSLSyntax.md#multi‑instruction-execution)).

## Error Cases

- Using `/sort` on a non‑`IList` → `ArgumentException`.
- Unknown algorithm key → `ArgumentException`.
- Elements not comparable → `InvalidOperationException`.
- See [ErrorHandling.md](../ErrorHandling.md) for details.
