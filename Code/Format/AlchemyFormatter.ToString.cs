using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SeanOne.Alchemy
{
    partial class AlchemyFormatter
    {
        /// <summary>
        /// 解碼並選擇適當的方法
        /// </summary>
        /// <param name="obj"> 目標物件 </param>
        /// <param name="dslInstruction"> Dsl 指令 </param>
        private static string Decoder(object obj, string dslInstruction)
        {
            // 從 DSL 指令中提取函數名稱
            string directive = dslInstruction.Contains(DslSymbols.ParamPrefix) ?
                dslInstruction.Substring(0, dslInstruction.IndexOf(DslSymbols.ParamPrefix)).Trim()
                : dslInstruction;

            // 創建函數名稱字典，映射到對應的執行函數
            Dictionary<string, Func<string>> actions = new Dictionary<string, Func<string>>
            {
                ["fe"] = () => FE(obj, dslInstruction, "fe"),
                ["foreach"] = () => FE(obj, dslInstruction, "foreach"),
                ["basic"] = () => Basic(obj, dslInstruction),
            };

            // 嘗試從字典中獲取對應的執行函數
            if (actions.TryGetValue(directive, out var func))
            {
                // 找到並執行函數
                return func();
            }
            else
            {
                // 如果指令以 DSL 定義的參數前綴符號（目前為 '/'）開頭，則執行 Basic 方法
                if (dslInstruction.StartsWith(DslSymbols.ParamPrefix))
                {
                    return Basic(obj, dslInstruction);
                }
                else
                {
                    // 未知指令拋出異常
                    throw new MissingMethodException($"Unknown functions directive: {directive}");
                }
            }
        }

        #region FE Method
        /// <summary>
        /// 將 IEnumerable 轉換為字串
        /// </summary>
        /// <param name="obj"> 目標物件 </param>
        /// <param name="dslInstruction"> Dsl 指令 </param>
        /// <param name="commandName"> 呼叫時的指令名稱(僅用做 throw 時) </param>
        private static string FE(object obj, string dslInstruction, string commandName)
        {
            if (obj == null)
                throw new ArgumentNullException($"Target object cannot be null for '{commandName}' directive");
            if (obj is string)
                throw new ArgumentException($"String is not supported for '{commandName}' directive");
            if (!(obj is IEnumerable enumerable))
                throw new ArgumentException($"Object must implement IEnumerable for '{commandName}' directive");

            // 提前提取所有參數
            string end = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey("end"), string.Empty);
            string final_pair_separator = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey("final-pair-separator"), string.Empty);
            string format = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey("tostring"), string.Empty);
            string dictFormat = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey("dict-format"), string.Empty);
            string keyFormat = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey("key-format"), string.Empty);
            string valueFormat = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey("value-format"), string.Empty);

            // 提取並解析 exclude-last-end 參數
            bool exclude_last_end = false;
            string excludeLastEndValue = Get.ExtractParameterValue(dslInstruction, DslSyntaxBuilder.BuildParamKey("exclude-last-end"));
            if (!string.IsNullOrEmpty(excludeLastEndValue) &&
                bool.TryParse(excludeLastEndValue, out bool parsedEndPrint))
            {
                exclude_last_end = parsedEndPrint;
            }

            // 提取並解析 fe-opt
            bool fe_opt = false;
            string feOptValue = Get.ExtractParameterValue(dslInstruction, DslSyntaxBuilder.BuildParamKey("fe-opt"));
            if (!string.IsNullOrEmpty(feOptValue) &&
                bool.TryParse(feOptValue, out bool parsedOptPrint)) 
            {
                fe_opt = parsedOptPrint;
            }

            // 驗證格式參數
            if (!string.IsNullOrEmpty(format))
            {
                ValidateEnumerableFormattable(enumerable, format);
            }

            // 處理字典類型
            if (obj is IDictionary dictionary)
            {
                if (!Judge.ValidateCodeParametersAuto(dslInstruction, dictionary.GetType(), out var invalidParams))
                    throw new ArgumentException($"Invalid parameters for dictionary processing: {string.Join(", ", invalidParams)}");

                if (fe_opt)
                    return FE_ProcessDictionary_Optimized(dictionary, dictFormat, keyFormat, valueFormat, end, final_pair_separator, exclude_last_end);
                else
                    return FE_ProcessDictionary(dictionary, dictFormat, keyFormat, valueFormat, end, final_pair_separator, exclude_last_end);
            }

            // 處理普通集合類型
            if (!Judge.ValidateCodeParametersAuto(dslInstruction, enumerable.GetType(), out var invalidParamsForEnum))
                throw new ArgumentException($"Invalid parameters for enumerable processing: {string.Join(", ", invalidParamsForEnum)}");

            if (fe_opt)
                return FE_ProcessEnumerable_Optimized(enumerable, format, end, final_pair_separator, exclude_last_end);
            else
                return FE_ProcessEnumerable(enumerable, format, end, final_pair_separator, exclude_last_end);
        }

        /// <summary>
        /// 處理字典集合
        /// </summary>
        /// <param name="dictionary"> 目標字典 </param>
        /// <param name="dictFormat"> 字典格式 </param>
        /// <param name="keyFormat"> 字典的鍵格式 </param>
        /// <param name="valueFormat"> 字典的值格式 </param>
        /// <param name="end"> 每次跌代後加的字串(如果是倒數第二個且 final_pair_separator 為空字串，或是最後一個且 exclude_last_end 為 true，則不加) </param>
        /// <param name="final_pair_separator"> 用於倒數第二個與最後一個項目之間的連接字串 </param>
        /// <param name="exclude_last_end"> 是否排除最後一個項目的 end 字串 </param>
        private static string FE_ProcessDictionary(IDictionary dictionary, string dictFormat, string keyFormat, string valueFormat, string end, string final_pair_separator, bool exclude_last_end)
        {
            if (string.IsNullOrEmpty(dictFormat))
                throw new ArgumentNullException("'dict-format' parameter is required when processing dictionaries.");

            var results = new StringBuilder();

            // 獲取字典的鍵集合
            ICollection keys = dictionary.Keys;
            int count = keys.Count;

            var keyList = keys.Cast<object>().ToList();

            for (int i = 0; i < count; i++)
            {
                var key = keyList[i];
                var value = dictionary[key];

                string keyStr = FormatObject(key, keyFormat);
                string valueStr = FormatObject(value, valueFormat);

                string formatted = Dict_Format(dictFormat, keyStr, valueStr);

                // 如果是倒數第二個，且 final_pair_separator 不為 null 或空字串
                if (i == count - 2 && !string.IsNullOrEmpty(final_pair_separator))
                    results.Append(formatted).Append(final_pair_separator);
                // 如果是最後一個，且 exclude_last_end 為 true
                else if (i == count - 1 && exclude_last_end)
                    results.Append(formatted); // 不加 end
                else
                    results.Append(formatted).Append(end);
            }

            return results.ToString();
        }

        /// <summary>
        /// 處理字典集合 (優化過後)
        /// </summary>
        /// <param name="dictionary"> 目標字典 </param>
        /// <param name="dictFormat"> 字典格式 </param>
        /// <param name="keyFormat"> 字典的鍵格式 </param>
        /// <param name="valueFormat"> 字典的值格式 </param>
        /// <param name="end"> 每次跌代後加的字串(如果是倒數第二個且 final_pair_separator 為空字串，或是最後一個且 exclude_last_end 為 true，則不加) </param>
        /// <param name="final_pair_separator"> 用於倒數第二個與最後一個項目之間的連接字串 </param>
        /// <param name="exclude_last_end"> 是否排除最後一個項目的 end 字串 </param>
        private static string FE_ProcessDictionary_Optimized(IDictionary dictionary, string dictFormat, string keyFormat, string valueFormat, string end, string final_pair_separator, bool exclude_last_end)
        {
            if (dictionary == null) return string.Empty;
            if (string.IsNullOrEmpty(dictFormat))
                throw new ArgumentNullException("'dict-format' parameter is required when processing dictionaries.");

            var results = new StringBuilder();
            var enumerator = dictionary.GetEnumerator();

            try
            {
                // 嘗試讀取第一個元素
                if (!enumerator.MoveNext()) return string.Empty;

                // 字典使用 DictionaryEntry 來同時獲取 Key 和 Value
                DictionaryEntry currentItem = (DictionaryEntry)enumerator.Current; // 當前要處理的元素
                bool hasNextItem = enumerator.MoveNext(); // 檢查是否還有更多元素

                // 如果只有一個元素
                if (!hasNextItem)
                {
                    string keyStr = FormatObject(currentItem.Key, keyFormat);
                    string valueStr = FormatObject(currentItem.Value, valueFormat);
                    results.Append(Dict_Format(dictFormat, keyStr, valueStr));

                    if (!exclude_last_end) results.Append(end);
                    return results.ToString();
                }

                // 處理多個元素
                while (hasNextItem)
                {
                    // 讀取下一個元素到 nextItem
                    DictionaryEntry nextItem = (DictionaryEntry)enumerator.Current; // 下一個要處理的元素
                    // 嘗試讀取下下個元素，結果存入 hasNextItem
                    hasNextItem = enumerator.MoveNext();

                    // 處理當前元素 (currentItem)
                    string keyStr = FormatObject(currentItem.Key, keyFormat);
                    string valueStr = FormatObject(currentItem.Value, valueFormat);
                    string formatted = Dict_Format(dictFormat, keyStr, valueStr);

                    if (!hasNextItem)
                    {
                        // 當前是倒數第二個元素，且 final_pair_separator 不為 null 或空字串
                        if (!string.IsNullOrEmpty(final_pair_separator))
                            results.Append(formatted).Append(final_pair_separator);
                        // 否則添加 end
                        else
                            results.Append(formatted).Append(end);

                        // 處理最後一個元素 (nextItem)
                        string lastKeyStr = FormatObject(nextItem.Key, keyFormat);
                        string lastValueStr = FormatObject(nextItem.Value, valueFormat);
                        results.Append(Dict_Format(dictFormat, lastKeyStr, lastValueStr));

                        // 如果是最後一個，且 exclude_last_end 為 false 才加 end
                        if (!exclude_last_end) results.Append(end);
                    }
                    else
                    {
                        // 一般中間元素
                        results.Append(formatted).Append(end);
                    }

                    // 移動到下一輪要處理的元素
                    currentItem = nextItem;
                }
            }
            finally
            {
                // 釋放資源
                if (enumerator is IDisposable disposable) disposable.Dispose();
            }

            return results.ToString();
        }

        /// <summary>
        /// 處理普通集合
        /// </summary>
        /// <param name="enumerable"> 目標集合 </param>
        /// <param name="format"> 指定集合的格式化方式 </param>
        /// <param name="end"> 每次跌代後加的字串(如果是倒數第二個且 final_pair_separator 為空字串，或是最後一個且 exclude_last_end 為 true，則不加) </param>
        /// <param name="final_pair_separator"> 用於倒數第二個與最後一個項目之間的連接字串 </param>
        /// <param name="exclude_last_end"> 是否排除最後一個項目的 end 字串 </param>
        private static string FE_ProcessEnumerable(IEnumerable enumerable, string format, string end, string final_pair_separator, bool exclude_last_end)
        {
            // 用來存儲格式化後的結果
            var results = new StringBuilder();

            // 將 IEnumerable 轉成 IList 以支援索引存取
            var list = enumerable.Cast<object>().ToList();
            int count = list.Count;

            for (int i = 0; i < count; i++)
            {
                string itemString = FormatObject(list[i], format);

                // 如果是倒數第二個，且 final_pair_separator 不為 null 或空字串
                if (i == count - 2 && !string.IsNullOrEmpty(final_pair_separator))
                    results.Append(itemString).Append(final_pair_separator);
                // 如果是最後一個，且 exclude_last_end 為 true
                else if (i == count - 1 && exclude_last_end)
                    results.Append(itemString); // 不加 end
                else
                    results.Append(itemString).Append(end);
            }

            return results.ToString();
        }

        /// <summary>
        /// 處理普通集合 (優化過後)
        /// </summary>
        /// <param name="enumerable"> 目標集合 </param>
        /// <param name="format"> 指定集合的格式化方式 </param>
        /// <param name="end"> 每次跌代後加的字串(如果是倒數第二個且 final_pair_separator 為空字串，或是最後一個且 exclude_last_end 為 true，則不加) </param>
        /// <param name="final_pair_separator"> 用於倒數第二個與最後一個項目之間的連接字串 </param>
        /// <param name="exclude_last_end"> 是否排除最後一個項目的 end 字串 </param>
        private static string FE_ProcessEnumerable_Optimized(IEnumerable enumerable, string format, string end, string final_pair_separator, bool exclude_last_end)
        {
            if (enumerable == null) return string.Empty;

            var sb = new StringBuilder();
            var enumerator = enumerable.GetEnumerator();

            try
            {
                // 嘗試讀取第一個元素
                if (!enumerator.MoveNext()) return string.Empty;

                object currentItem = enumerator.Current; // 當前要處理的元素
                bool hasNextItem = enumerator.MoveNext(); // 檢查是否還有更多元素

                // 如果只有一個元素
                if (!hasNextItem)
                {
                    sb.Append(FormatObject(currentItem, format));
                    if (!exclude_last_end) sb.Append(end);
                    return sb.ToString();
                }

                // 處理多個元素
                while (hasNextItem)
                {
                    // 讀取下一個元素到 nextItem
                    object nextItem = enumerator.Current; // 下一個要處理的元素
                    // 嘗試讀取下下個元素，結果存入 hasNextItem
                    hasNextItem = enumerator.MoveNext();

                    // 處理當前元素 (currentItem)
                    string currentStr = FormatObject(currentItem, format);

                    if (!hasNextItem)
                    {
                        // 當前是倒數第二個元素，且 final_pair_separator 不為 null 或空字串
                        if (!string.IsNullOrEmpty(final_pair_separator))
                            sb.Append(currentStr).Append(final_pair_separator);
                        // 否則添加 end
                        else
                            sb.Append(currentStr).Append(end);

                        // 處理最後一個元素 (nextItem)
                        sb.Append(FormatObject(nextItem, format));

                        // 如果是最後一個，且 exclude_last_end 為 false 才加 end
                        if (!exclude_last_end) sb.Append(end);
                    }
                    else
                    {
                        // 一般中間的元素
                        sb.Append(currentStr).Append(end);
                    }

                    // 移動到下一輪要處理的元素
                    currentItem = nextItem;
                }
            }
            finally
            {
                // 釋放資源
                if (enumerator is IDisposable disposable) disposable.Dispose();
            }

            return sb.ToString();
        }
        #endregion

        #region Basic Method
        /// <summary>
        /// 處理單個對象的格式化 (非同步)
        /// </summary>
        /// <param name="obj"> 目標物件 </param>
        /// <param name="dslInstruction"> Dsl 指令 </param>
        private static string Basic(object obj, string dslInstruction)
        {
            string format = string.Empty;

            string end = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey("end"), string.Empty);

            // 提取並驗證 /tostring: 參數
            if (Judge.HasString(dslInstruction, DslSyntaxBuilder.BuildParamKey("tostring")))
            {
                format = Get.ExtractParameterValue(dslInstruction, DslSyntaxBuilder.BuildParamKey("tostring"));

                // 驗證 obj 是否實作 IFormattable
                if (obj != null && !Judge.SafeToString(obj))
                    throw new ArgumentException($"Collection elements must implement IFormattable for 'tostring'. Found: {obj.GetType().Name}");
            }

            if (!Judge.ValidateCodeParameters(dslInstruction, "basic", out var invalidParams))
                throw new ArgumentException($"Invalid parameters for basic processing: {string.Join(", ", invalidParams)}");

            // 格式化對象
            string result = FormatObject(obj, format);

            return result + end;
        }
        #endregion

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
    }
}
