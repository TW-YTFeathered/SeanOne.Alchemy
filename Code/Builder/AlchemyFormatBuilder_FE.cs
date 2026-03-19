// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using SeanOne.Alchemy.Definitions;
using System;
using System.Text;

namespace SeanOne.Alchemy.Builder
{
    /// <summary>
    /// Defines the parameter options used by <c>ISequenceAlchemyFunction</c> (fe).
    /// Handles sequence-specific DSL parameters and formatting logic.
    /// </summary>
    public enum FeSeqParam
    {
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
    /// Implementation of <c>ISequenceAlchemyFunction</c> using <c>FeSeqParam</c>.
    /// </summary>
    public class FeSequenceFunc : ISequenceAlchemyFunction<FeSeqParam>
    {
        // 暫存的字串
        private readonly StringBuilder _sb = new StringBuilder();

        /// <summary>
        /// 初始化，先添加 fe
        /// </summary>
        internal FeSequenceFunc()
        {
            _sb.Append("fe ");
        }

        /// <summary>
        /// Adds a parameter and its associated value to the DSL function.
        /// </summary>
        /// <param name="param">The DSL parameter to configure.</param>
        /// <param name="value">The value associated with the parameter.</param>
        /// <returns>The current DSL function instance for chaining.</returns>
        public IAlchemyFunction<FeSeqParam> With(FeSeqParam param, string value)
        {
            value = DslSyntaxBuilder.EscapeDslValue(value);

            switch (param)
            {
                case FeSeqParam.End:
                    _sb.AppendParam(CommonParams.End);
                    break;
                case FeSeqParam.ExcludeLastEnd:
                    _sb.AppendParam(IEnumerableParams.ExcludeLastEnd);
                    break;
                case FeSeqParam.ToString:
                    _sb.AppendParam(CommonParams.Tostring);
                    break;
                case FeSeqParam.FinalPairSeparator:
                    _sb.AppendParam(IEnumerableParams.FinalPairSeparator);
                    break;
                case FeSeqParam.Prefix:
                    _sb.AppendParam(CommonParams.Prefix);
                    break;
                case FeSeqParam.Suffix:
                    _sb.AppendParam(CommonParams.Suffix);
                    break;
                case FeSeqParam.FeOpt:
                    _sb.AppendParam(FeParams.FeOpt);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(param), param, null);
            }
            _sb.AppendQuoted(value);

            return this; // 回傳自己，支援 Fluent DSL
        }

        /// <summary>
        /// Builds the DSL function into a <see cref="AlchemyExecutable"/> instance.
        /// </summary>
        /// <returns>The constructed <see cref="AlchemyExecutable"/>.</returns>
        public AlchemyExecutable Build()
        {
            return new AlchemyExecutable(_sb.ToString());
        }
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
    /// <summary>
    /// Implementation of <c>IDictionaryAlchemyFunction</c> using <c>FeDictParam</c>.
    /// Handles dictionary-specific DSL parameters and formatting logic.
    /// </summary>
    public class FeDictionaryFunc : IDictionaryAlchemyFunction<FeDictParam>
    {
        // 暫存的字串
        private readonly StringBuilder _sb = new StringBuilder();

        /// <summary>
        /// 初始化，先添加 fe
        /// </summary>
        internal FeDictionaryFunc()
        {
            _sb.Append("fe ");
        }

        /// <summary>
        /// Adds a parameter and its associated value to the DSL function.
        /// </summary>
        /// <param name="param">The DSL parameter to configure.</param>
        /// <param name="value">The value associated with the parameter.</param>
        /// <returns>The current DSL function instance for chaining.</returns>
        public IAlchemyFunction<FeDictParam> With(FeDictParam param, string value)
        {
            value = DslSyntaxBuilder.EscapeDslValue(value);

            switch (param)
            {
                case FeDictParam.DictFormat:
                    _sb.AppendParam(IDictionaryParams.DictFormat);
                    break;
                case FeDictParam.End:
                    _sb.AppendParam(CommonParams.End);
                    break;
                case FeDictParam.ExcludeLastEnd:
                    _sb.AppendParam(IEnumerableParams.ExcludeLastEnd);
                    break;
                case FeDictParam.FinalPairSeparator:
                    _sb.AppendParam(IEnumerableParams.FinalPairSeparator);
                    break;
                case FeDictParam.KeyFormat:
                    _sb.AppendParam(IDictionaryParams.KeyFormat);
                    break;
                case FeDictParam.ValueFormat:
                    _sb.AppendParam(IDictionaryParams.ValueFormat);
                    break;
                case FeDictParam.Prefix:
                    _sb.AppendParam(CommonParams.Prefix);
                    break;
                case FeDictParam.Suffix:
                    _sb.AppendParam(CommonParams.Suffix);
                    break;
                case FeDictParam.FeOpt:
                    _sb.AppendParam(FeParams.FeOpt);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(param), param, null);
            }
            _sb.AppendQuoted(value);

            return this; // 回傳自己，支援 Fluent DSL
        }

        /// <summary>
        /// Builds the DSL function into a <see cref="AlchemyExecutable"/> instance.
        /// </summary>
        /// <returns>The constructed <see cref="AlchemyExecutable"/>.</returns>
        public AlchemyExecutable Build()
        {
            return new AlchemyExecutable(_sb.ToString());
        }
    }
}
