using System;
using System.Text;

namespace SeanOne.Alchemy
{
    /// <summary>
    /// DSL 符號定義類別，集中管理 DSL 語法解析中使用的各種符號常數。
    /// </summary>
    internal class DslSymbols
    {
        /// <summary>
        /// 引號符號，用於包裹字串值，標示其開始與結束。
        /// </summary>
        public static string QuoteSymbol { get; } = "\"";

        /// <summary>
        /// 參數前綴符號，用於標示 DSL 參數的起始。
        /// </summary>
        public static string ParamPrefix { get; } = "/";

        /// <summary>
        /// 參數分隔符號，用於分隔參數名稱與其對應的值。
        /// </summary>
        public static string ParamSeparator { get; } = ":";
    }

    /// <summary>
    /// DSL 語法建置工具類別，
    /// 提供建立參數鍵、添加參數值與符號處理等功能，
    /// 主要用於 DSL 語法解析的建置過程，並兼具部分通用性。
    /// </summary>
    internal static class DslSyntaxBuilder
    {
        /// <summary>
        /// 新增參數值
        /// </summary>
        /// <param name="value"> 要添加的值 </param>
        /// <param name="sb"> 要修改的 <c>StringBuilder</c> </param>
        public static StringBuilder AppendQuoted(this StringBuilder sb, string value)
            => sb.Append(DslSymbols.QuoteSymbol).Append(value).Append(DslSymbols.QuoteSymbol);

        /// <summary>
        /// 新增參數
        /// </summary>
        /// <param name="param"> 要添加的參數名稱 </param>
        /// <param name="sb"> 要修改的 <c>StringBuilder</c> </param>
        public static StringBuilder AppendParam(this StringBuilder sb, string param)
            => sb.Append(DslSymbols.ParamPrefix).Append(param).Append(DslSymbols.ParamSeparator);

        /// <summary>
        /// 將可能導致解析錯誤的字元轉換為安全字元
        /// </summary>
        /// <param name="value"> 要解析的字串 </param>
        public static string EscapeDslValue(string value)
        {
            StringBuilder escaped = new StringBuilder();

            foreach (char ch in value)
            {
                if (ch == '\"')
                    escaped.Append("\\u0022");
                else
                    escaped.Append(ch);
            }
            return escaped.ToString();
        }

        /// <summary>
        /// 建立 DSL 參數鍵
        /// </summary>
        /// <param name="paramName"> 要建置的字串 </param>
        public static string BuildParamKey(string paramName)
            => DslSymbols.ParamPrefix + paramName + DslSymbols.ParamSeparator;
    }
}
