// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

namespace SeanOne.Alchemy.Builder;

/// <summary>
/// Defines the parameter options used by <c>BasicFunctionBuilder</c> (basic).
/// Handles basic DSL parameters and formatting logic.
/// </summary>
public enum BasicParam
{
    /// <summary>
    /// Prepends a string immediately before the formatted value (after prefix).
    /// DSL param: <c>begin</c>.
    /// </summary>
    Begin,

    /// <summary>
    /// Appends a string immediately after the formatted value (before suffix).
    /// DSL param: <c>end</c>.
    /// </summary>
    End,

    /// <summary>
    /// Format string applied to the value. The value must implement <c>IFormattable</c>
    /// (e.g. <c>"F2"</c>). Do not use for <c>string</c>.
    /// DSL param: <c>tostring</c>.
    /// </summary>
    ToString,

    /// <summary>
    /// Prepends a string before the value.
    /// DSL param: <c>prefix</c>.
    /// </summary>
    Prefix,

    /// <summary>
    /// Appends a string after the value.
    /// DSL param: <c>suffix</c>.
    /// </summary>
    Suffix
}
