# Weight Conversion with Alchemy.Transform

The `/weight` parameter converts numeric values between different weight (mass) units.

## DSL Syntax

```
cnv /weight:source->target
```

or

```
cnv /weight:sourceTotarget
```

Parameter value is **case‑insensitive** – `KgToG`, `kgtog`, `KGTOG` all work the same.

## Supported Units

| Unit | Code | Description |
|------|------|-------------|
| Milligram | `Mg` | 1 mg = 0.001 g |
| Centigram | `Cg` | 1 cg = 0.01 g |
| Decigram | `Dg` | 1 dg = 0.1 g |
| Gram | `G` | 1 g |
| Decagram | `Dag` | 1 dag = 10 g |
| Hectogram | `Hg` | 1 hg = 100 g |
| Kilogram | `Kg` | 1 kg = 1000 g |
| Tonne | `T` | 1 t = 1000 kg |
| Ounce | `Oz` | 1 oz ≈ 28.349523125 g |
| Pound | `Lb` | 1 lb = 453.59237 g |
| Stone | `St` | 1 st = 6.35029318 kg |
| Short Ton | `ShortTon` | 1 short ton = 907.18474 kg (US ton) |
| Long Ton | `LongTon` | 1 long ton = 1016.0469088 kg (imperial ton) |

## Examples

### Single value

```csharp
using SeanOne.Alchemy;

double kilograms = 5;
double grams = Alchemy.Transform(kilograms, "cnv /weight:Kg->G").ToObject<double>();
// grams = 5000
```

### Collection of values

```csharp
using SeanOne.Alchemy;

var weights = new List<double> { 1, 2, 3 };
Alchemy.Transform(weights, "cnv /weight:Lb->Kg");
// Result: [0.45359237, 0.90718474, 1.36077711]
```

### Combining with sorting

```csharp
using SeanOne.Alchemy;

var data = new List<double> { 1000, 1, 0.5 };
Alchemy.Transform(data, "cnv /sort:is", "/weight:Kg->G");
// Sorts to [0.5, 1, 1000] then converts to [500, 1000, 1000000]
```

## Input Types Supported

- Single numeric: `int`, `double`, `decimal`, `float`, etc.
- Collection of numerics: any `IEnumerable` where each element can be converted to `double`.

## Important Notes

- Weight values **cannot be negative** – passing a negative number throws `ArgumentException`.
- All conversions use exact standard factors (e.g., 1 lb = 453.59237 g exactly).
- The library performs a **deep clone** before conversion; original object unchanged.

## Error Cases

- Unknown unit code (e.g., `/weight:X->Y`) → `ArgumentException`.
- Negative value → `ArgumentException`.
- Non‑numeric collection element → `InvalidOperationException`.
- See [ErrorHandling.md](../ErrorHandling.md) for details.
