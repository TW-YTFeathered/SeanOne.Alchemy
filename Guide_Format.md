# Formatting Guide (Guide_Format.md)

> **Note:** This guide provides quick examples and detailed parameter reference for formatting with `AlchemyFormatter`.  
> **Quick Start:** The following are common usage scenarios. Each example includes the input object, DSL instruction, and expected output.  
> **Compatibility:** Examples use `C# 9` top‑level statements and are supported only in `.NET 5+` or `.NET Core 3.1+`; they cannot be compiled on `.NET Framework`.

## Table of contents

- [Parameter Reference](#parameter-reference)
  - [Naming Conventions](#naming-conventions)
  - [Escape Sequences](#escape-sequences)
  - [Common Parameters](#common-parameters)
  - [Fe Parameters](#fe-parameters)
    - [IEnumerable‑Specific Parameters](#ienumerable-specific-parameters)
    - [IDictionary‑Specific Parameters](#idictionary-specific-parameters)
- [DSL Format String Demonstration](#dsl-format-string-demonstration)
  - [Using `basic` functions](#using-basic-functions)
  - [`Fe` Common](#fe-common)
  - [IEnumerable demonstration](#ienumerable-demonstration)
  - [IDictionary demonstration](#idictionary-demonstration)
  - [Error scenarios](#error-scenarios)
- [Fluent API Demonstration](#fluent-api-demonstration)
  - [Basic usage with Fluent API](#basic-usage-with-fluent-api)
  - [Advanced Usage (Reusable)](#advanced-usage-reusable)

---

## Parameter Reference

### Naming Conventions

The DSL and Fluent API adopt distinct naming styles, each tailored to their respective usage contexts.

> Note: DSL parameters use lowercase strings, and multi-word identifiers are written in kebab-case (with dashes `-`, not underscores `_`). This differs from snake_case and is intentional for DSL readability.

#### DSL: kebab-case or lowercase

DSL parameters like `/tostring:F2` are designed to invoke C# built-in functions such as `ToString()`. In DSL syntax, these function names are intentionally written in lowercase (e.g., `tostring`) to simplify the grammar and distinguish them from user-defined functions. This lowercase convention applies specifically when referencing C# standard library methods—not their arguments like `"f2"` or `"yyyy-MM-dd"`, which are format strings.

- Examples:
  - `/padleft:3`
  - `/upper-case:true`
  - `/dict-format:{0}=>{1}`

This naming style is suitable for quick inline formatting but lacks compile-time safety and IDE support.

#### Fluent API: PascalCase via Enum

In Fluent API, all parameters are mapped to strongly-typed enums using PascalCase naming. This aligns with C# conventions and enables full IntelliSense support, compile-time checking, and semantic clarity.

Each `SelectXx()` method corresponds to a dedicated enum named `XxParam`, which defines all valid parameters for that formatting context.

| DSL Function                  | Fluent API Entry Point | Enum Container |
|-------------------------------|------------------------|----------------|
| `basic`                       | `SelectBasic()`        | `BasicParam`   |
| `fe` / `foreach` (sequence)   | `SelectFeSeq()`        | `FeSeqParam`   |
| `fe` / `foreach` (dictionary) | `SelectFeDict()`       | `FeDictParam`  |

#### Naming Conversion Examples

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

### Escape Sequences

#### Background

To ensure cross-platform compatibility and clarity in DSL instruction strings, Alchemy supports two escape sequence formats:

#### Supported Escape Sequences

##### 1. **Unicode Escape Sequences** (`\uXXXX`)

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

##### 2. **Short Escape Sequences** (`\X`)

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

#### Important Notes on Line Breaks

##### **`\n` Behavior**

Unlike traditional escape sequences, Alchemy's `\n` is converted to `Environment.NewLine` at runtime. This means:

- **On Windows**: `\n` → `\r\n`
- **On Unix/Linux**: `\n` → `\n`
- **On macOS**: `\n` → `\n`

**Advantage**: Automatic platform adaptation - you don't need to worry about platform differences.

**Disadvantage**: Inconsistent output across platforms - the same DSL string may produce different output on different operating systems.

##### **For Consistent Cross-Platform Output**

If you need **identical output** across all platforms, use Unicode escape sequences:

- `\u000A` for LF (always `\n`)
- `\u000D` for CR (always `\r`)
- `\u000D\u000A` for CRLF (always `\r\n`)

#### Design Considerations

##### **Why convert `\n` to `Environment.NewLine`?**

- **Developer convenience**: Most C# developers expect `\n` to behave like `Environment.NewLine`
- **Native .NET behavior**: Aligns with how C# handles newlines in string literals
- **Legacy compatibility**: Many existing applications rely on platform-specific line breaks

##### **Usage Recommendations**

1. **General use**: Use `\n` for newlines (recommended for most scenarios)
2. **Cross-platform consistency needed**: Use `\u000A` for LF or `\u000D\u000A` for CRLF
3. **Special characters**: Use the short escape sequences for common control characters
4. **Unicode characters**: Use `\uXXXX` for any Unicode character
5. **Literal backslash**: Use `\\`
6. **Quotes inside strings**: Use `\"` or `\'`

#### Example Comparison

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

#### Notes

1. Escape sequences in DSL strings **only take effect in parameter values**
2. Parameter names (like `/tostring:`) do not support escape sequences
3. If a string value contains spaces or special characters, it is recommended to enclose it in double quotes
4. Unrecognized escape sequences (like `\x`) will be treated as literal characters (e.g., `\x` becomes `\x`)

---

### Common Parameters

| Parameter Name | Example        | Description                                                                                                            |
|----------------|----------------|------------------------------------------------------------------------------------------------------------------------|
| `begin`        | `/begin:*`     | Prepends a string before each value.                                                                                   |
| `end`          | `/end:\\n`     | Appends a string after each value.                                                                                     |
| `tostring`     | `/tostring:F2` | Applies formatting to items implementing `IFormattable`. Not applicable to dictionaries. Use C#'s `ToString()` method. |
| `prefix`       | `/prefix:"["`  | Prepends a string before the entire result.                                                                            |
| `suffix`       | `/suffix:"]"`  | Appends a string after the entire result.                                                                              |

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

## DSL Format String Demonstration

### DSL Format String Demonstration directory

- [Home](#table-of-contents)
- [Using `basic` functions](#using-basic-functions)
- [`Fe` Common](#fe-common)
- [IEnumerable demonstration](#ienumerable-demonstration)
- [IDictionary demonstration](#idictionary-demonstration)
- [Error scenarios](#error-scenarios)

---

### Using `basic` functions

```csharp
using SeanOne.Alchemy;

AlchemyFormatter.Format(1, "/tostring:f2");
// Return: 1.00
```

### `Fe` Common

- [DSL Format String Demonstration directory](#dsl-format-string-demonstration-directory)

> **Performance Tip:** Use `/fe-opt:true` for large collections. This enables an optimized formatter that's ~1.5x faster but may have compatibility issues with some custom collections.  

```csharp
using SeanOne.Alchemy;

List<int> ints = Enumerable.Range(0, 10).ToList();

AlchemyFormatter.Format(ints, "fe /fe-opt:true /tostring:f2 /end:\", \" /final-pair-separator:\" and \" /exclude-last-end:true");
/* Return:
0.00, 1.00, 2.00, 3.00, 4.00, 5.00, 6.00, 7.00, 8.00 and 9.00
*/
```

### IEnumerable demonstration

- [DSL Format String Demonstration directory](#dsl-format-string-demonstration-directory)

#### `DateTime`

- The following example shows how to format a collection of dates to a specific format and use a custom separator string:

```csharp
using SeanOne.Alchemy;

List<DateTime> dates = Enumerable.Range(1, 12).Select(month => new DateTime(2024, month, 1)).ToList();

AlchemyFormatter.Format(dates, "fe /tostring:yyyy-MM-dd /end:\\n");
/* Return:
2024-01-01
2024-02-01
2024-03-01
2024-04-01
2024-05-01
2024-06-01
2024-07-01
2024-08-01
2024-09-01
2024-10-01
2024-11-01
2024-12-01

*/
```

#### `int`

- The following example shows how to format a collection of integers to two decimal places and use a custom concatenation string:

```csharp
using SeanOne.Alchemy;

List<int> ints = Enumerable.Range(0, 10).ToList();

AlchemyFormatter.Format(ints, "fe /tostring:F2 /end:\", \" /final-pair-separator:\" and \" /exclude-last-end:true");
// Return: 0.00, 1.00, 2.00, 3.00, 4.00, 5.00, 6.00, 7.00, 8.00 and 9.00
```

### IDictionary demonstration

- [DSL Format String Demonstration directory](#dsl-format-string-demonstration-directory)

#### `Guid`, `DateTimeOffset`

- The following example shows how to format a dictionary of GUIDs and DateTimeOffsets to a specific format and use a custom separator string:

```csharp
using SeanOne.Alchemy;

Dictionary<Guid, DateTimeOffset> dict = Enumerable.Range(1, 12)
    .ToDictionary(
        _ => Guid.NewGuid(),
        month => new DateTimeOffset(2024, month, 1, 0, 0, 0, TimeSpan.Zero)
    );

AlchemyFormatter.Format(dict, "fe /dict-format:{1} /value-format:\"yyyy-MM-dd ddd HH:mm:ss zz\" /end:\\n");
/* Return:
2024-01-01 Mon 00:00:00 +00
2024-02-01 Thu 00:00:00 +00
2024-03-01 Fri 00:00:00 +00
2024-04-01 Mon 00:00:00 +00
2024-05-01 Wed 00:00:00 +00
2024-06-01 Sat 00:00:00 +00
2024-07-01 Mon 00:00:00 +00
2024-08-01 Thu 00:00:00 +00
2024-09-01 Sun 00:00:00 +00
2024-10-01 Tue 00:00:00 +00
2024-11-01 Fri 00:00:00 +00
2024-12-01 Sun 00:00:00 +00

*/
```

#### `int`, `string`

- The following example shows how to format a dictionary of integers and strings to a specific format and use a custom separator string:

```csharp
using SeanOne.Alchemy;

Dictionary<int, string> dict = Enumerable.Range(0, 10)
    .ToDictionary(
        i => i,
        _ => $"Number"
    );

AlchemyFormatter.Format(dict, "fe /dict-format:{1}>\\u0020{0}\\u0020 /key-format:F2");
// Return: Number> 0.00 Number> 1.00 Number> 2.00 Number> 3.00 Number> 4.00 Number> 5.00 Number> 6.00 Number> 7.00 Number> 8.00 Number> 9.00
```

---

### Error scenarios

- [DSL Format String Demonstration directory](#dsl-format-string-demonstration-directory)

#### Error scenarios directory

- [Home](#table-of-contents)
- [Incorrect: `obj` or `dslInstruction` is null or empty](#incorrect-obj-or-dslinstruction-is-null-or-empty)
- [Incorrect: Parameter appears multiple times](#incorrect-parameter-appears-multiple-times)
- [Incorrect: Non-existent parameters](#incorrect-non-existent-parameters)
- [Incorrect type for `/tostring`](#incorrect-type-for-tostring)
- [Incorrect Function Name in DSL Instruction](#incorrect-function-name-in-dsl-instruction)
- [Incorrect: `fe` / `foreach` usage](#incorrect-fe--foreach-usage)

#### Incorrect: `obj` or `dslInstruction` is null or empty

- [Error scenarios directory](#error-scenarios-directory)

```csharp
using SeanOne.Alchemy;

// ❌ Incorrect: obj is null
// Throw: System.ArgumentNullException: 'Value cannot be null. (Parameter 'Input object must not be null.')'
AlchemyFormatter.Format(null, "/tostring:f2");

// ❌ Incorrect: dslInstruction is null or empty
// Throw: System.ArgumentNullException: 'Value cannot be null. (Parameter 'Alchemy instruction cannot be null or empty')'
AlchemyFormatter.Format(5, null);
AlchemyFormatter.Format(5, "");

// ✅ Correct: obj and dslInstruction are not null
AlchemyFormatter.Format(5, "/tostring:f2");
// Return: 5.00
```

#### Incorrect: Parameter appears multiple times

- [Error scenarios directory](#error-scenarios-directory)

```csharp
using SeanOne.Alchemy;

// ❌ Incorrect: Parameter '/tostring:' is specified multiple times
// Throw: System.ArgumentException: 'Parameter '/tostring:' is specified multiple times.'
AlchemyFormatter.Format(5, "/tostring:f2 /tostring:F3");

// ✅ Correct: Parameter '/tostring:' is specified only once
AlchemyFormatter.Format(5, "/tostring:f2");
// Return: 5.00
```

#### Incorrect: Non-existent parameters

- [Error scenarios directory](#error-scenarios-directory)

```csharp
using SeanOne.Alchemy;

// ❌ Incorrect: Unknown parameter: ts
// Throw: System.ArgumentException: 'Invalid parameters for basic processing: ts'
AlchemyFormatter.Format(5, "/ts:F2 /end:!");

// ✅ Correct: Known parameter: tostring
AlchemyFormatter.Format(5, "/tostring:F2 /end:!");
// Return: 5.00!

// ❌ Incorrect: Unknown parameter: ls
// Throw: System.ArgumentException: 'Invalid parameters for enumerable processing: ls' 
var list = Enumerable.Range(1,10).ToList();
AlchemyFormatter.Format(list, "fe /ls:F2 /end:\\u0020");

// ✅ Correct: Known parameter: tostring
AlchemyFormatter.Format(list, "fe /tostring:F2 /end:\\u0020");
// Return: 1.00 2.00 3.00 4.00 5.00 6.00 7.00 8.00 9.00 10.00

// ❌ Incorrect: Unknown parameter: df
// Throw: System.ArgumentException: 'Invalid parameters for dictionary processing: df' 
var dict = Enumerable.Range(1,10)
    .ToDictionary(
    i => i,
    i => Math.PI * i
    );
AlchemyFormatter.Format(dict, "fe /df:{0}=>{1} /value-format:F2 /end:\\u0020");

// ✅ Correct: Known parameter: dict-format
AlchemyFormatter.Format(dict, "fe /dict-format:{0}=>{1} /value-format:F2 /end:\\u0020");
// Return: 1=>3.14 2=>6.28 3=>9.42 4=>12.57 5=>15.71 6=>18.85 7=>21.99 8=>25.13 9=>28.27 10=>31.42
```

#### Incorrect type for `/tostring`

- [Error scenarios directory](#error-scenarios-directory)

```csharp
using SeanOne.Alchemy;

// ❌ Incorrect: string does not implement IFormattable
// Throw: System.ArgumentException: 'Collection elements must implement IFormattable for 'tostring'. Found: String'
AlchemyFormatter.Format("5", "/tostring:f2");

// ✅ Correct: int implements IFormattable
AlchemyFormatter.Format(5, "/tostring:f2");
// Return: 5.00
```

#### Incorrect Function Name in DSL Instruction

- [Error scenarios directory](#error-scenarios-directory)

```csharp
using SeanOne.Alchemy;

List<int> ints = Enumerable.Range(0, 10).ToList();

// ❌ Incorrect: Unknown functions directive: loop
// Throw: System.MissingMethodException: 'Unknown functions directive: loop'
AlchemyFormatter.Format(ints, "loop /tostring:f2 /end:\\u0020");

// ✅ Correct: Known functions directive: foreach
AlchemyFormatter.Format(ints, "foreach /tostring:f2 /end:\\u0020");
// Return: 0.00 1.00 2.00 3.00 4.00 5.00 6.00 7.00 8.00 9.00
```

---

#### Incorrect: `fe` / `foreach` usage

- [Error scenarios directory](#error-scenarios-directory)

##### Incorrect: type is non-`IEnumerable` or type is `string`

```csharp
using SeanOne.Alchemy;

List<int> ints = Enumerable.Range(0, 10).ToList();

// ❌ Incorrect: fe directive is used with a collection of strings
// Throw: System.ArgumentException: 'String is not supported for 'fe' directive'
AlchemyFormatter.Format("ints", "fe /end:\\u0020");

// ❌ Incorrect: fe directive is used with a non-IEnumerable type
// Throw: System.ArgumentException: 'Object must implement IEnumerable for 'fe' directive'
AlchemyFormatter.Format(5, "fe /tostring:f2 /end:\\u0020");

// ✅ Correct: fe directive is used with a collection of integers
AlchemyFormatter.Format(ints, "fe /tostring:f2 /end:\\u0020");
// Return: 0.00 1.00 2.00 3.00 4.00 5.00 6.00 7.00 8.00 9.00
```

##### Incorrect: `dict-format` is null or empty

```csharp
using SeanOne.Alchemy;

var dict = Enumerable.Range(1, 10)
    .ToDictionary(i => i, i => $"Value {i}");

// ❌ Incorrect: dict-format is null
// Throw System.ArgumentNullException: 'Value cannot be null. (Parameter ''dict-format' parameter is required when processing dictionaries.')' 
AlchemyFormatter.Format(dict, "fe /end:\\u0020");

// ❌ Incorrect: dict-format is empty
// Throw: System.ArgumentNullException: 'Value cannot be null. (Parameter ''dict-format' parameter is required when processing dictionaries.')' 
AlchemyFormatter.Format(dict, "fe /dict-format: /end:\\u0020");

// ✅ Correct: dict-format is not null or empty
AlchemyFormatter.Format(dict, "fe /dict-format:{0} /end:\\u0020");
// Return: 1 2 3 4 5 6 7 8 9 10
```

---

## Fluent API Demonstration

> **Note:** Fluent API provides the same functionality as DSL format strings, but with better IDE support and compile-time type safety.

### Fluent API Demonstration directory

- [Home](#table-of-contents)
- [Basic usage with Fluent API](#basic-usage-with-fluent-api)
- [Advanced Usage (Reusable)](#advanced-usage-reusable)

---

### Basic usage with Fluent API

```csharp
using SeanOne.Alchemy.Builder;

AlchemyFormatBuilder.SelectBasic()
    .With(BasicParam.ToString, "f2")
    .BuildRun(5);
// Return: 5.00
```

### Advanced Usage (Reusable)

- [Fluent API Demonstration directory](#fluent-api-demonstration-directory)

```csharp
using SeanOne.Alchemy.Builder;

var formatter = AlchemyFormatBuilder.SelectBasic()
    .With(BasicParam.End, "!")
    .With(BasicParam.ToString, "f2")
    .Build();

formatter.Run(5);
// Return: 5.00!
formatter.Run(12);
// Return: 12.00!
```
