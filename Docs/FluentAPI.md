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

## Conversion Fluent API (Beta)

> ⚠️ **Status**: Beta feature – available when building with the `Beta` configuration (e.g., select `Beta` in Visual Studio). The API below is subject to change.

```csharp
// Example (using Beta configuration)
var converter = AlchemyBuilder.SelectCnv()
    .With(CnvParam.Sort, "bubble")
    .With(CnvParam.Temp, "C->F")
    .Build();

var result = converter.RunAsConvert(data);
```

## Pipeline Builder (Beta)

Combine multiple instructions into a pipeline (Beta configuration only):

```csharp
var pipeline = AlchemyBuilder.CreatePipeline()
    .Add(AlchemyBuilder.SelectBasic().With(BasicParam.Prefix, ">>"))
    .Add(AlchemyBuilder.SelectFeSeq().With(FeSeqParam.End, "!"))
    .Build();

var output = pipeline.RunAsConvert(myObject);
```
