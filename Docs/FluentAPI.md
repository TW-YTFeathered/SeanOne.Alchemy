# Fluent API

The Fluent API provides a type‑safe, compile‑time checked alternative to DSL strings. It is available for formatting (`AlchemyFormatBuilder`) and partially for conversion (Beta).

## Basic Usage (Formatting)

```csharp
using SeanOne.Alchemy.Builder;

// One‑shot execution
string result = AlchemyFormatBuilder.SelectBasic()
    .With(BasicParam.ToString, "F2")
    .With(BasicParam.End, "!")
    .BuildRun(5);
// result = "5.00!"
```

## Reusable Formatter

```csharp
var formatter = AlchemyFormatBuilder.SelectBasic()
    .With(BasicParam.End, "!")
    .With(BasicParam.ToString, "F2")
    .Build();

formatter.Run(5);   // "5.00!"
formatter.Run(12);  // "12.00!"
```

## SelectXx Methods

| DSL Function | Fluent Entry Point | Enum Type |
|--------------|--------------------|-----------|
| `basic` | `SelectBasic()` | `BasicParam` |
| `fe` (sequence) | `SelectFeSeq()` | `FeSeqParam` |
| `fe` (dictionary) | `SelectFeDict()` | `FeDictParam` |

### Enum Members Examples

| DSL Parameter | Fluent Enum Member |
|---------------|--------------------|
| `tostring` | `BasicParam.ToString` or `FeSeqParam.ToString` |
| `end` | `FeSeqParam.End` |
| `exclude-last-end` | `FeSeqParam.ExcludeLastEnd` |
| `final-pair-separator` | `FeSeqParam.FinalPairSeparator` |
| `dict-format` | `FeDictParam.DictFormat` |
| `key-format` | `FeDictParam.KeyFormat` |
| `value-format` | `FeDictParam.ValueFormat` |

Below is the revised **Conversion Fluent API (Beta)** section in English, written to align with the current codebase (no `AlchemyConverter`, use `Alchemy.Transform` for execution). The API is marked as Beta and subject to change.

## Conversion Fluent API (Beta)

> ⚠️ **Status**: Beta feature – available only when building with the `BETA` compilation symbol (e.g., select `Beta` configuration in Visual Studio). The API described below may change in future releases.

The Fluent API for conversions allows you to build `cnv` instructions in a type‑safe, compile‑time checked way.

### Building a Conversion Instruction

```csharp
using SeanOne.Alchemy.Builder;

var executable = AlchemyBuilder.SelectCnv()
    .With(CnvParam.Sort, "is")        // insertion sort, ascending
    .With(CnvParam.Temp, "C->F")      // Celsius to Fahrenheit
    .Build();

string dsl = executable.ToString();   // "cnv /sort:is /temp:C->F"
```

### Executing the Conversion

The `AlchemyExecutable` returned by `Build()` provides two ways to execute the conversion:

#### 1. Using `RunAsTransform()` (Beta, recommended for pipelines)

```csharp
using SeanOne.Alchemy;

var data = new List<double> { 0, 100, 25 };
AlchemyResult result = executable.RunAsTransform(data);
double first = result.ToObject<List<double>>()[0];
```

> `RunAsTransform` executes **all** instructions stored in the executable and returns an `AlchemyResult`.

#### 2. Using `Alchemy.Transform` with the DSL string(s)

```csharp
AlchemyResult result = Alchemy.Transform(data, executable.ToString());
// Or for multiple instructions:
AlchemyResult result = Alchemy.Transform(data, executable.GetDsls());
```

### Pipeline Builder (Beta)

Combine multiple instructions into a pipeline:

```csharp
var pipeline = AlchemyBuilder.CreatePipeline()
    .Add(AlchemyBuilder.SelectCnv().With(CnvParam.Sort, "as"))
    .Add(AlchemyBuilder.SelectCnv().With(CnvParam.Temp, "F->C"))
    .Build();

// Execute all instructions in sequence
var finalResult = pipeline.RunAsTransform(originalData);
```

> **Note**: The legacy `Run` method (used for formatting) only processes the first instruction and returns a string. For conversion pipelines, always use `RunAsTransform` (Beta) or `Alchemy.Transform`.
