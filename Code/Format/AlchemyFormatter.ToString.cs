// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections;
using System.Linq;
using System.Text;
using SeanOne.Alchemy.Utility;

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
            string directive = Get.ExtractDirective(dslInstruction);

            // 嘗試從字典中獲取對應的執行函數
            if (s_ActionsSync.TryGetValue(directive, out var func))
            {
                // 找到並執行函數
                return func(obj, dslInstruction);
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
            string prefix = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey("prefix"), string.Empty);
            string suffix = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey("suffix"), string.Empty);

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
                    return FE_ProcessDictionary_Optimized(dictionary,
                        dictFormat, keyFormat, valueFormat,
                        end, final_pair_separator,
                        prefix, suffix,
                        exclude_last_end);
                else
                    return FE_ProcessDictionary(dictionary,
                        dictFormat, keyFormat, valueFormat,
                        end, final_pair_separator,
                        prefix, suffix,
                        exclude_last_end);
            }

            // 處理普通集合類型
            if (!Judge.ValidateCodeParametersAuto(dslInstruction, enumerable.GetType(), out var invalidParamsForEnum))
                throw new ArgumentException($"Invalid parameters for enumerable processing: {string.Join(", ", invalidParamsForEnum)}");

            if (fe_opt)
                return FE_ProcessEnumerable_Optimized(enumerable, 
                    format, end, final_pair_separator,
                    prefix, suffix,
                    exclude_last_end);
            else
                return FE_ProcessEnumerable(enumerable,
                    format, end, final_pair_separator,
                    prefix, suffix,
                    exclude_last_end);
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
        /// <param name="prefix"> 最前面的前綴，會加在整個字典處理結果的開頭(即使字典為空也會添加) </param>
        /// <param name="suffix"> 最後面的後綴，會加在整個字典處理結果的結尾(即使字典為空也會添加) </param>
        /// <param name="exclude_last_end"> 是否排除最後一個項目的 end 字串 </param>
        private static string FE_ProcessDictionary(IDictionary dictionary, 
            string dictFormat, string keyFormat, string valueFormat, 
            string end, string final_pair_separator,
            string prefix, string suffix,
            bool exclude_last_end)
        {
            if (string.IsNullOrEmpty(dictFormat))
                throw new ArgumentNullException("'dict-format' parameter is required when processing dictionaries.");

            var results = new StringBuilder();

            // 獲取字典的鍵集合
            ICollection keys = dictionary.Keys;
            int count = keys.Count;

            var keyList = keys.Cast<object>().ToList();

            results.Append(prefix);

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

            results.Append(suffix);

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
        /// <param name="prefix"> 最前面的前綴，會加在整個字典處理結果的開頭(即使字典為空也會添加) </param>
        /// <param name="suffix"> 最後面的後綴，會加在整個字典處理結果的結尾(即使字典為空也會添加) </param>
        /// <param name="exclude_last_end"> 是否排除最後一個項目的 end 字串 </param>
        private static string FE_ProcessDictionary_Optimized(IDictionary dictionary,
            string dictFormat, string keyFormat, string valueFormat,
            string end, string final_pair_separator,
            string prefix, string suffix,
            bool exclude_last_end)
        {
            if (dictionary == null) return prefix + suffix;
            if (string.IsNullOrEmpty(dictFormat))
                throw new ArgumentNullException("'dict-format' parameter is required when processing dictionaries.");

            var results = new StringBuilder();
            var enumerator = dictionary.GetEnumerator();

            try
            {
                // 嘗試讀取第一個元素
                if (!enumerator.MoveNext()) return prefix + suffix;

                // 字典使用 DictionaryEntry 來同時獲取 Key 和 Value
                DictionaryEntry currentItem = (DictionaryEntry)enumerator.Current; // 當前要處理的元素
                bool hasNextItem = enumerator.MoveNext(); // 檢查是否還有更多元素

                // 如果只有一個元素
                if (!hasNextItem)
                {
                    results.Append(prefix);

                    string keyStr = FormatObject(currentItem.Key, keyFormat);
                    string valueStr = FormatObject(currentItem.Value, valueFormat);
                    results.Append(Dict_Format(dictFormat, keyStr, valueStr));

                    if (!exclude_last_end) results.Append(end);

                    results.Append(suffix);

                    return results.ToString();
                }

                results.Append(prefix);

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

            results.Append(suffix);

            return results.ToString();
        }

        /// <summary>
        /// 處理普通集合
        /// </summary>
        /// <param name="enumerable"> 目標集合 </param>
        /// <param name="format"> 指定集合的格式化方式 </param>
        /// <param name="end"> 每次跌代後加的字串(如果是倒數第二個且 final_pair_separator 為空字串，或是最後一個且 exclude_last_end 為 true，則不加) </param>
        /// <param name="final_pair_separator"> 用於倒數第二個與最後一個項目之間的連接字串 </param>
        /// <param name="prefix"> 最前面的前綴，會加在整個集合處理結果的開頭(即使集合為空也會添加) </param>
        /// <param name="suffix"> 最後面的後綴，會加在整個集合處理結果的結尾(即使集合為空也會添加) </param>
        /// <param name="exclude_last_end"> 是否排除最後一個項目的 end 字串 </param>
        private static string FE_ProcessEnumerable(IEnumerable enumerable, 
            string format, string end, string final_pair_separator,
            string prefix, string suffix,
            bool exclude_last_end)
        {
            // 用來存儲格式化後的結果
            var results = new StringBuilder();

            // 將 IEnumerable 轉成 IList 以支援索引存取
            var list = enumerable.Cast<object>().ToList();
            int count = list.Count;

            results.Append(prefix);

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

            results.Append(suffix);

            return results.ToString();
        }

        /// <summary>
        /// 處理普通集合 (優化過後)
        /// </summary>
        /// <param name="enumerable"> 目標集合 </param>
        /// <param name="format"> 指定集合的格式化方式 </param>
        /// <param name="end"> 每次跌代後加的字串(如果是倒數第二個且 final_pair_separator 為空字串，或是最後一個且 exclude_last_end 為 true，則不加) </param>
        /// <param name="final_pair_separator"> 用於倒數第二個與最後一個項目之間的連接字串 </param>
        /// <param name="prefix"> 最前面的前綴，會加在整個字典處理結果的開頭(即使字典為空也會添加) </param>
        /// <param name="suffix"> 最後面的後綴，會加在整個字典處理結果的結尾(即使字典為空也會添加) </param>
        /// <param name="exclude_last_end"> 是否排除最後一個項目的 end 字串 </param>
        private static string FE_ProcessEnumerable_Optimized(IEnumerable enumerable,
            string format, string end, string final_pair_separator,
            string prefix, string suffix,
            bool exclude_last_end)
        {
            if (enumerable == null) return prefix + suffix;

            var result = new StringBuilder();
            var enumerator = enumerable.GetEnumerator();

            try
            {
                // 嘗試讀取第一個元素
                if (!enumerator.MoveNext()) return prefix + suffix;

                object currentItem = enumerator.Current; // 當前要處理的元素
                bool hasNextItem = enumerator.MoveNext(); // 檢查是否還有更多元素

                // 如果只有一個元素
                if (!hasNextItem)
                {
                    result.Append(prefix);

                    result.Append(FormatObject(currentItem, format));
                    if (!exclude_last_end) result.Append(end);
                    
                    result.Append(suffix);
                    
                    return result.ToString();
                }

                result.Append(prefix);

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
                            result.Append(currentStr).Append(final_pair_separator);
                        // 否則添加 end
                        else
                            result.Append(currentStr).Append(end);

                        // 處理最後一個元素 (nextItem)
                        result.Append(FormatObject(nextItem, format));

                        // 如果是最後一個，且 exclude_last_end 為 false 才加 end
                        if (!exclude_last_end) result.Append(end);
                    }
                    else
                    {
                        // 一般中間的元素
                        result.Append(currentStr).Append(end);
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

            result.Append(suffix);

            return result.ToString();
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
            string prefix = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey("prefix"), string.Empty);
            string suffix = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey("suffix"), string.Empty);

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

            return prefix + result + end + suffix;
        }
        #endregion
    }
}
