using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SeanOne.Alchemy
{
    /// <summary>
    /// 判斷 DSL 條件
    /// </summary>
    internal static class Judge
    {
        /// <summary>
        /// 檢查物件是否實作 IFormattable 介面
        /// </summary>
        /// <param name="obj"> 要檢查的物件 </param>
        public static bool SafeToString(object obj)
        {
            return obj is IFormattable;
        }

        /// <summary>
        /// 檢查 fullStr 是否包含 searchStr
        /// </summary>
        /// <param name="fullStr"> 被檢查的字串 </param>
        /// <param name="searchStr"> 要查詢的字串 </param>
        public static bool HasString(string fullStr, string searchStr)
        {
            if (string.IsNullOrWhiteSpace(fullStr) || string.IsNullOrWhiteSpace(searchStr)) // 如果任一字串為空，直接回傳 false
                return false;

            return fullStr.Contains(searchStr);
        }

        /// <summary>
        /// 檢查某個 參數 是否在 dslInstruction 中出現多次
        /// </summary>
        /// <param name="dslInstruction"> Dsl 指令(要被檢查的字串) </param>
        /// <param name="parameterName"> 要查找的參數名稱 </param>
        public static bool ValidateSingleParameter(string dslInstruction, string parameterName)
        {
            int count = CountParameterOccurrences(dslInstruction, parameterName); // 數數參數出現次數

            return count > 1;
        }

        /// <summary>
        /// 計算 參數 在 dslInstruction 中出現的次數 (使用正則表達式)
        /// </summary>
        /// <param name="dslInstruction"> Dsl 指令(要被檢查的字串) </param>
        /// <param name="parameterName"> 要查找的參數名稱 </param>
        private static int CountParameterOccurrences(string dslInstruction, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(dslInstruction) || string.IsNullOrWhiteSpace(parameterName))
                return 0;

            // 移除所有 \"...\" 之間的內容（非貪婪）
            string withoutQuotes = Regex.Replace(dslInstruction, "\\\\\".*?\\\\\"", string.Empty);

            // 精確匹配參數名稱（前後界限）
            string pattern = $@"(?<=^|\s){Regex.Escape(parameterName)}(?=\s|:|$)";
            return Regex.Matches(withoutQuotes, pattern).Count;
        }

        // 定義每個方法支援的參數
        private static readonly Dictionary<string, HashSet<string>> MethodParameters = new Dictionary<string, HashSet<string>>
        {
            ["basic"] = new HashSet<string> { "end", "tostring" },
            ["FE_ProcessEnumerable"] = new HashSet<string> { "end", "final-pair-separator", "exclude-last-end", "tostring" },
            ["FE_ProcessDictionary"] = new HashSet<string> { "end", "final-pair-separator", "exclude-last-end", "dict-format", "key-format", "value-format" }
        };

        /// <summary>
        /// 根據方法類型驗證參數
        /// </summary>
        /// <param name="dslInstruction"> Dsl 指令(要被檢查的字串) </param>
        /// <param name="methodType"> 方法類型(用字串表示) </param>
        /// <param name="invalidParams"> 要回傳的無效參數列表 </param>
        public static bool ValidateCodeParameters(string dslInstruction, string methodType, out List<string> invalidParams)
        {
            invalidParams = new List<string>();

            if (string.IsNullOrWhiteSpace(dslInstruction))
                return true;

            if (!MethodParameters.ContainsKey(methodType))
                throw new KeyNotFoundException($"Unsupported method types: {methodType}");

            var validParams = MethodParameters[methodType];

            // 1. 移除所有引號內的內容（允許跳脫字元）
            string withoutQuotes = Regex.Replace(dslInstruction, "\"(?:\\\\.|[^\"])*\"", string.Empty);

            // 2. 正則匹配引號外的參數
            var parameterPattern = @"(?<=/)([\w-]+)(?::([^/\s]*))?";
            var matches = Regex.Matches(withoutQuotes, parameterPattern);

            foreach (Match match in matches)
            {
                string paramName = match.Groups[1].Value;
                if (!validParams.Contains(paramName))
                {
                    invalidParams.Add(paramName);
                }
            }

            return invalidParams.Count == 0;
        }

        /// <summary>
        /// 自動偵測方法類型並驗證參數
        /// </summary>
        /// <param name="dslInstruction"> Dsl 指令(要被檢查的字串) </param>
        /// <param name="objectType"> 方法類型(用類型表示) </param>
        /// <param name="invalidParams"> 要回傳的無效參數列表 </param>
        public static bool ValidateCodeParametersAuto(string dslInstruction, Type objectType, out List<string> invalidParams)
        {
            string methodType = DetermineMethodType(objectType); // 根據物件類型自動判斷方法
            return ValidateCodeParameters(dslInstruction, methodType, out invalidParams); // 驗證參數
        }

        /// <summary>
        /// 根據物件類型自動判斷應該使用的方法
        /// </summary>
        /// <param name="objectType"> 要判斷的物件類型 </param>
        private static string DetermineMethodType(Type objectType)
        {
            if (objectType == null)
                return "basic"; // 預設為 basic

            // 如果是字典類型
            if (objectType.IsGenericType &&
                (objectType.GetGenericTypeDefinition() == typeof(Dictionary<,>) ||
                 objectType.GetGenericTypeDefinition() == typeof(IDictionary<,>) ||
                 objectType.GetInterfaces().Any(i => i.IsGenericType &&
                     i.GetGenericTypeDefinition() == typeof(IDictionary<,>))))
            {
                return "FE_ProcessDictionary";
            }

            // 如果是可枚舉類型（但不是字符串）
            if (objectType != typeof(string) &&
                (objectType.GetInterfaces().Contains(typeof(System.Collections.IEnumerable)) ||
                 objectType.IsArray))
            {
                return "FE_ProcessEnumerable";
            }

            // 預設為 basic
            return "basic";
        }
    }

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
    }

    /// <summary>
    /// 將指定輸入轉換成 Unicode 值
    /// 注意：部分情況直接使用 C# 數值轉換更快更準，可能看似非標準 Unicode
    /// </summary>
    internal static class ConvertToUnicode
    {
        /// <summary>
        /// 將字串中的所有 \uXXXX 轉義序列轉換為其對應的 Unicode 字符
        /// </summary>
        /// <param name="input"> 要被轉換的字串 </param>
        public static string DecodeUnicodeEscapes(string input)
        {
            // 使用正則表達式來尋找所有的 \\uXXXX 序列
            return Regex.Replace(input, @"\\u([0-9a-fA-F]{4})", match =>
            {
                try
                {
                    // 將十六進位值轉換為對應的 Unicode 字符
                    return ((char)Convert.ToInt32(match.Groups[1].Value, 16)).ToString();
                }
                catch
                {
                    // 如果轉換失敗，保留原始字串
                    return match.Value;
                }
            });
        }

        /// <summary>
        /// 將字串中的所有 \X 轉義序列轉換為其對應的 Unicode 字符
        /// </summary>
        /// <param name="input"> 要被轉換的字串 </param>
        public static string Unescape(string input)
        {
            // 如果為空，直接返回 string.Empty
            if (string.IsNullOrEmpty(input)) return string.Empty; 

            var sb = new StringBuilder(); // 用於存放結果的字串

            int count = input.Count(); // 取得字串長度

            for (int i = 0; i < count; i++)
            {
                char thisChar = input[i]; // 取得當前字符

                if (thisChar == '\\' && i + 1 < count)
                {
                    char nextChar = input[i + 1]; // 取得下一個字符

                    switch (nextChar) 
                    {
                        case '0': sb.Append('\0'); break;
                        case 'a': sb.Append('\a'); break;
                        case 'b': sb.Append('\b'); break;
                        case 'f': sb.Append('\f'); break;
                        case 'n': sb.Append(Environment.NewLine); break; // 避免環境差異，使用 Environment.NewLine
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        case 'v': sb.Append('\v'); break;
                        case '\\': sb.Append('\\'); break;
                        case '\'': sb.Append('\''); break;
                        case '\"': sb.Append('\"'); break;
                        default: sb.Append(thisChar).Append(nextChar); break;
                    }

                    i++;
                }
                else
                {
                    sb.Append(thisChar); // 非轉義字符，直接添加
                }
            }

            return sb.ToString(); // 回傳轉換結果
        }
    }
}
