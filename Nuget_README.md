# SeanOne.Alchemy

A lightweight and efficient C# library for fast object transformation using a simple DSL syntax. Format collections, convert units, sort data – all with intuitive string instructions.

> 📌 This documentation reflects the upcoming V3.0.0 release.

## Quick Examples

### Formatting a collection

```csharp
using SeanOne.Alchemy;

var numbers = Enumerable.Range(0, 10).ToList();

string result = Alchemy.Format(numbers, 
    "fe /tostring:F2 /end:\\n /exclude-last-end:true");

// Output:
// 0.00
// 1.00
// 2.00
// ...
// 9.00
```

### Sorting and unit conversion

```csharp
using SeanOne.Alchemy;

var temps = new List<double> { 32.0, 212.0, 0.0 };

AlchemyResult result = Alchemy.Transform(temps,
    "arr /sort:is",        // Sort ascending using insertion sort
    "cnv /temp:F->C"       // Convert Fahrenheit to Celsius
);

List<double> converted = result.ToObject<List<double>>();
// Result: [-17.77, 0, 100]
```

### Fluent API (type‑safe)

```csharp
using SeanOne.Alchemy.Builder;

var numbers = Enumerable.Range(0, 10).ToList();

string result = AlchemyFormatBuilder.SelectFeSeq()
    .With(FeSeqParam.ToString, "F2")
    .With(FeSeqParam.End, "\\n")
    .With(FeSeqParam.ExcludeLastEnd, true)
    .BuildRun(numbers);

// Output:
// 0.00
// 1.00
// 2.00
// ...
// 9.00
```

## Features

- **Formatting** – Custom separators, prefixes, suffixes, and format strings for any object
- **Conversion** – Sort collections or convert temperature/weight/length units
- **DSL Driven** – Intuitive string instructions like `/temp:C->F` or `/sort:is`
- **Fluent API** – Type‑safe builder with full IntelliSense support
- **Async Support** – All operations available in both sync and async versions

## Documentation

- [Full README](https://github.com/TW-YTFeathered/SeanOne.Alchemy/tree/master/README.md)
- [Getting Started](https://github.com/TW-YTFeathered/SeanOne.Alchemy/blob/master/Docs/GettingStarted.md)

## GitHub Repository

[SeanOne.Alchemy on GitHub](https://github.com/TW-YTFeathered/SeanOne.Alchemy/)

## License

MIT License
