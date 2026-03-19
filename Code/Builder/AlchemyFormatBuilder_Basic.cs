// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using SeanOne.Alchemy.Definitions;
using System;
using System.Text;

namespace SeanOne.Alchemy.Builder
{
    /// <summary>
    /// Defines the parameter options used by <c>IBasicAlchemyFunction</c> (basic).
    /// Handles basic DSL parameters and formatting logic.
    /// </summary>
    public enum BasicParam
    {
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
    /// Implementation of <c>IBasicAlchemyFunction</c> using <c>BasicParam</c>.
    /// Handles basic DSL parameters and formatting logic.
    /// </summary>
    public class BasicFunc : IBasicAlchemyFunction<BasicParam>
    {
        // 暫存的字串
        private readonly StringBuilder _sb = new StringBuilder();

        /// <summary>
        /// 初始化，先添加 basic
        /// </summary>
        internal BasicFunc()
        {
            _sb.Append("basic ");
        }

        /// <summary>
        /// Adds a parameter and its associated value to the DSL function.
        /// </summary>
        /// <param name="param">The DSL parameter to configure.</param>
        /// <param name="value">The value associated with the parameter.</param>
        /// <returns>The current DSL function instance for chaining.</returns>
        public IAlchemyFunction<BasicParam> With(BasicParam param, string value)
        {
            value = DslSyntaxBuilder.EscapeDslValue(value);

            switch (param)
            {
                case BasicParam.ToString:
                    _sb.AppendParam(CommonParams.Tostring);
                    break;
                case BasicParam.End:
                    _sb.AppendParam(CommonParams.End);
                    break;
                case BasicParam.Prefix:
                    _sb.AppendParam(CommonParams.Prefix);
                    break;
                case BasicParam.Suffix:
                    _sb.AppendParam(CommonParams.Suffix);
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
