# Async Independent Implementation (Deprecated)

## Reasons for Deprecation

| Issue                | Description                                          | Impact                                                       |
|----------------------|------------------------------------------------------|--------------------------------------------------------------|
| **Code Duplication** | Sync/Async versions contained ~95% identical logic   | Increased maintenance burden, higher risk of inconsistencies |
| **No Actual I/O**    | Formatting and conversion are CPU-bound operations   | Independent async logic provided no performance benefit      |
| **Complexity**       | Separate async files increased cyclomatic complexity | `ToStringAsync.cs` scored 29.20/100 (lowest in project)      |
| **Maintenance**      | Bug fixes required changes in two places             | Higher chance of sync/async behavior divergence              |

## Current Implementation: Task.Run Wrapper

Instead of duplicating logic, async methods now offload CPU-bound work to the ThreadPool using `Task.Run`.

### Code Example

```csharp
public static async Task<AlchemyResult> ConvertAsync(object obj, string dslInstruction)
{
    return await Task.Run(() =>
    {
        return Convert(obj, dslInstruction);
    });
}

public static async Task<AlchemyResult> ConvertAsync(object obj, params string[] dslInstructions)
{
    return await Task.Run(() =>
    {
        return Convert(obj, dslInstructions);
    });
}
```

## Why Task.Run Instead of Task.FromResult?

| Approach                        | Behavior                                  | Suitability                                           |
|---------------------------------|-------------------------------------------|-------------------------------------------------------|
| `Task.FromResult(Convert(...))` | Executes synchronously on caller thread   | ❌ Blocks UI thread (not suitable for CPU-bound work) |
| `Task.Run(() => Convert(...))`  | Offloads to ThreadPool                    | ✅ Keeps UI responsive, true async behavior           |
| Independent Async Logic         | Duplicated code with async/await keywords | ❌ Unnecessary complexity for CPU-bound operations    |

**Rationale:**  
`AlchemyConverter.Convert` is a CPU-intensive operation. Using `Task.Run` ensures the calling thread (e.g., UI thread) remains responsive while the work is executed on a background thread.

## Results

| Metric                 | Before                  | After   | Change        |
|------------------------|-------------------------|---------|---------------|
| **Code Quality Score** | 88.38                   | 91.90   | +3.52         |
| **Code Duplication**   | 6.28%                   | 2.41%   | -61%          |
| **Problem Files**      | `ToStringAsync.cs` (#1) | Removed | N/A           |
| **Maintainability**    | 2 files to update       | 1 file  | 50% less work |

## Migration Guide

### For Users

No changes required. API remains identical:

```csharp
await AlchemyConverter.ConvertAsync(data, "cnv /temp:KtoC");
```

### For Contributors

- ❌ Do **NOT** create separate `*Async.cs` files
- ❌ Do **NOT** duplicate logic for async methods
- ✅ Use `Task.Run` pattern for CPU-bound async wrappers
- ✅ Keep all core logic in synchronous methods

## References

- [Microsoft: Task.Run vs Task.FromResult](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.run)
- [CPU-Bound vs I/O-Bound Operations](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/)
- Code Quality Report: [report.md](https://github.com/TW-YTFeathered/SeanOne.Alchemy/blob/dev/DesignDocs/DeprecatedAPIs/report.md)

---

*Last Updated: 2026/03/02*  
*Author: SeanOne.Alchemy Contributors*
