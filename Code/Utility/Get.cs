// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Text.RegularExpressions;

namespace SeanOne.Alchemy.Utility
{
    /// <summary>
    /// 取得 DSL 指令的值
    /// </summary>
    internal static class Get
    {
        /// <summary>
        /// 從 dslInstruction 中提取參數的值
        /// </summary>
        /// <param name="dslInstruction"> Dsl 指令(要被提取的字串) </param>
        /// <param name="parameterName"> 要提取的參數名稱 </param>
        public static string ExtractParameterValue(string dslInstruction, string parameterName)
        {
            if (string.IsNullOrEmpty(dslInstruction) || string.IsNullOrEmpty(parameterName)) // 如果任一字串為空，直接回傳空字串
                return string.Empty;

            if (Judge.ValidateSingleParameter(dslInstruction, parameterName)) // 檢查參數是否出現多次
            {
                // 如果參數出現多次，拋出例外
                throw new ArgumentException($"Parameter '{parameterName}' is specified multiple times.");
            }

            int startIndex = dslInstruction.IndexOf(parameterName); // 找到參數名稱的位置
            if (startIndex == -1) // 如果找不到參數名稱，回傳空字串
                return string.Empty;

            startIndex += parameterName.Length;

            // 跳過空白字符
            while (startIndex < dslInstruction.Length && char.IsWhiteSpace(dslInstruction[startIndex]))
            {
                startIndex++;
            }

            if (startIndex >= dslInstruction.Length) // 如果已經到達字串末尾，回傳空字串
                return string.Empty;

            // 檢查是否以引號開頭
            if (dslInstruction[startIndex] == '"')
            {
                startIndex++; // 跳過開頭的引號
                int endIndex = dslInstruction.IndexOf('"', startIndex);
                if (endIndex == -1)
                {
                    // 如果找不到結尾的引號，則取到字串末尾
                    endIndex = dslInstruction.Length;
                }

                string value = dslInstruction.Substring(startIndex, endIndex - startIndex); // 擷取參數值
                value = ConvertToUnicode.DecodeUnicodeEscapes(value); // 轉換 Unicode
                return ConvertToUnicode.Unescape(value); // 處理轉義字元
            }
            else
            {
                // 非引號開頭，取到下一個空白或 '/' 為止
                int endIndex = FindNextTerminator(dslInstruction, startIndex); // 尋找終止符位置
                string value = dslInstruction.Substring(startIndex, endIndex - startIndex); // 擷取參數值
                value = ConvertToUnicode.DecodeUnicodeEscapes(value); // 轉換 Unicode
                return ConvertToUnicode.Unescape(value); // 處理轉義字元
            }
        }

        /// <summary>
        /// 取得參數值，如果未找到參數或參數為空，則傳回預設值
        /// </summary>
        /// <param name="dslInstruction"> Dsl 指令(要被提取的字串) </param>
        /// <param name="parameterName"> 要提取的參數名稱 </param>
        /// <param name="defaultValue"> 預設值 </param>
        public static string ParameterValueOrDefault(string dslInstruction, string parameterName, string defaultValue)
        {
            string value = ExtractParameterValue(dslInstruction, parameterName);
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }

        /// <summary>
        /// 尋找下一個終止字元（空格或“/”）
        /// </summary>
        /// <param name="dslInstruction"> Dsl 指令(要被查找的字串) </param>
        /// <param name="startIndex"> 開始查找的位置 </param>
        private static int FindNextTerminator(string dslInstruction, int startIndex)
        {
            // 如果要查找的位置大於字串本身長度，直接返回字串長度
            if (startIndex >= dslInstruction.Length)
                return dslInstruction.Length;

            // 從 startIndex 開始取子字串
            string sub = dslInstruction.Substring(startIndex);

            // 使用正則尋找第一個空白或 '/'
            var match = Regex.Match(sub, $@"[\s{Regex.Escape(DslSymbols.ParamPrefix)}]");
            return match.Success ? startIndex + match.Index : dslInstruction.Length; // 如果沒有找到終止符，則回傳字串的結尾
        }

        /// <summary>
        /// 從 dslInstruction 中提取指令的名稱，最前面的那一個字串
        /// </summary>
        /// <param name="dslInstruction"> Dsl 指令(要被提取的字串)，但本身必須要被Trim過一遍 </param>
        public static string ExtractDirective(string dslInstruction)
        {
            return dslInstruction.Contains(DslSymbols.ParamPrefix) ?
                    dslInstruction.Substring(0, dslInstruction.IndexOf(DslSymbols.ParamPrefix)).Trim()
                    : dslInstruction;
        }
    }
}
