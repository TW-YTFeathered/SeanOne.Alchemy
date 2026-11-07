// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using SeanOne.Alchemy.Definitions;
using System;

namespace SeanOne.Alchemy.Utility;

/// <summary>
/// DSL 指令解析工具，提供從指令字串取出指令名與參數欄位值的方法。
/// </summary>
internal static class Get
{
    /// <summary>
    /// 從 dslInstruction 中提取參數的值
    /// </summary>
    /// <param name="dslInstruction"> Dsl 指令(要被提取的字串) </param>
    /// <param name="parameterName"> 要提取的參數名稱 </param>
    private static string ParameterValue(string dslInstruction, string parameterName)
    {
        if (string.IsNullOrEmpty(dslInstruction) || string.IsNullOrEmpty(parameterName))
            return string.Empty;

        // 找出'第一個位置'與'是否重複'
        int count = dslInstruction.FindUnquotedOccurrence(parameterName, out int index);

        if (count > 1)
            throw new ArgumentException($"Parameter '{parameterName}' is specified multiple times.");
        if (count == 0)
            return string.Empty;

        int length = dslInstruction.Length;
        int startIndex = index + parameterName.Length;

        while (startIndex < length && char.IsWhiteSpace(dslInstruction[startIndex]))
            startIndex++;

        if (startIndex >= length)
            return string.Empty;

        string quote = DslSymbols.QuoteSymbol;
        char q0 = quote[0];

        int endIndex = 0;
        if (dslInstruction.MatchAt(startIndex, quote))
        {
            startIndex += quote.Length;
            endIndex = startIndex;
            while (endIndex < length)
            {
                char c = dslInstruction[endIndex];
                if (c == DslSymbols.EscapeChar)
                {
                    // 跳過被轉義的字元
                    endIndex += 2;
                    continue;
                }
                if (c == q0 && dslInstruction.MatchAt(endIndex, quote))
                    break;
                endIndex++;
            }
            if (endIndex > length)
                endIndex = length;
        }
        else
            endIndex = dslInstruction.FindNextTerminator(startIndex);

        string value = dslInstruction.Substring(startIndex, endIndex - startIndex);
        return value.DecodeUnicodeEscapes().Unescape();
    }

    /// <summary>
    /// 從 DSL 指令字串中，以參數常數取出對應的欄位值，若該欄位不存在或值為空，則回傳指定的預設值。
    /// </summary>
    /// <param name="dslIns"> DSL 指令字串 </param>
    /// <param name="paramKey"> 參數名稱常數 (如 <see cref="CommonParams.Begin"/>) </param>
    /// <param name="def"> 找不到或值為空時回傳的預設值，預設為空字串 </param>
    /// <returns> 該參數對應的值: 找不到時為 <paramref name="def"/> </returns>
    public static string ParamField(this string dslIns, string paramKey, string def = "")
    {
        string param = DslSyntaxBuilder.BuildParamKey(paramKey);
        string res = ParameterValue(dslIns, param);
        return string.IsNullOrEmpty(res) ? def : res;
    }

    /// <summary>
    /// 從 dslInstruction 中提取指令的名稱，最前面的那一個字串
    /// </summary>
    /// <param name="dslInstruction"> Dsl 指令(要被提取的字串)，但本身必須要被Trim過一遍 </param>
    public static string Directive(string dslInstruction)
    {
        return dslInstruction.Contains(DslSymbols.ParamPrefix) ?
                dslInstruction.Substring(0, dslInstruction.IndexOf(DslSymbols.ParamPrefix)).Trim()
                : dslInstruction;
    }
}
