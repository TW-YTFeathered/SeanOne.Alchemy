# SeanOne.Alchemy

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
![.NET Support](https://img.shields.io/badge/.NET-net6.0%2B%20%7C%20net8.0%20%7C%20netstandard2.0%20%7C%20net472%20%7C%20net48-blue)

**Alchemy** is a lightweight C# library that enables fast and flexible object transformation through concise DSL syntax – a true data alchemy.

## ✨ Core Features

- **Object Formatting** – Convert objects to strings with custom separators, format strings, etc. (`AlchemyFormatter`).
- **Object Conversion** – Sort collections or perform temperature unit conversions (`AlchemyConverter`).
- **DSL Driven** – Describe transformation logic with intuitive string instructions (e.g., `cnv /sort:bubble /temp:C->F`).
- **Fluent API** – Type‑safe builder with full IntelliSense and compile‑time checks.
- **Async Support** – Both synchronous and asynchronous methods for all operations.

## 📦 Installation

```bash
dotnet add package SeanOne.Alchemy --version 2.0.0
```

## 🚀 Quick Examples

### Formatting

```csharp
using SeanOne.Alchemy;

string result = AlchemyFormatter.Format(5, "/tostring:F2 /end:!");
// Output: "5.00!"
```

### Conversion

```csharp
using SeanOne.Alchemy;

var numbers = new List<double> { 32.0, 212.0, 0.0 };
var result = AlchemyConverter.Convert(numbers, "cnv /sort:is", "/temp:F->C");
// Converted list: [-17.777..., 0, 100] (sorted and temperature converted)
```

## Formatting with `AlchemyFormatter`

### Syntax

```text
[FunctionName] /param1:value1 /param2:"value with spaces" /param3:"value/with/slash" ...
```

- Parameters start with `/` followed by the name and a colon
- Values containing spaces must be enclosed in double quotes
- Parameter names are case-sensitive

## 📖 Detailed Guides

- **[Formatting Guide (Guide_Format.md)](https://github.com/TW-YTFeathered/SeanOne.Alchemy/blob/master/Guide_Format.md)** – Full documentation for `AlchemyFormatter`: DSL syntax, parameters, examples, and Fluent API.
- **[Conversion Guide (Guide_Convert.md)](https://github.com/TW-YTFeathered/SeanOne.Alchemy/blob/master/Guide_Convert.md)** – Full documentation for `AlchemyConverter`: sorting algorithms, temperature conversion, and combined usage.

## 📄 License

MIT License
