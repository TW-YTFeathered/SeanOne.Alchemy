// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SeanOne.Alchemy.Utility
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

            // 移除所有 "..." 之間的內容 (非貪婪)
            string withoutQuotes = Regex.Replace(dslInstruction, "\\\".*?\\\"", string.Empty);

            // 直接匹配參數名稱子串
            return Regex.Matches(withoutQuotes, Regex.Escape(parameterName)).Count;
        }

        // 定義每個方法支援的參數
        private static readonly Dictionary<string, HashSet<string>> MethodParameters = new Dictionary<string, HashSet<string>>
        {
            ["basic"] = new HashSet<string> { "end", "tostring", "prefix", "suffix" },
            ["FE_ProcessEnumerable"] = new HashSet<string> { "end", "final-pair-separator", "exclude-last-end", "fe-opt", "tostring", "prefix", "suffix" },
            ["FE_ProcessDictionary"] = new HashSet<string> { "end", "final-pair-separator", "exclude-last-end", "fe-opt", "dict-format", "key-format", "value-format", "prefix", "suffix" }
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
}
