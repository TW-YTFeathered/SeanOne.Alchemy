# Update Log

> ℹ️ This Update Log serves as a consolidated history of the project,  
> including versions developed in the original repository (now closed)  
> and all subsequent releases under the new name `SeanOne.Alchemy`.

## **Release**

## V2.0.0

*Focus:Rename*

- Renamed the project to `SeanOne.Alchemy`

### **Why the Rename to `SeanOne.Alchemy`?**

The project originally began as a simple object-to-string DSL formatter.
However, its long-term vision is broader than a single formatting tool.
The name **“Alchemy”** better reflects the project’s direction and future potential.

Although the current version focuses on:

- **object → string** formatting

the design philosophy behind the project aims to support a wider concept of **data transformation**, such as:

- object → object (flexible data shaping or mapping)
- string → object (config-driven parsing or construction)
- string → string (templating or programmable rewriting)

These future directions are *possibilities*, not promises, but they represent the conceptual scope that the name “Alchemy” is meant to capture.

Just as alchemy historically symbolized **transforming one substance into another**,
this project adopts the name to express its broader goal:
a general transformation framework driven by DSL and expressive rules.

`SeanOne.Alchemy` marks the beginning of that expanded vision.

## V1.1.0

*Focus:Fluent API*

- Added a fluent API to make DSL formatting simpler and more readable

### V1.0.1

- Corrected NuGet package URLs
- First published version on NuGet

### **V1.0.0**

*Focus:Initial release*

- Initial release of the DSL formatting tool

## **Beta**

### **V3.0.0-beta.3**

*Focus: Add object → object conversion functions & performance optimizations*

- **Added .NET 10 support** – The library now targets and is fully compatible with .NET 10, in addition to existing .NET 6.0, 8.0, .NET Standard 2.0, .NET Framework 4.7.2 and 4.8.
- **New conversion parameters** – Extended `AlchemyConverter.Convert` with three new unit‑aware transformations:
  - `/temp` – Convert between Celsius, Fahrenheit, and Kelvin.
  - `/weight` – Convert between metric (mg, g, kg, t) and imperial (oz, lb, st, short/long ton) units.
  - `/length` – Convert between metric (mm, cm, m, km) and imperial (in, ft, yd, mi, nmi) units, plus Angstrom, nanometer, micrometer.
  - All unit codes are **case‑insensitive** (e.g., `kgtog`, `KGTOG`, `KgToG` all work).
- **New formatting parameter** – Added `/begin` to `AlchemyFormatter.Format`, allowing a string to be prepended before each element/entry when using `fe` (and also works in `basic`).
- **Improved `fe-opt` performance** – The optimized enumerable/dictionary formatter in `.NET 6+` now uses `ArrayPool<char>` and `Span`-based appending, reducing allocations and increasing throughput.
- The `AlchemyConverter.Convert` method now automatically deep‑clones the input object before applying any transformation, leaving the original unchanged.
- Added full support for both **single numeric values** and **collections of numerics** (`IEnumerable`), with automatic type detection.
- Internal refactoring: unified the conversion core logic (`Convert_Core`) to allow future extensions.
- **Documentation overhaul** – Completely restructured the documentation from the old `README.md`, `Guide_Format.md`, and `Guide_Convert.md` into a comprehensive, multi‑file system:
  - Top‑level guides: `GettingStarted.md`, `DSLSyntax.md`, `FluentAPI.md`, `ErrorHandling.md`, `FAQ.md`
  - Formatter sub‑docs: `Basic.md`, `Collections.md`, `Dictionaries.md`
  - Converter sub‑docs: `Sorting.md`, `Temperature.md`, `Length.md`, `Weight.md`
  - All documentation now resides under the `Docs/` folder (capital D), with clear separation between user guides (`Docs/`) and internal design notes (`DesignDocs/`).

### **V3.0.0-beta.2**

*Focus:Add new object → object functions*

- Added `cnv` function to support object → object transformations via `AlchemyConverter.Convert` method (synchronous entry point).
  - Added `/sort` parameter to control sorting of collections (currently supports IList only).
- Fixed a bug where duplicate parameters were not detected as duplicates.
- Performance: Converted frequently used dictionaries to `static readonly` to reduce allocations and improve performance.

### **V3.0.0-beta.1**

*Focus:Add new parameters for collection formatting*

- Added new parameters to `AlchemyFormatter.Format` for advanced collection formatting:
  - `prefix` / `suffix`: Add strings before/after the entire collection result
  - `fe-opt`: Enable optimized formatting (may have compatibility issues)

### **V1.0.0-beta.4**

*Focus:Bug fixs*

- Addressed errors arising from dependence on regular expressions or `string.Format()` by removing reliance on them
- Fixed the regular expression so that parameters are treated as valid matchable arguments, avoiding errors
- Converted function comments to a unified XML format
- Converted Markdown permalinks to relative URLs

### **V1.0.0-beta.3**

*Focus:Refine GUIDE.md and improve API clarity*

- Added `GUIDE.md` to complement `README.md` with extended documentation
- Added asynchronous (`async`) methods to support non-blocking operations
- Renamed parameters to follow a more consistent and syntactically appropriate naming style, except for `tostring`, which directly invokes C#'s `ToString` method
- Replaced generic `throw` with specific exception type for clearer error semantics
- Improve the comments

### **V1.0.0-beta.2**

*Focus:Bug fixs*

- Create a Git repository and push it to GitHub
- Renamed `namespace`, `class`, and `entry point` to align with `DSL` naming conventions
- Wrote Markdown files
- Refined the approach
- Fixed the issue where a parameter value was incorrectly interpreted as a parameter name
- Fixed the bug where `\r`, `\n`, etc. are incorrectly treated as whitespace

### **V1.0.0-beta.1**

*Focus:Initial file creation*

- Extracted formatting module from an unpublished project and repurposed it as a DSL formatting tool
