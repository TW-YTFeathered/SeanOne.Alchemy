# Builder Generic Factory Design (Deprecated)

## Reasons for Deprecation

1. **Type Unsafe**: Requires type casting of different TParam values, violating compile-time type checking.
2. **Unintuitive API**: Users need to understand the fixed correspondence between FunctionName and TParam.
3. **Poor Development Experience**: Cannot provide the intelligent code completion advantages of the Fluent API.

```csharp
#region Generic Factory (Commented Out)
/*
/// <summary>
/// Represents the available function types.
/// Note: FeSequence and FeDictionary are distinguished at the enum level,
/// but both rely on the same underlying DSL function (fe).
/// </summary>
public enum FunctionName
{
    /// <summary>
    /// Sequence-based DSL function (internally uses the same 'fe' function).
    /// </summary>
    FeSequence,

    /// <summary>
    /// Dictionary-based DSL function (internally uses the same 'fe' function).
    /// </summary>
    FeDictionary,

    /// <summary>
    /// Basic DSL function.
    /// </summary>
    Basic
}
*/

/*
/// <summary>
/// Generic factory method to select a DSL function based on the specified function name.
/// </summary>
/// <typeparam name="TParam">The type of parameter used by the DSL function.</typeparam>
/// <param name="funcName">The function type to select.</param>
/// <returns>An implementation of <see cref="IAlchemyFunction{TParam}"/>.</returns>
public static IAlchemyFunction<TParam> SelectFunction<TParam>(FunctionName funcName)
{
    switch (funcName)
    {
        case FunctionName.FeSequence:
            return (IAlchemyFunction<TParam>)new FeSequenceFunc();
        case FunctionName.FeDictionary:
            return (IAlchemyFunction<TParam>)new FeDictionaryFunc();
        case FunctionName.Basic:
            return (IAlchemyFunction<TParam>)new BasicFunc();
        default:
            throw new ArgumentOutOfRangeException(nameof(funcName), funcName, null);
    }
}
*/
#endregion
```
