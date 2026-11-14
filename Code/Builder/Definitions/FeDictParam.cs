// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

namespace SeanOne.Alchemy.Builder;

/// <summary>
/// Defines the parameter options used by <c>FeDictionaryFunctionBuilder</c> (fe).
/// Handles dictionary-specific DSL parameters and formatting logic.
/// </summary>
public enum FeDictParam
{
    /// <summary>
    /// Format string for dictionary entries. 
    /// <para>This parameter is <b>required</b> when formatting a dictionary.</para>
    /// Use <c>{0}</c> to represent the key and <c>{1}</c> to represent the value.
    /// DSL param: <c>dict-format</c>.
    /// </summary>
    DictFormat,

    /// <summary>
    /// Prepends a string before each dictionary entry.
    /// DSL param: <c>begin</c>.
    /// </summary>
    Begin,

    /// <summary>
    /// Appends a string after each dictionary entry.
    /// DSL param: <c>end</c>.
    /// </summary>
    End,

    /// <summary>
    /// Omits the end string after the last item.
    /// DSL param: <c>exclude-last-end</c>.
    /// </summary>
    ExcludeLastEnd,

    /// <summary>
    /// Replaces the separator between the last two items in the dictionary. Falls back to <c>end</c> if not specified.
    /// DSL param: <c>final-pair-separator</c>.
    /// </summary>
    FinalPairSeparator,

    /// <summary>
    /// Format string applied to dictionary keys.
    /// DSL param: <c>key-format</c>.
    /// </summary>
    KeyFormat,

    /// <summary>
    /// Format string applied to dictionary values.
    /// DSL param: <c>value-format</c>.
    /// </summary>
    ValueFormat,

    /// <summary>
    /// Prepends a string before the whole result.
    /// DSL param: <c>prefix</c>.
    /// </summary>
    Prefix,

    /// <summary>
    /// Appends a string after the whole result.
    /// DSL param: <c>suffix</c>.
    /// </summary>
    Suffix,

    /// <summary>
    /// Enable optimized formatting (may have compatibility issues).
    /// DSL param: <c>fe-opt</c>.
    /// </summary>
    FeOpt
}
