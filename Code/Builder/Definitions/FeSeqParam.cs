// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

namespace SeanOne.Alchemy.Builder;

/// <summary>
/// Defines the parameter options used by <c>FeSequenceFunctionBuilder</c> (fe).
/// Handles sequence-specific DSL parameters and formatting logic.
/// </summary>
public enum FeSeqParam
{
    /// <summary>
    /// Prepends a string before each element in the sequence.
    /// DSL param: <c>begin</c>.
    /// </summary>
    Begin,

    /// <summary>
    /// Appends a string after each element in the sequence.
    /// DSL param: <c>end</c>.
    /// </summary>
    End,

    /// <summary>
    /// Omits the end string after the last item.
    /// DSL param: <c>exclude-last-end</c>.
    /// </summary>
    ExcludeLastEnd,

    /// <summary>
    /// Replaces the separator between the last two items in the sequence. Falls back to <c>end</c> if not specified.
    /// DSL param: <c>final-pair-separator</c>.
    /// </summary>
    FinalPairSeparator,

    /// <summary>
    /// Format string applied to each element. Each element must implement <c>IFormattable</c>.
    /// Do not use on string elements.
    /// DSL param: <c>tostring</c>.
    /// </summary>
    ToString,

    /// <summary>
    /// Prepends a string before the sequence. Note: Adds to entire result, not each element.
    /// DSL param: <c>prefix</c>.
    /// </summary>
    Prefix,

    /// <summary>
    /// Appends a string after the sequence. Note: Adds to entire result, not each element.
    /// DSL param: <c>suffix</c>.
    /// </summary>
    Suffix,

    /// <summary>
    /// Enable optimized formatting (may have compatibility issues).
    /// DSL param: <c>fe-opt</c>.
    /// </summary>
    FeOpt
}
