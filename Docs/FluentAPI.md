# Fluent API

The Fluent API builds Alchemy DSL instructions with enums, typed helper methods, and IntelliSense instead of hand-written DSL strings. It is implemented in the `SeanOne.Alchemy.Builder` namespace.

The current API is centered on:

- `AlchemyBuilder` – creates builders and assembles pipelines.
- Typed function builders – `BasicFunctionBuilder`, `FeSequenceFunctionBuilder`, `FeDictionaryFunctionBuilder`, `CnvFunctionBuilder`, and `ArrFunctionBuilder`.
- `AlchemyExecutable` – stores one or more finished DSL instructions and executes them.

> ⚠️ The legacy `AlchemyFormatBuilder` API is obsolete. It is covered in [Legacy API and migration](#legacy-api-and-migration) at the end of this page.

## Quick Start

```csharp
using SeanOne.Alchemy;
using SeanOne.Alchemy.Builder;

string result = AlchemyBuilder.SelectBasic()
    .WithToString("F2")
    .WithPrefix("Total: ")
    .Execute(99.5)
    .GetString();

// result == "Total: 99.50"
```

`Execute` returns an `AlchemyResult`. Use `GetString()` for formatted text, or `ToObject<T>()` / `ToList<T>()` when the result is a converted object.

## Reusable Executable

`Build()` finalizes the current instruction and returns an `AlchemyExecutable`. The executable can be reused any number of times.

```csharp
using SeanOne.Alchemy.Builder;

AlchemyExecutable formatter = AlchemyBuilder.SelectBasic()
    .WithEnd("!")
    .WithToString("F2")
    .Build();

formatter.Execute(5).GetString();    // "5.00!"
formatter.Execute(12.5).GetString(); // "12.50!"
```

`Build()` is equivalent to `Step().Build()`. You normally do not need to call `Step()` yourself.

## Inspecting the Generated DSL

Function builders provide `ToString()` to return the DSL instruction they emitted. Calling `ToString()` also finalizes the stage.

```csharp
using SeanOne.Alchemy.Builder;

string dsl = AlchemyBuilder.SelectBasic()
    .WithToString("F2")
    .ToString();

// dsl == "basic /tostring:\"F2\""
```

For an `AlchemyExecutable`, `ToString()` joins all stored instructions with `" | "` for display:

```csharp
var pipeline = AlchemyBuilder.SelectArr()
    .WithSort(SortAlgorithm.InsertionAsc)
    .ThenCnv()
    .WithTemp(TempUnit.F, TempUnit.C)
    .Build();

pipeline.ToString();
// "arr /sort:\"is\" | cnv /temp:\"F->C\""
```

> ℹ️ The `" | "` separator is for display only. It is not a single DSL instruction that can be passed back to `Alchemy.Format` or `Alchemy.Transform`. Use `executable.Execute(obj)` to run a pipeline.

## Entry Points

| Method | Builder | DSL function | Parameter enum |
|--------|---------|--------------|----------------|
| `AlchemyBuilder.SelectBasic()` | `BasicFunctionBuilder` | `basic` | `BasicParam` |
| `AlchemyBuilder.SelectFeSeq()` | `FeSequenceFunctionBuilder` | `fe` | `FeSeqParam` |
| `AlchemyBuilder.SelectFeDict()` | `FeDictionaryFunctionBuilder` | `fe` | `FeDictParam` |
| `AlchemyBuilder.SelectCnv()` | `CnvFunctionBuilder` | `cnv` | `CnvParam` |
| `AlchemyBuilder.SelectArr()` | `ArrFunctionBuilder` | `arr` | `ArrParam` |
| `AlchemyBuilder.Create()` | `AlchemyBuilder` | – | – |

`SelectXxx()` is shorthand for `Create().ThenXxx()`:

```csharp
AlchemyBuilder.SelectBasic();
// is equivalent to:
AlchemyBuilder.Create().ThenBasic();
```

## How a Builder Works

A function builder collects parameters for **one** DSL instruction.

```csharp
AlchemyBuilder.SelectBasic()
    .WithBegin("[")
    .WithToString("F2")
    .WithEnd("]");
```

This produces:

```text
basic /begin:"["/tostring:"F2"/end:"]"
```

Parameter order follows the order of the `With...` calls.

Each `With...` method:

- Translates the enum member to its DSL parameter name.
- Wraps the value in double quotes.
- Escapes embedded double quotes as `\u0022`.
- Returns the same builder for chaining.

On a function builder, the current stage is committed when one of these is called:

| Call | Effect |
|------|--------|
| `Build()` | Finalizes the stage and returns an `AlchemyExecutable`. |
| `Execute(obj)` | Finalizes, builds, and runs the full pipeline against `obj`. |
| `ToString()` | Finalizes and returns the instruction string. |
| `ThenXxx()` | Finalizes the current stage and starts the next stage. |
| `Step()` | Finalizes the current stage and returns the parent `AlchemyBuilder`. |

After a stage has been finalized, adding another parameter to that same builder throws `InvalidOperationException` (see [Error Handling](ErrorHandling.md#fluent-api-builder-exceptions)):

```csharp
var builder = AlchemyBuilder.SelectBasic().WithToString("F2");
builder.Build();

builder.WithEnd("!"); // InvalidOperationException
```

Start a new builder with `ThenXxx()` or `AlchemyBuilder.SelectXxx()` instead.

> ℹ️ A stage with no parameters is silently skipped. If an executable ends up with no instructions at all, executing it throws an `ArgumentNullException` (see [Error Handling](ErrorHandling.md#fluent-api-builder-exceptions)).

## Basic Formatting

`BasicFunctionBuilder` builds `basic` instructions for a single object.

### Parameter enum

| `BasicParam` | DSL parameter | Helper method |
|--------------|---------------|---------------|
| `Begin` | `begin` | `WithBegin(string)` |
| `End` | `end` | `WithEnd(string)` |
| `ToString` | `tostring` | `WithToString(string)` |
| `Prefix` | `prefix` | `WithPrefix(string)` |
| `Suffix` | `suffix` | `WithSuffix(string)` |

The output order is:

```text
prefix + begin + formatted value + end + suffix
```

### Example

```csharp
using SeanOne.Alchemy;
using SeanOne.Alchemy.Builder;

string result = AlchemyBuilder.SelectBasic()
    .WithToString("F2")
    .WithPrefix("Result: ")
    .WithBegin("(")
    .WithEnd(")")
    .WithSuffix("!")
    .Execute(5)
    .GetString();

// "Result: (5.00)!"
```

`With(BasicParam.ToString, "F2")` is also available and produces the same parameter; the typed helper methods are simply more discoverable.

## Sequence Formatting

`FeSequenceFunctionBuilder` builds `fe` instructions for `IEnumerable` sequences.

### Parameter enum

| `FeSeqParam` | DSL parameter | Helper method |
|--------------|---------------|---------------|
| `Begin` | `begin` | `WithBegin(string)` |
| `End` | `end` | `WithEnd(string)` |
| `ExcludeLastEnd` | `exclude-last-end` | `WithExcludeLastEnd(bool flag = true)` |
| `FinalPairSeparator` | `final-pair-separator` | `WithFinalPairSeparator(string)` |
| `ToString` | `tostring` | `WithToString(string)` |
| `Prefix` | `prefix` | `WithPrefix(string)` |
| `Suffix` | `suffix` | `WithSuffix(string)` |
| `FeOpt` | `fe-opt` | `WithFeOpt(bool flag = true)` |

`WithExcludeLastEnd()` and `WithFeOpt()` default to `true`. Pass `false` to emit `"false"` explicitly.

### Example

```csharp
using SeanOne.Alchemy;
using SeanOne.Alchemy.Builder;

var numbers = new List<double> { 1.0, 2.5, 3.14 };

string result = AlchemyBuilder.SelectFeSeq()
    .WithToString("F2")
    .WithEnd(", ")
    .WithFinalPairSeparator(" and ")
    .WithExcludeLastEnd()
    .Execute(numbers)
    .GetString();

// "1.00, 2.50 and 3.14"
```

See [Formatting Collections](Format/Collections.md) for the DSL semantics of each parameter.

## Dictionary Formatting

`FeDictionaryFunctionBuilder` also builds `fe` instructions, but exposes dictionary-specific parameters.

### Parameter enum

| `FeDictParam` | DSL parameter | Helper method |
|---------------|---------------|---------------|
| `DictFormat` | `dict-format` | `WithDictFormat(string)` |
| `Begin` | `begin` | `WithBegin(string)` |
| `End` | `end` | `WithEnd(string)` |
| `ExcludeLastEnd` | `exclude-last-end` | `WithExcludeLastEnd(bool flag = true)` |
| `FinalPairSeparator` | `final-pair-separator` | `WithFinalPairSeparator(string)` |
| `KeyFormat` | `key-format` | `WithKeyFormat(string)` |
| `ValueFormat` | `value-format` | `WithValueFormat(string)` |
| `Prefix` | `prefix` | `WithPrefix(string)` |
| `Suffix` | `suffix` | `WithSuffix(string)` |
| `FeOpt` | `fe-opt` | `WithFeOpt(bool flag = true)` |

`dict-format` is required at execution time. The builder does not enforce this when the parameter is omitted; the error is raised by `Alchemy.Format` / `Alchemy.Transform`.

### Example

```csharp
using SeanOne.Alchemy;
using SeanOne.Alchemy.Builder;

var dict = new Dictionary<int, string>
{
    [1] = "one",
    [2] = "two"
};

string result = AlchemyBuilder.SelectFeDict()
    .WithDictFormat("{0}={1}")
    .WithEnd(", ")
    .WithExcludeLastEnd()
    .Execute(dict)
    .GetString();

// "1=one, 2=two"
```

See [Formatting Dictionaries](Format/Dictionaries.md) for `{0}` / `{1}` formatting and the dictionary-specific rules.

## Unit Conversions

`CnvFunctionBuilder` builds `cnv` instructions. Prefer the typed unit helpers over raw strings: they convert the enum values to the unit codes understood by the DSL.

### Conversion parameters

| `CnvParam` | DSL parameter | Typed helper |
|------------|---------------|--------------|
| `Temp` | `temp` | `WithTemp(TempUnit from, TempUnit to)` |
| `Length` | `length` | `WithLength(LengthUnit from, LengthUnit to)` |
| `Weight` | `weight` | `WithWeight(WeightUnit from, WeightUnit to)` |

The unit enums live in `SeanOne.Alchemy.Builder`:

| Enum | Members |
|------|---------|
| `TempUnit` | `C`, `F`, `K` |
| `LengthUnit` | `A`, `NM`, `UM`, `MM`, `CM`, `M`, `KM`, `IN`, `FT`, `YD`, `MI`, `NMI` |
| `WeightUnit` | `Mg`, `Cg`, `Dg`, `G`, `Dag`, `Hg`, `Kg`, `T`, `Oz`, `Lb`, `St`, `ShortTon`, `LongTon` |

The helpers generate values such as `"C->F"`, `"CM->IN"`, and `"Kg->G"`.

> ℹ️ If the source and target units are the same, the typed helper returns the builder unchanged and does not emit a parameter. Raw DSL does not perform identity conversion: `C->C` and `CtoC` throw `ArgumentException`. See [Temperature](Transform/Temperature.md#same-unit-conversions), [Length](Transform/Length.md#same-unit-conversions), and [Weight](Transform/Weight.md#same-unit-conversions).

### Example

```csharp
using SeanOne.Alchemy;
using SeanOne.Alchemy.Builder;

double kelvin = AlchemyBuilder.SelectCnv()
    .WithTemp(TempUnit.C, TempUnit.K)
    .Execute(25.0)
    .ToObject<double>();

// 298.15
```

Multiple conversion parameters can be placed in the same `cnv` instruction:

```csharp
var executable = AlchemyBuilder.SelectCnv()
    .WithTemp(TempUnit.C, TempUnit.F)
    .WithLength(LengthUnit.CM, LengthUnit.IN)
    .Build();

// "cnv /temp:\"C->F\"/length:\"CM->IN\""
```

The execution order inside a single `cnv` instruction is fixed by the library: temperature, then weight, then length.

The raw form is also available when you want to pass a DSL unit expression directly, for example using the equivalent `CToF` spelling:

```csharp
AlchemyBuilder.SelectCnv().With(CnvParam.Temp, "CToF");
```

The raw form performs no compile-time validation. See [Temperature](Transform/Temperature.md), [Length](Transform/Length.md), and [Weight](Transform/Weight.md) for the supported unit codes and conversion rules.

## Sorting

`ArrFunctionBuilder` builds `arr` instructions.

| `ArrParam` | DSL parameter | Typed helper |
|------------|---------------|--------------|
| `Sort` | `sort` | `WithSort(SortAlgorithm algorithm)` |

`SortAlgorithm` maps to sort keys as follows:

| `SortAlgorithm` | DSL sort key | Meaning |
|-----------------|--------------|---------|
| `InsertionAsc` | `is` | Insertion sort, ascending |
| `InsertionDesc` | `isd` | Insertion sort, descending |
| `ArraySortAsc` | `as` | `Array.Sort`, ascending |
| `ArraySortDesc` | `asd` | `Array.Sort`, descending |
| `LinqAsc` | `ls` | LINQ sort, ascending |
| `LinqDesc` | `lsd` | LINQ sort, descending |

### Example

```csharp
using SeanOne.Alchemy;
using SeanOne.Alchemy.Builder;

var sorted = AlchemyBuilder.SelectArr()
    .WithSort(SortAlgorithm.InsertionAsc)
    .Execute(new List<int> { 5, 2, 8, 1 })
    .ToObject<List<int>>();

// [1, 2, 5, 8]
```

See [Sorting](Transform/Sorting.md) for accepted sort-key aliases and input requirements.

## Pipelines

Chain `ThenXxx()` on a builder to finalize the current instruction and start another. The same methods are available on `AlchemyBuilder` to start the first stage.

```csharp
using SeanOne.Alchemy;
using SeanOne.Alchemy.Builder;

AlchemyExecutable pipeline = AlchemyBuilder.SelectArr()
    .WithSort(SortAlgorithm.InsertionAsc)
    .ThenCnv()
    .WithTemp(TempUnit.F, TempUnit.C)
    .Build();

var result = pipeline.Execute(new List<double> { 32.0, 212.0, 0.0 });
List<double> values = result.ToObject<List<double>>();

// [-17.77777777777777, 0, 100]
```

The resulting DSL is:

```text
arr /sort:"is" | cnv /temp:"F->C"
```

A `ThenXxx()` call on a function builder automatically performs the same finalization as `Step()`:

```csharp
AlchemyExecutable executable = AlchemyBuilder.SelectCnv()
    .WithTemp(TempUnit.C, TempUnit.F)
    .ThenBasic()
    .WithToString("F2")
    .Build();

// The pipeline converts 25 °C to Fahrenheit, then formats the number.
executable.Execute(25.0).GetString(); // "77.00"
```

You can also build a pipeline starting from an empty `AlchemyBuilder`:

```csharp
var executable = AlchemyBuilder.Create()
    .ThenCnv()
    .WithTemp(TempUnit.C, TempUnit.F)
    .ThenCnv()
    .WithLength(LengthUnit.M, LengthUnit.KM)
    .Build();
```

`AlchemyExecutable.Execute(obj)` passes **all** stored instructions to `Alchemy.Transform(obj, params string[] dslInstructions)` in order. Each step receives the previous step's result, and the original input is deep-cloned before processing.

## Executing and Extracting Results

| API | Returns | Notes |
|-----|---------|-------|
| `builder.Execute(obj)` | `AlchemyResult` | Builds and executes the full pipeline. |
| `executable.Execute(obj)` | `AlchemyResult` | Executes all stored instructions. |
| `executable.Run(obj)` | `string` | **Obsolete.** Executes only the first instruction and uses `Alchemy.Format`. |
| `Alchemy.Transform(obj, dsl)` | `AlchemyResult` | DSL-string alternative for a single instruction. |
| `Alchemy.Transform(obj, dsls)` | `AlchemyResult` | DSL-string alternative for multiple instructions. |

For a formatted result:

```csharp
string text = executable.Execute(obj).GetString();
```

For a transformed object:

```csharp
List<double> result = executable.Execute(values).ToObject<List<double>>();
```

`ToObject<T>()` casts the wrapped object directly. `ToList<T>()` only works when the wrapped object already implements `IEnumerable<T>` exactly. For element-type conversion, use methods such as `GetDoubleList()` or `GetInt32List()`. See [AlchemyResult](AlchemyResult.md) for the full set of conversion helpers.

## Legacy API and migration

The obsolete builder API remains for source compatibility in the same `SeanOne.Alchemy.Builder` namespace. Its public types are marked `[Obsolete]`, as are the legacy factory methods, `With` methods, and helper extensions:

- `AlchemyFormatBuilder` with `SelectBasic()`, `SelectFeSeq()`, and `SelectFeDict()`.
- Interfaces `IAlchemyFunction<TParam>`, `IBasicAlchemyFunction<TParam>`, `ISequenceAlchemyFunction<TParam>`, and `IDictionaryAlchemyFunction<TParam>`.
- Implementations `BasicFunc`, `FeSequenceFunc`, and `FeDictionaryFunc`.
- Legacy extension methods `BuildRun(...)` and `BuildToString()` for `IAlchemyFunction<TParam>`.
- `AlchemyExecutable.Run(...)`.

A legacy formatter looks like this:

```csharp
// Obsolete – shown only for migration context.
string output = AlchemyFormatBuilder.SelectBasic()
    .With(BasicParam.ToString, "F2")
    .BuildRun(5);
```

The supported replacement is:

```csharp
using SeanOne.Alchemy;
using SeanOne.Alchemy.Builder;

string output = AlchemyBuilder.SelectBasic()
    .WithToString("F2")
    .Execute(5)
    .GetString();
```

### Migration map

| Legacy API | Replacement |
|------------|-------------|
| `AlchemyFormatBuilder.SelectBasic()` | `AlchemyBuilder.SelectBasic()` |
| `AlchemyFormatBuilder.SelectFeSeq()` | `AlchemyBuilder.SelectFeSeq()` |
| `AlchemyFormatBuilder.SelectFeDict()` | `AlchemyBuilder.SelectFeDict()` |
| `IAlchemyFunction<T>.With(param, value)` | Typed `WithXxx(...)` helpers, or `.With(param, value)` on the concrete builder |
| `IAlchemyFunction<T>.Build()` | `.Build()` on the concrete builder |
| `BuildRun(obj)` extension | `.Execute(obj).GetString()` for formatting, or `.Execute(obj)` for an `AlchemyResult` |
| `BuildToString()` extension | `.ToString()` or `.Build().ToString()` |
| `AlchemyExecutable.Run(obj)` | `AlchemyExecutable.Execute(obj)` |
| `.With(FeSeqParam.ExcludeLastEnd, true)` | `.WithExcludeLastEnd()` or `.WithExcludeLastEnd(true)` |
| `.With(FeDictParam.ExcludeLastEnd, true)` | `.WithExcludeLastEnd()` or `.WithExcludeLastEnd(true)` |
| `.With(FeSeqParam.FeOpt, true)` | `.WithFeOpt()` or `.WithFeOpt(true)` |
| `.With(FeDictParam.FeOpt, true)` | `.WithFeOpt()` or `.WithFeOpt(true)` |

The `With` methods take `(TParam, string)`; there is no Boolean overload. Use the `WithExcludeLastEnd` and `WithFeOpt` helpers, or pass `"true"` / `"false"` explicitly.

The legacy API only covered formatting (`basic`, `fe`). Unit conversion (`cnv`) and sorting (`arr`) are available through the new `SelectCnv()` and `SelectArr()` builders.

## Additional Notes

- **Compile-time safety is limited to signatures.** The enums prevent misspelled parameter names, but the library still validates semantic rules such as required `dict-format`, supported sort keys, and valid conversion units at execution time.
- **Values are automatically quoted.** Embedded double quotes are encoded as `\u0022`. DSL escape sequences such as `\n` and `\u000A` are still interpreted when the instruction is executed; see [DSL Syntax](DSLSyntax.md).
- **Parameter order is preserved.** Parameters are emitted in the order the `With...` calls are made.
- **One builder stage emits one instruction.** Use `ThenXxx()` or separate `SelectXxx()` calls for multiple instructions.
- **No raw-command append is exposed.** `AlchemyBuilder.AddCommand` is internal; build instructions through the typed builders.

## See Also

- [Getting Started](GettingStarted.md)
- [DSL Syntax Reference](DSLSyntax.md)
- [Basic Formatting](Format/Basic.md)
- [Formatting Collections](Format/Collections.md)
- [Formatting Dictionaries](Format/Dictionaries.md)
- [Sorting](Transform/Sorting.md)
- [Temperature](Transform/Temperature.md)
- [Length](Transform/Length.md)
- [Weight](Transform/Weight.md)
- [AlchemyResult](AlchemyResult.md)
- [Error Handling](ErrorHandling.md)
