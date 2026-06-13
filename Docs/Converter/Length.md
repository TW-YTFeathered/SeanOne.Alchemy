# Length Conversion with Alchemy.Transform

The `/length` parameter converts numeric values between different length units.

## DSL Syntax

```
cnv /length:source->target
```

or

```
cnv /length:sourceTotarget
```

Parameter value is **case‑insensitive** – `KgToG`, `kgtog`, `KGTOG` all work the same.

## Supported Units

| Unit | Code | Description |
|------|------|-------------|
| Angstrom | `A` | 1 Å = 10⁻¹⁰ m |
| Nanometer | `NM` | 1 nm = 10⁻⁹ m |
| Micrometer | `UM` | 1 μm = 10⁻⁶ m |
| Millimeter | `MM` | 1 mm |
| Centimeter | `CM` | 1 cm |
| Meter | `M` | 1 m |
| Kilometer | `KM` | 1 km |
| Inch | `IN` | 1 in = 2.54 cm |
| Foot | `FT` | 1 ft = 12 in |
| Yard | `YD` | 1 yd = 3 ft |
| Mile | `MI` | 1 mi = 5280 ft |
| Nautical Mile | `NMI` | 1 nmi = 1852 m |

## Examples

### Single value

```csharp
using SeanOne.Alchemy;

double meters = 1000;
double kilometers = Alchemy.Transform(meters, "cnv /length:M->KM").ToObject<double>();
// kilometers = 1.0
```

### Collection of values

```csharp
using SeanOne.Alchemy;

var lengths = new List<double> { 2.54, 5.08, 7.62 };
Alchemy.Transform(lengths, "cnv /length:CM->IN");
// Result: [1.0, 2.0, 3.0]
```

### Combining with sorting

```csharp
using SeanOne.Alchemy;

var data = new List<double> { 1000, 1, 0.001 };
Alchemy.Transform(data, "cnv /sort:is", "/length:M->KM");
// Sorts then converts: [0.001, 1, 1000] → [0.000001, 0.001, 1] (meters to kilometers)
```

## Input Types Supported

- Single numeric: `int`, `double`, `decimal`, `float`, etc.
- Collection of numerics: any `IEnumerable` where each element can be converted to `double`.

## Important Notes

- Length values **cannot be negative** – passing a negative number throws `ArgumentException`.
- Conversion uses exact factors (e.g., 1 in = 2.54 cm exactly).
- The library performs a **deep clone** before conversion; original object unchanged.

## Error Cases

- Unknown unit code (e.g., `/length:X->Y`) → `ArgumentException`.
- Negative value → `ArgumentException`.
- Non‑numeric collection element → `InvalidOperationException`.
- See [ErrorHandling.md](../ErrorHandling.md) for details.
