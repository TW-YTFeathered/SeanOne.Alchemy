// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System.Text;

namespace SeanOne.Alchemy.Definitions;

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
        // 如果 value 是空的則直接回傳
        if (string.IsNullOrEmpty(value))
            return value;

        char q0 = DslSymbols.QuoteSymbol[0];
        if (value.IndexOf(q0) < 0)
            return value; // 沒命中就直接回傳

        StringBuilder escaped = new();

        foreach (char ch in value)
        {
            if (ch == q0)
                escaped.Append(DslSymbols.UnicodeEscapePrefix)
                    .Append(((int)ch).ToString($"X{DslSymbols.UnicodeEscapeHexDigits}"));
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
