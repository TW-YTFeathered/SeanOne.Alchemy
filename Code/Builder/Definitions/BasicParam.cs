// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

namespace SeanOne.Alchemy.Builder;

/// <summary>
/// Defines the parameter options used by <c>IBasicAlchemyFunction</c> (basic).
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
    /// Appends a string after each value.
    /// DSL param: <c>end</c>.
    /// </summary>
    End,

    /// <summary>
    /// Applies formatting to items implementing <c>IFormattable</c>. Not applicable to dictionaries. Use C#'s <c>ToString()</c> method.
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
