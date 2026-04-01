// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using SeanOne.Alchemy.Definitions;
using System.Text;

namespace SeanOne.Alchemy.Builder
{
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

            _sb.AppendParam(param.ToFeSeqParamString());

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

            _sb.AppendParam(param.ToFeDictParamString());

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
