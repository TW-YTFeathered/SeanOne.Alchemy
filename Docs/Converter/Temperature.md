# Temperature Conversion with Alchemy.Transform

The `/temp` parameter converts numeric values between Celsius, Fahrenheit, and Kelvin.

## DSL Syntax

```
cnv /temp:source->target
```

or

```
cnv /temp:sourceTotarget
```

Parameter value is **case‑insensitive** – `C->F`, `c->f`, `CtoF` all work the same.

## Supported Units

| Unit Code | Meaning |
|-----------|---------|
| `C` | Celsius |
| `F` | Fahrenheit |
| `K` | Kelvin |

## Conversion Formulas

- `F = C * 9/5 + 32`
- `C = (F - 32) * 5/9`
- `K = C + 273.15`
- `C = K - 273.15`
- `F` ↔ `K` via intermediate Celsius.

## Examples

### Single value

```csharp
using SeanOne.Alchemy;

double celsius = 25;
double fahrenheit = Alchemy.Transform(celsius, "cnv /temp:C->F").ToObject<double>();
// fahrenheit = 77.0
```

### Collection of values

```csharp
using SeanOne.Alchemy;

var temps = new List<double> { 0, 100, -40 };
Alchemy.Transform(temps, "cnv /temp:C->K");
// Result: [273.15, 373.15, 233.15]
```

### Combining with sorting

```csharp
using SeanOne.Alchemy;

var data = new List<double> { 32.0, 212.0, 0.0 };
Alchemy.Transform(data, "cnv /sort:is", "/temp:F->C");
// Sorts to [0, 32, 212] then converts to [-17.777..., 0, 100]
```

## Input Types Supported

- Single numeric: `int`, `double`, `decimal`, `float`, etc.
- Collection of numerics: any `IEnumerable` where each element can be converted to `double`.

## Important Notes

- Temperature values **below absolute zero (0 K)** throw `ArgumentException`.
- Conversions use `double` arithmetic; results may have floating‑point rounding.
- The library performs a **deep clone** before conversion; original object unchanged.

## Error Cases

- Unknown unit code (e.g., `/temp:X->Y`) → `ArgumentException`.
- Value below absolute zero → `ArgumentException`.
- Non‑numeric collection element → `InvalidOperationException`.
- See [ErrorHandling.md](../ErrorHandling.md) for details.
