# Frequently Asked Questions

## General

### Why does `\n` behave differently on Windows and Linux?

Alchemy's `\n` is converted to `Environment.NewLine` at runtime. On Windows that's `\r\n`, on Unix `\n`. This matches default C# string behavior. For platform‑independent newlines, use `\u000A` (LF) or `\u000D\u000A` (CRLF).

### Does Alchemy modify my original object?

No. All operations are performed on a **deep clone** of the input object. The original remains unchanged.

### Are the methods thread‑safe?

The static methods `AlchemyFormatter.Format` and `AlchemyConverter.Convert` are thread‑safe as long as the input object is not mutated concurrently. Reusable builders (`Build()`) produce instances that are not guaranteed thread‑safe; create separate instances per thread or synchronize access.

## Performance

### When should I use `/fe-opt:true`?

For large collections (thousands of elements), `/fe-opt:true` can be ~1.5x faster. However, it may have compatibility issues with some custom collection types. Test with your data first.

### How expensive is deep cloning?

Cloning is done via serialization (binary or JSON). For very large objects, consider working on a copy you create yourself, or use the Fluent API to avoid cloning by design? Actually the library always clones. If performance is critical, you might want to contribute an option to disable cloning.

## Formatting

### Can I format a `string` with `/tostring`?

No. `string` does not implement `IFormattable`. Use the string directly without `/tostring`.

### Why does `fe` throw on a `string`?

Because `string` is `IEnumerable<char>`, but treating it as a collection of characters is rarely intended. Alchemy explicitly disallows formatting a `string` with `fe` to avoid confusion.

### How do I format a dictionary to show only values?

Use `/dict-format:{1}` (ignore the key placeholder).

## Conversion

### What happens if I sort a read‑only collection?

The library clones the input first, so the read‑only original is unaffected. However, if the collection type itself cannot be cloned (e.g., some immutable collections), an exception may occur.

### Does temperature conversion check for absolute zero?

Currently not enforced, but planned for a future release. Inputs below 0 K will produce mathematically correct but physically invalid results.

### Can I add custom sorting algorithms?

Not via DSL, but you can extend the library by contributing to the `Code/Sorting/ListSorter` folder.

## Fluent API

### Is the Fluent API for conversion available?

Not yet. It is under active development (targeting v3.x). See the preview in [FluentAPI.md](FluentAPI.md).

### Can I parse a DSL string and then modify it with Fluent API?

Yes. Use `AlchemyFormatBuilder.FromDsl(dslString)` to get a builder, then chain `.With(...)` and finally `.Build()` or `.BuildRun()`.

## Errors and Debugging

### How do I see which parameter caused an error?

All exceptions include the parameter name in the message when possible. Use try‑catch and examine `ex.Message`. For more details, see [ErrorHandling.md](ErrorHandling.md).

### Does the library log anything?

No internal logging. All issues are reported via exceptions.

## Project & Support

### What .NET versions are supported?

. NET 6.0, 8.0, 10.0, .NET Standard 2.0, .NET Framework 4.7.2 and 4.8.

### Where can I report issues or request features?

GitHub repository: [TW-YTFeathered/SeanOne.Alchemy](https://github.com/TW-YTFeathered/SeanOne.Alchemy)

### Is there a changelog?

See `UpdateLog.md` in the repository root.
