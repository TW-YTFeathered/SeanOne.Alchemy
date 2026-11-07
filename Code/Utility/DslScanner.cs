// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using SeanOne.Alchemy.Definitions;
using System.Collections.Generic;

namespace SeanOne.Alchemy.Utility;

/// <summary>
/// DSL 字串掃描工具，提供底層的詞法比對
/// </summary>
internal static class DslScanner
{
    /// <summary>
    /// 檢查來源字串從指定位置開始，是否剛好等於目標字串 (Ordinal 比較)
    /// </summary>
    /// <param name="source"> 要被檢查的來源字串 </param>
    /// <param name="startIndex"> 開始比對的位置 </param>
    /// <param name="value"> 要比對的字串 (如引號符號或參數前綴等) </param>
    public static bool MatchAt(this string source, int startIndex, string value) =>
        startIndex + value.Length <= source.Length &&
            string.CompareOrdinal(source, startIndex, value, 0, value.Length) == 0;

    /// <summary>
    /// 從指定位置開始，尋找下一個終止字元 (空白或 ParamPrefix)
    /// </summary>
    /// <param name="source"> 要被查找的來源字串 </param>
    /// <param name="startIndex"> 開始查找的位置 </param>
    public static int FindNextTerminator(this string source, int startIndex)
    {
        string prefix = DslSymbols.ParamPrefix;
        for (int i = startIndex; i < source.Length; i++)
        {
            char c = source[i];
            if (char.IsWhiteSpace(c) || prefix.IndexOf(c) >= 0)
                return i;
        }
        return source.Length;
    }

    /// <summary>
    /// 單次掃描：找出 <paramref name="pattern"/> 在'引號外'的出現次數 (最多數到 2 就提前結束)
    /// </summary>
    /// <param name="source"> 要被查找的來源字串 </param>
    /// <param name="pattern"> 要查找的目標字串 </param>
    /// <param name="firstIndex"> 若找到，要回傳的第一個出現位置，否則回傳 -1 </param>
    public static int FindUnquotedOccurrence(this string source, string pattern, out int firstIndex)
    {
        firstIndex = -1;
        int ptnLen = pattern.Length;
        int last = source.Length - ptnLen;
        if (last < 0)
            return 0;

        string quote = DslSymbols.QuoteSymbol;
        char q0 = quote[0];
        char p0 = pattern[0];
        bool quoted = false;
        bool escaped = false;
        int count = 0;

        for (int i = 0; i < source.Length; i++)
        {
            char c = source[i];

            // 先判斷比對 (此時 quoted 就是原版的 insideQuote[i])
            if (!quoted && i <= last && c == p0 &&
                string.CompareOrdinal(source, i, pattern, 0, ptnLen) == 0)
            {
                if (++count == 1)
                    firstIndex = i;
                else
                    return count; // 已重複，不必再掃
            }

            if (escaped)
                escaped = false;
            else if (c == DslSymbols.EscapeChar)
                escaped = true;
            else if (c == q0 && source.MatchAt(i, quote))
            {
                quoted = !quoted;
                i += quote.Length - 1;
            }
        }

        return count;
    }

    /// <summary>
    /// 單次掃描：列出所有 '引號外' 的參數名稱（/ 後接 [\w-]+）。
    /// </summary>
    /// <param name="source"> 要被掃描的 DSL 字串 </param>
    public static IEnumerable<string> ExtractParameterNames(this string source)
    {
        if (string.IsNullOrEmpty(source))
            yield break;

        string quote = DslSymbols.QuoteSymbol;
        string prefix = DslSymbols.ParamPrefix;
        char q0 = quote[0];
        bool quoted = false;
        bool escaped = false;

        for (int i = 0; i < source.Length; i++)
        {
            char c = source[i];

            if (escaped)
            {
                escaped = false;
                continue;
            }
            if (c == DslSymbols.EscapeChar)
            {
                escaped = true;
                continue;
            }
            if (c == q0 && source.MatchAt(i, quote))
            {
                quoted = !quoted;
                i += quote.Length - 1;
                continue;
            }
            if (!quoted && source.MatchAt(i , prefix))
            {
                int start = i + prefix.Length;
                int j = start;
                while (j < source.Length && IsParamNameChar(source[j]))
                    j++;

                if (j > start)
                {
                    yield return source.Substring(start, j - start);
                    i = j - 1; // 讓外層迴圈的 i++ 可以直接跳到參數尾端之後
                }
            }
        }
    }

    private static bool IsParamNameChar(char c)
        => char.IsLetterOrDigit(c) || c == '_' || c == '-';
}
