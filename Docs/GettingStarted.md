# Getting Started with Alchemy

Alchemy is a lightweight C# library that transforms objects using concise DSL instructions. This guide will get you running in 5 minutes.

## Installation

```bash
dotnet add package SeanOne.Alchemy --version 2.0.0
```

Or via Package Manager Console:
```
Install-Package SeanOne.Alchemy -Version 2.0.0
```

## Basic Concepts

Alchemy provides two main classes:

- `Alchemy.Format` – converts objects to formatted strings.
- `Alchemy.Transform` – sorts collections, converts temperature units, etc.

Both accept a **DSL instruction string** that describes the transformation.

## Your First Formatting

```csharp
using SeanOne.Alchemy;

string result = Alchemy.Format(123.456, "/tostring:F2 /prefix:\"$\" /suffix:\" USD\"");
Console.WriteLine(result); // $123.46 USD
```

## Your First Conversion

```csharp
using SeanOne.Alchemy;

var numbers = new List<int> { 5, 1, 9, 3 };
var sorted = Alchemy.Transform(numbers, "arr /sort:is");
// sorted contains [1, 3, 5, 9]
```

## Extracting Results

`Alchemy.Transform` returns an `AlchemyResult` object. You can extract the transformed data using the following methods:

### Direct Access (No Conversion)

Use these when you know the exact type of the wrapped object.

| Method | Description |
| :--- | :--- |
| `ToObject<T>()` | Returns the wrapped object directly as type `T`. |
| `ToList<T>()` | If the wrapped object is an `IEnumerable<T>`, returns it as a `List<T>`. |

### Single Value Conversion

Use these to convert the wrapped object into a specific primitive type.

| Method | Description |
| :--- | :--- |
| `GetString()` | Calls `ToString()` on the wrapped object and returns the result. |
| `GetDouble()` | Parses the wrapped object as `double`. |
| `GetInt32()` | Parses the wrapped object as `int`. |
| `GetDateTime()` | Parses the wrapped object as `DateTime`. |
| ... and many more | See `AlchemyConverterExpansions` for the full list. |

### Collection Conversion

Use these to convert every element of a collection into a specific type. Each method iterates the collection and applies the corresponding parser to each element, returning a **new** `List<T>`.

| Method | Description |
| :--- | :--- |
| `GetStringList()` | Converts all elements to strings. |
| `GetDoubleList()` | Converts all elements to doubles. |
| `GetInt32List()` | Converts all elements to ints. |
| `GetDateTimeList()` | Converts all elements to `DateTime`. |
| `GetObjectList()` | Returns a `List<object>` containing all elements. |

> 💡 **Tip:** Use `ToObject<T>()` when you already know the exact type (e.g., `List<double>`). It is more efficient because it avoids creating a new collection. Use `GetXxxList()` when you want to **convert** the elements to a specific type, or when you are unsure of the original collection type.

## Combining Operations

You can pass multiple instructions as an array:

```csharp
using SeanOne.Alchemy;

var temps = new List<double> { 32, 212, 0 };
var result = Alchemy.Transform(temps, "arr /sort:is", "cnv /temp:F->C");
// Sorts then converts Fahrenheit to Celsius
```

## Fluent API (Type‑Safe)

If you prefer compile‑time safety:

```csharp
using SeanOne.Alchemy.Builder;

string output = AlchemyFormatBuilder.SelectBasic()
    .With(BasicParam.ToString, "F2")
    .With(BasicParam.Prefix, "Value: ")
    .BuildRun(5);
```

## Next Steps

- Read the [DSL Syntax Reference](DSLSyntax.md) for all parameter rules.
- Explore [Format](Format/) and [Transform](Transform/) for detailed examples.
- Check [Error Handling](ErrorHandling.md) when things go wrong.
- See [FAQ](FAQ.md) for common questions.

Happy alchemizing!
