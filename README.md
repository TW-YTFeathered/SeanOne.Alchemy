# SeanOne.Alchemy

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)  
![.NET Support](https://img.shields.io/badge/.NET-net6.0%20%7C%20net8.0%20%7C%20net10.0%20%7C%20netstandard2.0%20%7C%20net472%20%7C%20net48-blue)

**Alchemy** is a lightweight C# library that enables fast and flexible object transformation through concise DSL syntax – a true data alchemy.

> ⚠️ **Important – API Update**  
> The class `AlchemyFormatter` is now **obsolete** and will be removed in a future version.  
> Please use the new unified entry points:  
> - `Alchemy.Format` for formatting objects to strings.  
> - `Alchemy.Transform` for conversions (sorting, unit conversion, etc.) and combined operations.  
> Both methods return an `AlchemyResult` which provides fluent conversion and chaining.

## ✨ Core Features

- **Object Formatting** – Convert objects to strings with custom separators, format strings, etc.
- **Object Conversion** – Sort collections or perform temperature/weight/length unit conversions
- **DSL Driven** – Describe transformation logic with intuitive string instructions (e.g., `cnv /temp:C->F` or `arr /sort:is`)
- **Fluent API** – Type‑safe builder with full IntelliSense and compile‑time checks
- **Async Support** – Both synchronous and asynchronous methods for all operations

## 📦 Installation

```bash
dotnet add package SeanOne.Alchemy
```

## 🚀 Quick Examples

### Formatting

```csharp
using SeanOne.Alchemy;

// Format a single value
string result = Alchemy.Format(5, "/tostring:F2 /end:!");
// Output: "5.00!"

// Format a collection
var items = new[] { "apple", "banana", "cherry" };
string list = Alchemy.Format(items, "fe /end:\", \" /final-pair-separator:\" and \" /exclude-last-end:true");
// Output: "apple, banana and cherry"
```

### Conversion

```csharp
using SeanOne.Alchemy;

// Sort a list
var numbers = new List<int> { 5, 2, 8, 1 };
var sortedResult = Alchemy.Transform(numbers, "arr /sort:is");
List<int> sorted = sortedResult.ToObject<List<int>>();
// Result: [1, 2, 5, 8]

// Temperature conversion
double fahrenheit = 212.0;
double celsius = Alchemy.Transform(fahrenheit, "cnv /temp:F->C").ToObject<double>();
// Result: 100.0

// Combine operations
var temps = new List<double> { 32.0, 212.0, 0.0 };
var result = Alchemy.Transform(temps, "arr /sort:bubble", "cnv /temp:F->C");
// Sorts then converts: [-17.777..., 0, 100]
```

## 📖 Documentation

| Topic | Location |
|-------|----------|
| Getting Started | [Docs/GettingStarted.md](Docs/GettingStarted.md) |
| DSL Syntax Reference | [Docs/DSLSyntax.md](Docs/DSLSyntax.md) |
| Formatting Guide | [Docs/Formatter/](Docs/Formatter/) |
| Conversion Guide | [Docs/Transform/](Docs/Transform/) |
| Fluent API | [Docs/FluentAPI.md](Docs/FluentAPI.md) |
| Error Handling | [Docs/ErrorHandling.md](Docs/ErrorHandling.md) |
| FAQ | [Docs/FAQ.md](Docs/FAQ.md) |

## 🤖 AI Assistance

Documentation assisted by **DeepSeek**.

## 📄 License

MIT License
