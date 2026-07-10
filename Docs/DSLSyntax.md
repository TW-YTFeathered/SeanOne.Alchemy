# DSL Syntax Reference

This document describes the common syntax rules for Alchemy DSL instructions, shared by both `Alchemy.Format` and `Alchemy.Transform`.

## Instruction Format

```text
[FunctionName] /param1:value1 /param2:"value with spaces" /param3:"value/with/slash"
```

- The instruction starts with a function name (e.g., `basic`, `fe`, `cnv`).
- Parameters begin with `/` followed by the parameter name and a colon `:`.
- Parameter names are **case-sensitive**.
- Values containing spaces or special characters (like `/`) must be enclosed in double quotes `" "`.
- Multiple instructions can be passed as an array; they execute sequentially.

## Parameter Naming Conventions

| Context | Style | Example |
|---------|-------|---------|
| DSL (string instructions) | kebab-case, lowercase | `/dict-format`, `/exclude-last-end` |
| Fluent API (enums) | PascalCase | `FeDictParam.DictFormat` |

## Escape Sequences

Escape sequences are supported **inside parameter values** (not in parameter names).

### Short Escape Sequences (platform-aware)

| Sequence | Character |
|----------|-----------|
| `\0`     | Null |
| `\a`     | Alert/beep |
| `\b`     | Backspace |
| `\f`     | Form feed |
| `\n`     | **Platform-specific newline** → `Environment.NewLine` |
| `\r`     | Carriage return |
| `\t`     | Horizontal tab |
| `\v`     | Vertical tab |
| `\\`     | Backslash |
| `\'`     | Single quote |
| `\"`     | Double quote |

> ⚠️ **`\n` behavior**: On Windows it becomes `\r\n`, on Unix/Linux `\n`. For cross‑platform consistency, use Unicode sequences.

### Unicode Escape Sequences (platform‑independent)

Format: `\uXXXX` (4 hex digits)

| Sequence       | Output |
|----------------|--------|
| `\u000A`       | Line feed (LF) – always `\n` |
| `\u000D`       | Carriage return (CR) – always `\r` |
| `\u000D\u000A` | CRLF – always `\r\n` |

### Recommendations

- For general use: `\n` is convenient and works like C#.
- For **identical output across platforms**: use `\u000A` or `\u000D\u000A`.
- For special characters: use short sequences where possible.

## Multi-Instruction Execution

When using `Alchemy.Transform`, you can pass multiple instruction strings as an array. Each instruction is executed **sequentially** – the transformed result from the previous step becomes the input for the next.

**Basic example:**

```csharp
using SeanOne.Alchemy;

var data = new List<double> { 212.0, 32.0, 100.0 };

Alchemy.Transform(data,
    "arr /sort:is",          // Step 1: sort ascending
    "cnv /temp:F->C"         // Step 2: convert Fahrenheit to Celsius
);
// Result: [-17.77, 37.77, 100.0] (sorted then converted)
```

This differs from merging parameters into a single instruction – each instruction is processed independently and in the order provided.

### Instruction Name Auto-Completion

When chaining multiple instructions, you can **omit the function name** in subsequent instructions if it matches the previous one. The library will automatically reuse the function name from the preceding instruction.

**Example (compatible instructions):**

```csharp
using SeanOne.Alchemy;

Alchemy.Transform(data,
    "cnv /temp:F->C",        // function: "cnv"
    "/weight:Kg->G"          // auto-completes to "cnv /weight:Kg->G"
);
// First converts temperature, then converts weight – both within cnv context.
```

This works for any function name (`cnv`, `arr`, `basic`, `fe`, etc.). When a new instruction explicitly specifies a different function name, auto‑completion resets to that new name.

**⚠️ Important:** Auto‑completion only applies the function name – it does **not** validate whether the parameters are compatible with that function. For example, if you omit the function name after an `arr` instruction and then write `/temp:C->F`, the resulting instruction becomes `arr /temp:C->F`, which will throw an exception because `arr` does not support `/temp`. Always ensure that the parameters you write match the inherited function's capabilities.

**Safe practice:** When in doubt, write the full function name explicitly rather than relying on auto‑completion.
