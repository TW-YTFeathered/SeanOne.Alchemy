# Frequently Asked Questions

## General

### Why does `\n` behave differently on Windows and Linux?

Alchemy's `\n` is converted to `Environment.NewLine` at runtime. On Windows that's `\r\n`, on Unix `\n`. This matches default C# string behavior. For platform‑independent newlines, use `\u000A` (LF) or `\u000D\u000A` (CRLF).

### Does Alchemy modify my original object?

No. All operations are performed on a **deep clone** of the input object. The original remains unchanged.

### Are the methods thread‑safe?

The static methods `Alchemy.Format` and `Alchemy.Transform` are thread‑safe as long as the input object is not mutated concurrently. Reusable builders (`Build()`) produce instances that are not guaranteed thread‑safe; create separate instances per thread or synchronize access.

## Performance

### When should I use `/fe-opt:true`?

For large collections (thousands of elements), `/fe-opt:true` can be ~1.5x faster. However, it may have compatibility issues with some custom collection types. Test with your data first.

### How expensive is deep cloning?

Cloning is done via reflection, recursively copying all fields (including private ones). For very large objects, consider working on a copy you create yourself, or use the Fluent API to avoid cloning by design? Actually the library always clones. If performance is critical, you might want to contribute an option to disable cloning.

## Formatting

### Can I format a `string` with `/tostring`?

No. `string` does not implement `IFormattable`. Use the string directly without `/tostring`.

### Why does `fe` throw on a `string`?

Because `string` is `IEnumerable<char>`, but treating it as a collection of characters is rarely intended. Alchemy explicitly disallows formatting a `string` with `fe` to avoid confusion.

### How do I format a dictionary to show only values?

Use `/dict-format:{1}` (ignore the key placeholder).

## Transform

### What happens if I sort a read‑only collection?

The library clones the input first, so the read‑only original is unaffected. However, if the collection type itself cannot be cloned (e.g., some immutable collections), an exception may occur.

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

Not yet. It is under active development (targeting v3.x). See the preview in [FluentAPI.md](FluentAPI.md).

### Can I parse a DSL string and then modify it with Fluent API?

Not implemented in this version.

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

## Advanced Topics

### What are the compatibility risks of `/fe-opt:true`?

The optimized formatter (`/fe-opt:true`) leverages `Span`-based APIs and `ArrayPool` for better performance. While it works flawlessly for standard BCL collections (`List<T>`, `T[]`, etc.), it may encounter issues with:

- **Custom collections** that do not properly implement `IEnumerator` (e.g., returning a non-disposable enumerator, or throwing exceptions during `MoveNext()` that are not handled).
- **Non-generic `IEnumerable`** implementations that rely on legacy iteration patterns.
- In .NET 6+, if the element type does not implement `ISpanFormattable`, the formatter safely falls back to `IFormattable`, so this is rarely a problem.

**What to do if it fails:**  
Set `/fe-opt:false` (or omit it, as `false` is the default) to use the stable, legacy formatter. If you encounter a failure, please report it with a minimal reproduction so the team can investigate.

### What does `AlchemyResult.ToString()` return?

It depends on the wrapped object:

- If the underlying object is a **`string`**, `.ToString()` returns that string directly.
- For **any other type**, it returns the base `Object.ToString()` implementation, which typically yields the type name (e.g., `System.Collections.Generic.List`1[System.Int32]`).

**To get the string representation of the wrapped object** (calling its `ToString()` method), use the extension method `.GetString()` provided in `AlchemyConverterExpansions`. This ensures you always get the intended textual representation.

### How does deep cloning handle circular references?

The internal `ReflectionCloner` safely handles circular references (e.g., `Parent` -> `Child` -> `Parent`).

It maintains a `Dictionary<object, object>` (the `visited` lookup) during the cloning process. When the cloner encounters an object reference that has already been cloned, it **reuses the existing cloned instance** instead of recursively re‑entering the same object graph. This prevents infinite recursion and `StackOverflowException`, ensuring that the cloned object graph retains its structural integrity.
