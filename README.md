# Alchemy for C\#

> **Data Alchemy** - Transform objects into formatted strings using concise DSL syntax

## Overview

Alchemy is a lightweight C# library that enables fast and flexible object-to-string transformation through custom DSL syntax.

## Core Concepts

- **Alchemy**: The brand name, representing the "transformation" process  
- **DSL**: The technical means of transformation (Domain-Specific Language)

---

## Quick Start

### Installation

```bash
dotnet add package SeanOne.Alchemy --version 2.0.0
```

### Supported .NET Versions

![Supported .NET Version](https://img.shields.io/badge/.NET%20Support-net6.0-blue)
![Supported .NET Version](https://img.shields.io/badge/.NET%20Support-net8.0-blue)
![Supported .NET Version](https://img.shields.io/badge/.NET%20Support-netstandard2.0-blue)
![Supported .NET Version](https://img.shields.io/badge/.NET%20Support-net472-blue)
![Supported .NET Version](https://img.shields.io/badge/.NET%20Support-net48-blue)

### Example Usage

```csharp
using SeanOne.Alchemy;

AlchemyFormatter.Format(5, "/tostring:F2 /end:!");
// Return: 5.00!

List<int> ints = Enumerable.Range(0, 10).ToList();

AlchemyFormatter.Format(ints, "fe /tostring:F2 /end:\\n /exclude-last-end:true");
/* Return:
0.00
1.00
2.00
3.00
4.00
5.00
6.00
7.00
8.00
9.00
*/
```

### Fluent API Example Usage

```csharp
using SeanOne.Alchemy.Builder;

AlchemyFormatBuilder.SelectBasic()
    .With(BasicParam.ToString, "F2")
    .With(BasicParam.End, "!")
    .Build()
    .Run(5);
// Return: 5.00!

List<int> ints = Enumerable.Range(0, 10).ToList();

AlchemyFormatBuilder.SelectFeSeq()
        .With(FeSeqParam.ToString, "F2")
        .With(FeSeqParam.End, "\\n")
        .With(FeSeqParam.ExcludeLastEnd, "true")
        .Build()
        .Run(ints);
/* Return:
0.00
1.00
2.00
3.00
4.00
5.00
6.00
7.00
8.00
9.00
*/
```

---

## Parameter Naming Conventions

The DSL and Fluent API adopt distinct naming styles, each tailored to their respective usage contexts.

> Note: DSL parameters use lowercase strings, and multi-word identifiers are written in kebab-case (with dashes -, not underscores _). This differs from snake_case and is intentional for DSL readability.  

### DSL: kebab-case or lowercase

> DSL parameters like `/tostring:F2` are designed to invoke C# built-in functions such as `ToString()`. In DSL syntax, these function names are intentionally written in lowercase (e.g., `tostring`) to simplify the grammar and distinguish them from user-defined functions. This lowercase convention applies specifically when referencing C# standard library methods—not their arguments like `"f2"` or `"yyyy-MM-dd"`, which are format strings.  

- Examples:
  - `/padleft:3`
  - `/upper-case:true`
  - `/dict-format:{0}=>{1}`

This naming style is suitable for quick inline formatting but lacks compile-time safety and IDE support.

### Fluent API: PascalCase via Enum

In Fluent API, all parameters are mapped to strongly-typed enums using PascalCase naming. This aligns with C# conventions and enables full IntelliSense support, compile-time checking, and semantic clarity.

Each `SelectXx()` method corresponds to a dedicated enum named `XxParam`, which defines all valid parameters for that formatting context.

| DSL Function                  | Fluent API Entry Point | Enum Container |
|-------------------------------|------------------------|----------------|
| `basic`                       | `SelectBasic()`        | `BasicParam`   |
| `fe` / `foreach` (sequence)   | `SelectFeSeq()`        | `FeSeqParam`   |
| `fe` / `foreach` (dictionary) | `SelectFeDict()`       | `FeDictParam`  |

### Naming Conversion Examples

> Note: The following table shows naming conversion examples.  
> Not all parameters may be implemented in the current release.  

| DSL Parameter          | Fluent API Enum Member          |
|------------------------|---------------------------------|
| `padleft`              | `FeSeqParam.PadLeft`            |
| `upper-case`           | `TextStyle.UpperCase`           |
| `dict-format`          | `FeDictParam.DictFormat`        |
| `key-format`           | `FeDictParam.KeyFormat`         |
| `value-format`         | `FeDictParam.ValueFormat`       |
| `exclude-last-end`     | `FeSeqParam.ExcludeLastEnd`     |
| `final-pair-separator` | `FeSeqParam.FinalPairSeparator` |

This naming convention ensures consistency, type safety, and a development experience that benefits from IDE tooling and C# language features.

---

## Escape Sequences

### Background

To ensure cross-platform compatibility and clarity in DSL instruction strings, Alchemy supports two escape sequence formats:

### Supported Escape Sequences

#### 1. **Unicode Escape Sequences** (`\uXXXX`)

- **Format**: `\u` followed by 4 hexadecimal digits
- **Purpose**: Represent any Unicode character
- **Advantages**:
  - Fully cross-platform
  - Supports all Unicode characters
  - Unambiguous parsing
- **Disadvantages**: Longer code, slightly less readable
- **Examples**:
  - `\u0020` → Space
  - `\u000A` → Line feed (LF) - platform independent
  - `\u000D` → Carriage return (CR)

#### 2. **Short Escape Sequences** (`\X`)

- **Format**: `\` followed by a single character
- **Currently supported**:
  - `\0` → Null character
  - `\a` → Alert/beep character
  - `\b` → Backspace
  - `\f` → Form feed
  - `\n` → **Platform-specific newline** (converted to `Environment.NewLine`)
  - `\r` → Carriage return
  - `\t` → Horizontal tab
  - `\v` → Vertical tab
  - `\\` → Backslash
  - `\'` → Single quote
  - `\"` → Double quote

### Important Notes on Line Breaks

#### **`\n` Behavior**

Unlike traditional escape sequences, Alchemy's `\n` is converted to `Environment.NewLine` at runtime. This means:

- **On Windows**: `\n` → `\r\n`
- **On Unix/Linux**: `\n` → `\n`
- **On macOS**: `\n` → `\n`

**Advantage**: Automatic platform adaptation - you don't need to worry about platform differences.

**Disadvantage**: Inconsistent output across platforms - the same DSL string may produce different output on different operating systems.

#### **For Consistent Cross-Platform Output**

If you need **identical output** across all platforms, use Unicode escape sequences:

- `\u000A` for LF (always `\n`)
- `\u000D` for CR (always `\r`)
- `\u000D\u000A` for CRLF (always `\r\n`)

### Design Considerations

#### **Why convert `\n` to `Environment.NewLine`?**

- **Developer convenience**: Most C# developers expect `\n` to behave like `Environment.NewLine`
- **Native .NET behavior**: Aligns with how C# handles newlines in string literals
- **Legacy compatibility**: Many existing applications rely on platform-specific line breaks

#### **Usage Recommendations**

1. **General use**: Use `\n` for newlines (recommended for most scenarios)
2. **Cross-platform consistency needed**: Use `\u000A` for LF or `\u000D\u000A` for CRLF
3. **Special characters**: Use the short escape sequences for common control characters
4. **Unicode characters**: Use `\uXXXX` for any Unicode character
5. **Literal backslash**: Use `\\`
6. **Quotes inside strings**: Use `\"` or `\'`

### Example Comparison

```csharp
// Using short escape sequence (platform-dependent newline)
AlchemyFormatter.Format(list, "fe /end:\\n");
// Windows output: "value1\r\nvalue2\r\nvalue3"
// Unix output: "value1\nvalue2\nvalue3"

// Using Unicode escape sequence (platform-independent newline)
AlchemyFormatter.Format(list, "fe /end:\\u000A");
// Always outputs: "value1\nvalue2\nvalue3"

// Using CRLF for Windows-style line breaks (platform-independent)
AlchemyFormatter.Format(list, "fe /end:\\u000D\\u000A");
// Always outputs: "value1\r\nvalue2\r\nvalue3"

// Traditional way (may cause issues)
AlchemyFormatter.Format(list, "fe /end:\r\n");
// Windows: works as expected
// Unix: outputs literal "\r\n" characters
```

### Notes

1. Escape sequences in DSL strings **only take effect in parameter values**
2. Parameter names (like `/tostring:`) do not support escape sequences
3. If a string value contains spaces or special characters, it is recommended to enclose it in double quotes
4. Unrecognized escape sequences (like `\x`) will be treated as literal characters (e.g., `\x` becomes `\x`)

---

*Note: The examples above use the DSL format string. For Fluent API, you can use regular C# string literals with standard escape sequences.*

> **Now that you understand how to represent special characters, let's explore the complete Alchemy syntax and parameters in detail below.**  

---

## Formatting with `AlchemyFormatter`

### Syntax

```text
[FunctionName] /param1:value1 /param2:"value with spaces" /param3:"value/with/slash" ...
```

- Parameters start with `/` followed by the name and a colon
- Values containing spaces must be enclosed in double quotes
- Parameter names are case-sensitive

### Common Functions

| Function Name     | Description                                                            |
|-------------------|------------------------------------------------------------------------|
| `fe` or `foreach` | Iterates over an `IEnumerable` or `IDictionary` and lists its contents |
| `basic`           | Default return method for formatting objects                           |

### Common Parameters

| Parameter Name | Example        | Description                                                                                                            |
|----------------|----------------|------------------------------------------------------------------------------------------------------------------------|
| `end`          | `/end:\\n`     | Appends a string after each value.                                                                                     |
| `tostring`     | `/tostring:F2` | Applies formatting to items implementing `IFormattable`. Not applicable to dictionaries. Use C#'s `ToString()` method. |
| `prefix`       | `/prefix:"["`  | Prepends a string before each value. Note: For collections, adds to entire result, not each element.                   |
| `suffix`       | `/suffix:"]"`  | Appends a string after each value. Note: For collections, adds to entire result, not each element.                     |

### Fe Parameters

#### Common Functions With Fe

| Parameter Name | Example        | Description                                                 |
|----------------|----------------|-------------------------------------------------------------|
| `fe-opt`       | `/fe-opt:true` | Enable optimized formatting (may have compatibility issues) |

#### IEnumerable-Specific Parameters

| Parameter Name         | Example                           | Description                                                                                              |
|------------------------|-----------------------------------|----------------------------------------------------------------------------------------------------------|
| `exclude-last-end`     | `/exclude-last-end:true`          | Omits the end string after the last item in the sequence. Applies to all `IEnumerable` types.            |
| `final-pair-separator` | `/final-pair-separator:" and "`   | Replaces the separator between the last two items in the sequence. Falls back to `end` if not specified. |

#### IDictionary-Specific Parameters

| Parameter Name | Example                 | Description                                                                                 |
|----------------|-------------------------|---------------------------------------------------------------------------------------------|
| `dict-format`  | `/dict-format:{0}=>{1}` | Format string for dictionary entries: `{0}` represents the key, `{1}` represents the value. |
| `key-format`   | `/key-format:F2`        | Format string applied to dictionary keys                                                    |
| `value-format` | `/value-format:F2`      | Format string applied to dictionary values                                                  |

---

### Functions Name

- `Format` – Formats an object into a string synchronously
- `FormatAsync` – Formats an object into a string asynchronously

## More Examples

For more detailed examples and advanced usage, see the [complete guide](https://github.com/TW-YTFeathered/SeanOne.Alchemy/blob/master/GUIDE.md#dsl-format-string-demonstration).

---

## Fluent API (Type-Safe Alternative)

For developers who prefer a more structured and type-safe approach, the library provides a Fluent API as an alternative to DSL format strings. This API offers better IDE support with IntelliSense and compile-time type checking.

### Available Functions

| Function         | Description                                                             |
|------------------|-------------------------------------------------------------------------|
| `SelectBasic()`  | Selects the basic formatting function for simple values                 |
| `SelectFeDict()` | Selects the foreach function specifically for dictionary types          |
| `SelectFeSeq()`  | Selects the foreach function specifically for sequence/enumerable types |

### Builder Methods

| Method               | Description                                                        |
|----------------------|--------------------------------------------------------------------|
| `With(param, value)` | Configures a formatting parameter                                  |
| `Build()`            | Creates a reusable formatter instance                              |
| `Run(obj)`           | Executes formatting on the specified object                        |
| `ToString()`         | Returns the DSL string representation of the configured formatter  |
| `BuildRun(obj)`      | Combines `Build()` and `Run(obj)` in one call                      |
| `BuildToString()`    | Combines `Build()` and `ToString()` to get the DSL string directly |

The Fluent API provides dedicated builders for different data types:

- **Basic types** - For simple value formatting using `SelectBasic()`
- **Collections** - For arrays, lists, and other enumerables using `SelectFeSeq()`  
- **Dictionaries** - For key-value pair formatting using `SelectFeDict()`

Each builder provides strongly-typed methods for configuring formatting options, eliminating the need to remember parameter names and reducing runtime errors.

See [GUIDE.md](https://github.com/TW-YTFeathered/SeanOne.Alchemy/blob/master/GUIDE.md#fluent-api-demonstration) for complete Fluent API examples and usage patterns.

---

## Issues and Support

Found a bug or have a feature request? Please open an issue on [GitHub Issues](https://github.com/TW-YTFeathered/SeanOne.Alchemy/issues).

---

## License

MIT License
