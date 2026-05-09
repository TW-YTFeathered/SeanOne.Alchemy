# Fluent API

The Fluent API provides a type‑safe, compile‑time checked alternative to DSL strings. It is available for formatting (`AlchemyFormatBuilder`) and partially for conversion (under development).

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
| `/tostring` | `BasicParam.ToString` or `FeSeqParam.ToString` |
| `/begin` | `FeSeqParam.Begin` |
| `/end` | `FeSeqParam.End` |
| `/exclude-last-end` | `FeSeqParam.ExcludeLastEnd` |
| `/final-pair-separator` | `FeSeqParam.FinalPairSeparator` |
| `/dict-format` | `FeDictParam.DictFormat` |
| `/key-format` | `FeDictParam.KeyFormat` |
| `/value-format` | `FeDictParam.ValueFormat` |

## Conversion Fluent API (Preview)

> ⚠️ **Status**: Under active development, targeting version 3.x. The API below is subject to change.

Planned usage:

```csharp
var converter = AlchemyConversionBuilder.Create()
    .Sort(SortAlgorithm.Bubble, descending: false)
    .ConvertTemperature(TemperatureUnit.Fahrenheit, TemperatureUnit.Celsius)
    .Build();

var result = converter.Run(data);
```

See `DesignDocs/` for current design drafts.

## Mixing DSL and Fluent API

You can build a formatter from DSL dynamically and then reuse it:

```csharp
var builder = AlchemyFormatBuilder.FromDsl("fe /tostring:F2 /end:\", \"");
var formatter = builder.Build();
string output = formatter.Run(myList);
```
