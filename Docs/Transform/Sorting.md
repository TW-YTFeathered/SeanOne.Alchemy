# Sorting with Alchemy.Transform

The `/sort` parameter sorts collections **in‑place** on a deep‑cloned copy. Input must implement `IList` (e.g., arrays, `List<T>`, `ArrayList`).

> **Note:** All algorithm keys are **case‑insensitive** – `isd`, `ISD`, `IsD` all work.  

## DSL Syntax

```
arr /sort:algorithm
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
Alchemy.Transform(numbers, "arr /sort:is");
// Result list: [1, 2, 5, 8]
```

### Descending array sort

```csharp
int[] array = { 9, 3, 7, 2 };
Alchemy.Transform(array, "arr /sort:asd");
// Result: [9, 7, 3, 2]
```

### LINQ sort ascending

```csharp
var list = new ArrayList { 3, 1, 4, 1, 5 };
Alchemy.Transform(list, "arr /sort:ls");
// Result: [1, 1, 3, 4, 5]
```

## Important Notes

- Sorting is performed on a **deep clone** of the input collection; the original object is unchanged.
- The collection must be modifiable (non‑readonly) and implement `IList`.
- Elements must be mutually comparable (implement `IComparable`), otherwise an exception is thrown.
- For performance, `Array.Sort` (O(n log n)) is generally fastest; Insertion sort (O(n²)) is suitable for small collections.

If you need a different order, use multi‑instruction array (see [DSLSyntax.md](../DSLSyntax.md#multi‑instruction-execution)).

## Error Cases

- Using `/sort` on a non‑`IList` → `ArgumentException`.
- Unknown algorithm key → `ArgumentException`.
- Elements not comparable → `InvalidOperationException`.
- See [ErrorHandling.md](../ErrorHandling.md) for details.
