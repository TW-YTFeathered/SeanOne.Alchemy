# GUIDE.md

> **Note:** This guide provides quick examples. For complete parameter details, see the main `README.md`.  
> **Quick Start:** The following are common usage scenarios. Each example includes the input object, DSL instruction, and expected output.  
> **Compatibility:** Examples use `C# 9` top‑level statements and are supported only in `.NET 5+` or `.NET Core 3.1+`; they cannot be compiled on `.NET Framework`.

## Table of contents

- [Example Format string](#dsl-format-string-demonstration)
- [Fluent API Demonstration](#fluent-api-demonstration)

## DSL Format String Demonstration

### DSL Format String Demonstration directory

- [Home](#table-of-contents)
- [Using `basic` functions](#using-basic-functions)
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

// Incorrect: obj is null
// Throw: System.ArgumentNullException: 'Value cannot be null. (Parameter 'Input object must not be null.')'
AlchemyFormatter.Format(null, "/tostring:f2");

// Incorrect: dslInstruction is null or empty
// Throw: System.ArgumentNullException: 'Value cannot be null. (Parameter 'Alchemy instruction cannot be null or empty')'
AlchemyFormatter.Format(5, null);
AlchemyFormatter.Format(5, "");

// Correct: obj and dslInstruction are not null
AlchemyFormatter.Format(5, "/tostring:f2");
// Return: 5.00
```

#### Incorrect: Parameter appears multiple times

- [Error scenarios directory](#error-scenarios-directory)

```csharp
using SeanOne.Alchemy;

// Incorrect: Parameter '/tostring:' is specified multiple times
// Throw: System.ArgumentException: 'Parameter '/tostring:' is specified multiple times.'
AlchemyFormatter.Format(5, "/tostring:f2 /tostring:F3");

// Correct: Parameter '/tostring:' is specified only once
AlchemyFormatter.Format(5, "/tostring:f2");
// Return: 5.00
```

#### Incorrect: Non-existent parameters

- [Error scenarios directory](#error-scenarios-directory)

```csharp
using SeanOne.Alchemy;

// Incorrect: Unknown parameter: ts
// Throw: System.ArgumentException: 'Invalid parameters for basic processing: ts'
AlchemyFormatter.Format(5, "/ts:F2 /end:!");

// Correct: Known parameter: tostring
AlchemyFormatter.Format(5, "/tostring:F2 /end:!");
// Return: 5.00!

// Incorrect: Unknown parameter: ls
// Throw: System.ArgumentException: 'Invalid parameters for enumerable processing: ls' 
var list = Enumerable.Range(1,10).ToList();
AlchemyFormatter.Format(list, "fe /ls:F2 /end:\\u0020");

// Correct: Known parameter: tostring
AlchemyFormatter.Format(list, "fe /tostring:F2 /end:\\u0020");
// Return: 1.00 2.00 3.00 4.00 5.00 6.00 7.00 8.00 9.00 10.00

// Incorrect: Unknown parameter: df
// Throw: System.ArgumentException: 'Invalid parameters for dictionary processing: df' 
var dict = Enumerable.Range(1,10)
    .ToDictionary(
    i => i,
    i => Math.PI * i
    );
AlchemyFormatter.Format(dict, "fe /df:{0}=>{1} /value-format:F2 /end:\\u0020");

// Correct: Known parameter: dict-format
AlchemyFormatter.Format(dict, "fe /dict-format:{0}=>{1} /value-format:F2 /end:\\u0020");
// Return: 1=>3.14 2=>6.28 3=>9.42 4=>12.57 5=>15.71 6=>18.85 7=>21.99 8=>25.13 9=>28.27 10=>31.42
```

#### Incorrect type for `/tostring`

- [Error scenarios directory](#error-scenarios-directory)

```csharp
using SeanOne.Alchemy;

// Incorrect: string does not implement IFormattable
// Throw: System.ArgumentException: 'Collection elements must implement IFormattable for 'tostring'. Found: String'
AlchemyFormatter.Format("5", "/tostring:f2");

// Correct: int implements IFormattable
AlchemyFormatter.Format(5, "/tostring:f2");
// Return: 5.00
```

#### Incorrect Function Name in DSL Instruction

- [Error scenarios directory](#error-scenarios-directory)

```csharp
using SeanOne.Alchemy;

List<int> ints = Enumerable.Range(0, 10).ToList();

// Incorrect: Unknown functions directive: loop
// Throw: System.MissingMethodException: 'Unknown functions directive: loop'
AlchemyFormatter.Format(ints, "loop /tostring:f2 /end:\\u0020");

// Correct: Known functions directive: foreach
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

// Incorrect: fe directive is used with a collection of strings
// Throw: System.ArgumentException: 'String is not supported for 'fe' directive'
AlchemyFormatter.Format("ints", "fe /end:\\u0020");

// Incorrect: fe directive is used with a non-IEnumerable type
// Throw: System.ArgumentException: 'Object must implement IEnumerable for 'fe' directive'
AlchemyFormatter.Format(5, "fe /tostring:f2 /end:\\u0020");

// Correct: fe directive is used with a collection of integers
AlchemyFormatter.Format(ints, "fe /tostring:f2 /end:\\u0020");
// Return: 0.00 1.00 2.00 3.00 4.00 5.00 6.00 7.00 8.00 9.00
```

##### Incorrect: `dict-format` is null or empty

```csharp
using SeanOne.Alchemy;

var dict = Enumerable.Range(1, 10)
    .ToDictionary(i => i, i => $"Value {i}");

// Incorrect: dict-format is null
// Throw System.ArgumentNullException: 'Value cannot be null. (Parameter ''dict-format' parameter is required when processing dictionaries.')' 
AlchemyFormatter.Format(dict, "fe /end:\\u0020");

// Incorrect: dict-format is empty
// Throw: System.ArgumentNullException: 'Value cannot be null. (Parameter ''dict-format' parameter is required when processing dictionaries.')' 
AlchemyFormatter.Format(dict, "fe /dict-format: /end:\\u0020");

// Correct: dict-format is not null or empty
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
