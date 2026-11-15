# Frequently Asked Questions

## General

### Why does `\n` behave differently on Windows and Linux?

Alchemy's `\n` is converted to `Environment.NewLine` at runtime. On Windows that's `\r\n`, on Unix `\n`. This matches default C# string behavior. For platform‑independent newlines, use `\u000A` (LF) or `\u000D\u000A` (CRLF).

### Does Alchemy modify my original object?

- **`Alchemy.Format`**: No. It works directly on the original object without modifying it. No cloning is performed.
- **`Alchemy.Transform`**: No. All operations are performed on a **deep clone** of the input object. The original remains unchanged.

If you only need formatting, use `Alchemy.Format` to avoid the performance cost of cloning.

### Are the methods thread‑safe?

The static methods `Alchemy.Format` and `Alchemy.Transform` are thread‑safe as long as the input object is not mutated concurrently. Reusable builders (`Build()`) produce instances that are not guaranteed thread‑safe; create separate instances per thread or synchronize access.

## Culture and Globalization

Formatting and parsing use the thread's culture (`CultureInfo.CurrentCulture`), not invariant.

- `de-DE`: `Alchemy.Format(1.5, "/tostring:F2")` → `"1,50"`
- `de-DE`: `AlchemyResult.Parse("1.5").GetDouble()` → `15`; `Parse("1,234").GetInt32()` → `FormatException`

A formatted value and the `GetXxx()` that reads it back must use the same culture; for data that crosses machines, format/parse it yourself with `CultureInfo.InvariantCulture`. Control it via `CultureInfo.CurrentCulture` / `DefaultThreadCurrentCulture`.

## Performance

### When should I use `/fe-opt:true`?

For large collections (thousands of elements), `/fe-opt:true` can be ~1.5x faster. However, it may have compatibility issues with some custom collection types. Test with your data first.

### How expensive is deep cloning?

Cloning is performed **only in `Alchemy.Transform`** – it uses reflection to recursively copy all fields (including private ones). `Alchemy.Format` does **not** clone the input object; it works directly on the original without modifying it.

For very large objects, the cloning overhead in `Alchemy.Transform` can be significant. If you only need formatting, use `Alchemy.Format` to avoid the cost of cloning entirely.

## Formatting

### Can I format a `string` with `/tostring`?

No. `string` does not implement `IFormattable`. Use the string directly without `/tostring`.

### Why does `fe` throw on a `string`?

Because `string` is `IEnumerable<char>`, but treating it as a collection of characters is rarely intended. Alchemy explicitly disallows formatting a `string` with `fe` to avoid confusion.

### How do I format a dictionary to show only values?

Use `/dict-format:{1}` (ignore the key placeholder).

## Transform

### What happens if I sort a read‑only collection?

The original is untouched (input is cloned), but the clone is still read‑only, so sorting throws `NotSupportedException: Collection is read-only.` Sort a mutable copy instead.

### Does temperature conversion check for absolute zero?

Yes. Passing a value below absolute zero (0 K) throws ArgumentException.

### Can I add custom sorting algorithms?

Not via DSL, but you can extend the library by contributing to the `Code/Sorting/ListSorter` folder.

### Can `Alchemy.Transform` perform formatting tasks like `Alchemy.Format`?

Yes. `Alchemy.Transform` seamlessly accepts formatting instructions (fe, foreach, basic, or a parameter starting with '/'). Under the hood, it routes such instructions to `Alchemy.Format`.

Example:
```csharp
using SeanOne.Alchemy;

/*
* Transform can handle formatting directives too – it internally routes them to Alchemy.Format,
* so you can use Transform as a single unified entry point instead of calling Format separately.
*/
string result = Alchemy.Transform(123.456, "/tostring:F2 /prefix:\"$\"").ToString();
```

This works because Transform internally checks the directive and delegates to Format when appropriate.

## Fluent API

### Is the Fluent API for conversion available?

Yes. `AlchemyBuilder.SelectCnv()` builds `cnv` instructions and `AlchemyBuilder.SelectArr()` builds `arr` instructions, so unit conversion and sorting are fully available through the Fluent API. See [Fluent API → Unit Conversions](FluentAPI.md#unit-conversions) and [Fluent API → Sorting](FluentAPI.md#sorting).

### Can I parse a DSL string and then modify it with Fluent API?

Not implemented in this version.

## Errors and Debugging

### How do I see which parameter caused an error?

All exceptions include the parameter name in the message when possible. Use try‑catch and examine `ex.Message`. For more details, see [ErrorHandling.md](ErrorHandling.md).

### Does the library log anything?

No internal logging. All issues are reported via exceptions.

## Project & Support

### What .NET versions are supported?

.NET Standard 2.0, .NET 8.0, and .NET 10.0 are supported.

### Where can I report issues or request features?

GitHub repository: [TW-YTFeathered/SeanOne.Alchemy](https://github.com/TW-YTFeathered/SeanOne.Alchemy)

### Is there a changelog?

See `UpdateLog.md` in the repository root.

## Advanced Topics

### What are the compatibility risks of `/fe-opt:true`?

On **.NET 6.0+**, the optimized formatter (`/fe-opt:true`) leverages `Span`-based APIs and `ArrayPool<char>` for better performance. On **.NET Standard 2.0**, a fallback implementation is used instead.

While the optimized path works flawlessly for standard BCL collections (`List<T>`, `T[]`, etc.), it may encounter issues with:

- **Custom collections** that do not properly implement `IEnumerator` (e.g., returning a non-disposable enumerator, or throwing exceptions during `MoveNext()` that are not handled).
- **Non-generic `IEnumerable`** implementations that rely on legacy iteration patterns.
- If the element type does not implement `ISpanFormattable`, the formatter safely falls back to `IFormattable`, so this is rarely a problem.

**What to do if it fails:**  
Set `/fe-opt:false` (or omit it, as `false` is the default) to use the stable, legacy formatter. If you encounter a failure, please report it with a minimal reproduction so the team can investigate.

### What does `AlchemyResult.ToString()` return?

Wrapped `string` → that string. Anything else → `"SeanOne.Alchemy.AlchemyResult"`.

For the wrapped object's own `ToString()`, use `GetString()`.

### How does deep cloning handle circular references?

The internal `ReflectionCloner` safely handles circular references (e.g., `Parent` -> `Child` -> `Parent`).

It maintains a `Dictionary<object, object>` (the `visited` lookup) during the cloning process. When the cloner encounters an object reference that has already been cloned, it **reuses the existing cloned instance** instead of recursively re‑entering the same object graph. This prevents infinite recursion and `StackOverflowException`, ensuring that the cloned object graph retains its structural integrity.
