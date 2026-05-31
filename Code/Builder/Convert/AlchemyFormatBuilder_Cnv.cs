// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

#if BETA
using SeanOne.Alchemy.Definitions;
using System.Text;

namespace SeanOne.Alchemy.Builder
{
    /// <summary>
    /// Implementation of <c>IAlchemyFunction&lt;CnvParam&gt;</c> for conversion instructions.
    /// </summary>
    public class CnvFunc : IAlchemyFunction<CnvParam>
    {
        // 暫存的字串
        private readonly StringBuilder _sb = new StringBuilder();

        /// <summary>
        /// 初始化，先添加 cnv
        /// </summary>
        internal CnvFunc()
        {
            _sb.Append("cnv ");
        }

        /// <summary>
        /// Implementation of <c>IAlchemyFunction&lt;CnvParam&gt;</c> for conversion instructions.
        /// Handles conversion DSL parameters and formatting logic.
        /// </summary>
        /// <param name="param">The DSL parameter to configure.</param>
        /// <param name="value">The value associated with the parameter.</param>
        /// <returns>The current DSL function instance for chaining.</returns>
        public IAlchemyFunction<CnvParam> With(CnvParam param, string value)
        {
            // 轉譯 value 後，依序加入參數及其值
            value = DslSyntaxBuilder.EscapeDslValue(value);
            _sb.AppendParam(param.ToCnvParamString());
            _sb.AppendQuoted(value);

            return this; // 回傳自己，支援 Fluent DSL
        }


        /// <summary>
        /// Builds the DSL function into a <see cref="AlchemyExecutable"/> instance.
        /// </summary>
        /// <returns>The constructed <see cref="AlchemyExecutable"/>.</returns>
        public AlchemyExecutable Build()
        {
            // 將累積的 DSL 指令轉為可執行物件
            return new AlchemyExecutable(_sb.ToString());
        }
    }
}
#endif
