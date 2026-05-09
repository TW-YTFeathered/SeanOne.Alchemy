# DSL Syntax Reference

This document describes the common syntax rules for Alchemy DSL instructions, shared by both `AlchemyFormatter` and `AlchemyConverter`.

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

## Multi‑Instruction Execution

When you pass an array of instruction strings (e.g., `AlchemyFormatter.Format(obj, instr1, instr2)`), each instruction is executed **independently and sequentially**. The result of the previous instruction becomes the input for the next.

Example:

```csharp
AlchemyConverter.Convert(data, "cnv /sort:is", "/temp:F->C");
// First sorts, then converts temperatures.
```

This is different from merging parameters into a single instruction (where internal order may be fixed).
