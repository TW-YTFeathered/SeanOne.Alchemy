// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

namespace SeanOne.Alchemy.Builder
{
#if BETA
    public enum CnvParam { Sort, Temp }
    public enum DirParam { Basic, FeSeq, FeDict, Cnv }
    public enum TemperatureUnit { C, F, K }
    public enum SortAlgorithm { Bubble, Insertion, ArraySort, Linq }
#endif

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

    /// <summary>
    /// Defines the parameter options used by <c>ISequenceAlchemyFunction</c> (fe).
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
        /// Appends a string after each value.
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
        /// Applies formatting to items implementing <c>IFormattable</c>. Not applicable to dictionaries. Use C#'s <c>ToString()</c> method.
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

    /// <summary>
    /// Defines the parameter options used by <c>IDictionaryAlchemyFunction</c> (fe).
    /// Handles dictionary-specific DSL parameters and formatting logic.
    /// </summary>
    public enum FeDictParam
    {
        /// <summary>
        /// Format string for dictionary entries. 
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
        /// Appends a string after each value.
        /// DSL param: <c>end</c>.
        /// </summary>
        End,

        /// <summary>
        /// Omits the end string after the last item.
        /// DSL param: <c>exclude-last-end</c>.
        /// </summary>
        ExcludeLastEnd,

        /// <summary>
        /// Replaces the separator between the last two items in the sequence. Falls back to `end` if not specified.
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
}
