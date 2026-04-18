// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using SeanOne.Alchemy.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SeanOne.Alchemy
{
    partial class AlchemyFormatter
    {
        /// <summary>
        /// 取得 Format 相關的同步執行函數字典
        /// </summary>
        /// <remarks>
        /// 字典鍵值對應：
        /// - "fe", "foreach": 執行 FE 函數 (帶對應的類型參數)
        /// - "basic": 執行 Basic 函數
        /// </remarks>
        private static readonly Dictionary<string, Func<object, string, string>> s_ActionsSync =
            new Dictionary<string, Func<object, string, string>>
            {
                ["fe"] = (obj, dslInstruction) => FE(obj, dslInstruction, "fe"),
                ["foreach"] = (obj, dslInstruction) => FE(obj, dslInstruction, "foreach"),
                ["basic"] = (obj, dslInstruction) => Basic(obj, dslInstruction),
            };

        #region Sync
        /// <summary>
        /// 驗證集合元素是否可格式化
        /// </summary>
        /// <param name="enumerable"> 要檢查的集合 </param>
        /// <param name="format"> 格式化字串(目前沒用) </param>
        private static void ValidateEnumerableFormattable(IEnumerable enumerable, string format)
        {
            foreach (var element in enumerable)
            {
                if (element != null && !Judge.SafeToString(element))
                {
                    var elementType = element.GetType();
                    throw new ArgumentException($"Collection elements must implement IFormattable for 'tostring'. Found: {elementType.Name}");
                }
            }
        }

        /// <summary>
        /// 格式化對象 (需支持 IFormattable)
        /// </summary>
        /// <param name="obj"> 要格式化的對象 </param>
        /// <param name="format"> 格式化字串 </param>
        private static string FormatObject(object obj, string format)
        {
            // 如果對象為null，返回空字符串
            if (obj == null) return string.Empty;

            // 如果提供了格式且對象實現了IFormattable，則使用該格式
            if (!string.IsNullOrEmpty(format) && obj is IFormattable formattable)
                return formattable.ToString(format, null);

            // 否則，使用默認的ToString方法
            return obj.ToString() ?? string.Empty;
        }

        private static readonly Regex _placeholderRegex =
            new Regex(@"\{(\d+)\}", RegexOptions.Compiled);
        /// <summary>
        /// 格式化字典
        /// </summary>
        /// <param name="format"> 格式化字串 </param>
        /// <param name="key"> 鍵 </param>
        /// <param name="value"> 值 </param>
        private static string Dict_Format(string format, string key, string value)
        {
            if (string.IsNullOrEmpty(format)) return string.Empty;

            var dict = new Dictionary<string, object>
            {
                { "0", key },
                { "1", value }
            };

            return _placeholderRegex.Replace(format, m =>
            {
                var index = m.Groups[1].Value;
                return dict.TryGetValue(index, out var val) ? val.ToString() : m.Value;
            });
        }
        #endregion
    }
}
