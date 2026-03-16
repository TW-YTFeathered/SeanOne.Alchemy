# Conversion Guide (Guide_Convert.md)

> **Note:** This guide provides examples and parameter reference for object conversion using `AlchemyConverter`.  
> **Quick Start:** The following are common usage scenarios. Each example includes the input object, DSL instruction, and expected output.  
> **Compatibility:** Examples use `C# 9` top‑level statements and are supported only in `.NET 5+` or `.NET Core 3.1+`; they cannot be compiled on `.NET Framework`.

## Table of Contents

- [Quick Start](#quick-start)
- [Sorting Parameter `/sort`](#sorting-parameter-sort)
  - [Supported Algorithms](#supported-algorithms)
- [Temperature Conversion Parameter `/temp`](#temperature-conversion-parameter-temp)
  - [Supported Units](#supported-units)
- [Combined Usage](#combined-usage)
- [Error Scenarios](#error-scenarios) *(Placeholder – under development)*
- [Fluent API](#fluent-api) *(Placeholder – under development)*

---

## Quick Start

```csharp
using SeanOne.Alchemy;

// Sort a list using insertion sort (ascending)
var numbers = new List<int> { 5, 2, 8, 1 };
var result = AlchemyConverter.Convert(numbers, "cnv /sort:is");
// result.ToObject<List<int>>() → [1, 2, 5, 8]

// Temperature conversion: Fahrenheit to Celsius (single value)
double tempF = 212.0;
var converted = AlchemyConverter.Convert(tempF, "cnv /temp:F->C");
// converted.ToObject<double>() → 100.0

// Multi‑instruction: sort then convert
var data = new List<double> { 32.0, 212.0, 0.0 };
var multiResult = AlchemyConverter.Convert(data, "cnv /sort:is", "/temp:F->C");
// Converted list: [-17.777..., 0, 100] (sorted ascending then temperature converted)
```

---

## Sorting Parameter `/sort`

Sorts a collection **in‑place**. The input object must implement `IList` (e.g., arrays, `ArrayList`, `List<T>`).

Value format: `algorithm name` + (optional) `descending indicator`.

### Supported Algorithms

| Algorithm     | Ascending Keys                                 | Descending Keys (add `d`/`desc`)                          |
|---------------|------------------------------------------------|-----------------------------------------------------------|
| Bubble Sort   | `bs`, `bubble`, `bubblesort`                   | `bsd`, `bubbledesc`, `bubblesortdescending`               |
| Insertion Sort| `is`, `insertion`, `insertionsort`             | `isd`, `insertiondesc`, `insertiondescending`             |
| Array.Sort    | `as`, `arraysort`                              | `asd`, `arraysortdesc`, `arraysortdescending`             |
| LINQ Sort     | `ls`, `linq`, `linqsort`                       | `lsd`, `linqsortdesc`, `linqsortdescending`               |

**Example:**

```csharp
using SeanOne.Alchemy;

var list = new ArrayList { 3, 1, 4, 1, 5 };

// Bubble sort descending
AlchemyConverter.Convert(list, "cnv /sort:bsd");
// list → [5, 4, 3, 1, 1]

// LINQ ascending
AlchemyConverter.Convert(list, "cnv /sort:ls");
```

---

## Temperature Conversion Parameter `/temp`

Converts a numeric value or a collection of numeric values between temperature units.  
Value format: `sourceUnittoTargetUnit` or `sourceUnit->targetUnit` (case‑insensitive).

### Supported Units

- `C` – Celsius
- `F` – Fahrenheit
- `K` – Kelvin

**Examples:**

```csharp
using SeanOne.Alchemy;

// Single value
double celsius = 25;
AlchemyConverter.Convert(celsius, "cnv /temp:C->F");   // 77.0

// List of values
var temps = new List<double> { 0, 100, -40 };
AlchemyConverter.Convert(temps, "cnv /temp:C->K");
// temps → [273.15, 373.15, 233.15]
```

---

## Combined Usage

You can specify both /sort and /temp in one cnv instruction. The execution order is determined by the function's internal logic (first sorting, then temperature conversion), regardless of the order in which the parameters appear.

```csharp
using SeanOne.Alchemy;

var data = new List<double> { 32.0, 212.0, 0.0 };
AlchemyConverter.Convert(data, "cnv /sort:bubble /temp:F->C");
// First sort (ascending): [0, 32, 212]
// Then temperature conversion: [-17.777..., 0, 100]
```

---

## Multi‑Instruction Support

You can specify both /sort and /temp in one cnv instruction. The execution order is determined by the function's internal logic (first sorting, then temperature conversion), regardless of the order in which the parameters appear.

```csharp
using SeanOne.Alchemy;

// Both calls are equivalent:
AlchemyConverter.Convert(data, "cnv /sort:is", "/temp:F->C");
AlchemyConverter.Convert(data, "cnv /sort:is /temp:F->C");
```

The multi‑instruction form is especially useful when building instructions dynamically.

---

## Error Scenarios

> **⚠️ Note:** This section is a placeholder. Exact error messages and behaviors are still under development and may change in future releases.

### Invalid sorting algorithm

```csharp
using SeanOne.Alchemy;

AlchemyConverter.Convert(list, "cnv /sort:unknown");
// [Error details will be added here]
```

### Using `/sort` on non‑collection

```csharp
using SeanOne.Alchemy;

AlchemyConverter.Convert(5, "cnv /sort:bubble");
// [Error details will be added here]
```

### Invalid temperature conversion instruction

```csharp
using SeanOne.Alchemy;

AlchemyConverter.Convert(100, "cnv /temp:X->Y");
// [Error details will be added here]
```

### Collection element cannot be converted to numeric

```csharp
using SeanOne.Alchemy;

var list = new List<object> { 1, "two", 3 };
AlchemyConverter.Convert(list, "cnv /temp:C->F");
// [Error details will be added here]
```

### Missing or empty parameter values

```csharp
using SeanOne.Alchemy;

AlchemyConverter.Convert(data, "cnv /sort:");      // Empty value
// [Error details will be added here]

AlchemyConverter.Convert(data, "cnv /temp");       // Missing colon
// [Error details will be added here]
```

---

## Fluent API

> **⚠️ Note:** The Fluent API for conversions is currently under development. This section will be updated once the API is finalized.

In future releases, a type‑safe Fluent API will be available for constructing conversion instructions with full IntelliSense support. The API is expected to follow patterns similar to the [formatting Fluent API](https://github.com/TW-YTFeathered/SeanOne.Alchemy/blob/master/Guide_Format.md#fluent-api-demonstration).

**Planned usage example (subject to change):**

```csharp
using SeanOne.Alchemy;

// This is a preview – actual API may differ
var converter = AlchemyConversionBuilder.Create()
    .Sort(SortAlgorithm.Bubble, descending: false)
    .ConvertTemperature(TemperatureUnit.Fahrenheit, TemperatureUnit.Celsius)
    .Build();

var result = converter.Run(data);
```

Please check future releases for updates.

---

## Additional Notes

- All conversion methods also have asynchronous counterparts: `ConvertAsync`.
- The input object is **deep cloned** before any modifications, ensuring the original remains unchanged.
- For detailed DSL syntax rules (escaping, quoting, etc.), refer to the [Formatting Guide](https://github.com/TW-YTFeathered/SeanOne.Alchemy/blob/master/Guide_Format.md#escape-sequences).
