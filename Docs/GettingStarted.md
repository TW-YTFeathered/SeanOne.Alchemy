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
var sorted = Alchemy.Transform(numbers, "cnv /sort:is");
// sorted contains [1, 3, 5, 9]
```

## Combining Operations

You can pass multiple instructions as an array:

```csharp
using SeanOne.Alchemy;

var temps = new List<double> { 32, 212, 0 };
var result = Alchemy.Transform(temps, "cnv /sort:bubble", "/temp:F->C");
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
- Explore [Formatter](Formatter/) and [Transform](Transform/) for detailed examples.
- Check [Error Handling](ErrorHandling.md) when things go wrong.
- See [FAQ](FAQ.md) for common questions.

Happy alchemizing!
