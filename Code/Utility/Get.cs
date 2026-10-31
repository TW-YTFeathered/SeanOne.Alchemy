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
        int count = FindUnquotedOccurrence(dslInstruction, parameterName, out int index);

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
        if (MatchAt(dslInstruction, startIndex, quote))
        {
            startIndex += quote.Length;
            endIndex = startIndex;
            while (endIndex < length)
            {
                char c = dslInstruction[endIndex];
                if (c == '\\')
                {
                    // 跳過被轉義的字元
                    endIndex += 2;
                    continue;
                }
                if (c == q0 && MatchAt(dslInstruction, endIndex, quote))
                    break;
                endIndex++;
            }
            if (endIndex > length)
                endIndex = length;
        }
        else
            endIndex = FindNextTerminator(dslInstruction, startIndex);

        string value = dslInstruction.Substring(startIndex, endIndex - startIndex);
        return value.DecodeUnicodeEscapes().Unescape();
    }

    /// <summary>
    /// 單次掃描：找出 pattern 在'引號外'的出現次數 (最多數到 2 就提前結束)
    /// </summary>
    private static int FindUnquotedOccurrence(string text, string pattern, out int firstIndex)
    {
        firstIndex = -1;
        int ptnLen = pattern.Length;
        int last = text.Length - ptnLen;
        if (last < 0)
            return 0;

        string quote = DslSymbols.QuoteSymbol;
        char q0 = quote[0];
        char p0 = pattern[0];
        bool quoted = false;
        bool escaped = false;
        int count = 0;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];

            // 先判斷比對 (此時 quoted 就是原版的 insideQuote[i])
            if (!quoted && i <= last && c == p0 &&
                string.CompareOrdinal(text, i, pattern, 0, ptnLen) == 0)
            {
                if (++count == 1)
                    firstIndex = i;
                else
                    return count; // 已重複，不必再掃
            }

            if (escaped)
                escaped = false;
            else if (c == '\\')
                escaped = true;
            else if (c == q0 && MatchAt(text, i, quote))
            {
                quoted = !quoted;
                i += quote.Length - 1;
            }
        }

        return count;
    }

    /// <summary>
    /// 檢查 text 從 index 開始的位置，是否剛好等於 token (Ordinal 比較)
    /// </summary>
    /// <param name="text"> 要被檢查的字串 </param>
    /// <param name="index"> 開始比對的位置 </param>
    /// <param name="token"> 要比對的字串 (如引號符號) </param>
    private static bool MatchAt(string text, int index, string token) =>
        index + token.Length <= text.Length && 
            string.CompareOrdinal(text, index, token, 0, token.Length) == 0;

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
    /// 尋找下一個終止字元 (空白或 ParamPrefix)，不再建立 Substring 與 Regex
    /// </summary>
    /// <param name="dslInstruction"> Dsl 指令(要被查找的字串) </param>
    /// <param name="startIndex"> 開始查找的位置 </param>
    private static int FindNextTerminator(string dslInstruction, int startIndex)
    {
        string prefix = DslSymbols.ParamPrefix;
        for (int i = startIndex; i < dslInstruction.Length; i++)
        {
            char c = dslInstruction[i];
            if (char.IsWhiteSpace(c) || prefix.IndexOf(c) >= 0)
                return i;
        }
        return dslInstruction.Length;
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
