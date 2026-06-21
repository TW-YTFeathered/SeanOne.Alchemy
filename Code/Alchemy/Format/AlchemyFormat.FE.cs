// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using SeanOne.Alchemy.Definitions;
using SeanOne.Alchemy.Utility;
using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace SeanOne.Alchemy
{
    partial class Alchemy
    {
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
            string begin = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey(CommonParams.Begin), string.Empty);
            string end = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey(CommonParams.End), string.Empty);
            string final_pair_separator = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey(IEnumerableParams.FinalPairSeparator), string.Empty);
            string format = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey(CommonParams.Tostring), string.Empty);
            string dictFormat = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey(IDictionaryParams.DictFormat), string.Empty);
            string keyFormat = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey(IDictionaryParams.KeyFormat), string.Empty);
            string valueFormat = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey(IDictionaryParams.ValueFormat), string.Empty);
            string prefix = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey(CommonParams.Prefix), string.Empty);
            string suffix = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey(CommonParams.Suffix), string.Empty);

            // 提取並解析 exclude-last-end 參數
            bool exclude_last_end = false;
            string excludeLastEndValue = Get.ExtractParameterValue(dslInstruction, DslSyntaxBuilder.BuildParamKey(IEnumerableParams.ExcludeLastEnd));
            if (!string.IsNullOrEmpty(excludeLastEndValue) &&
                bool.TryParse(excludeLastEndValue, out bool parsedEndPrint))
            {
                exclude_last_end = parsedEndPrint;
            }

            // 提取並解析 fe-opt
            bool fe_opt = false;
            string feOptValue = Get.ExtractParameterValue(dslInstruction, DslSyntaxBuilder.BuildParamKey(FeParams.FeOpt));
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
                        begin, end, final_pair_separator,
                        prefix, suffix,
                        exclude_last_end);
                else
                    return FE_ProcessDictionary(dictionary,
                        dictFormat, keyFormat, valueFormat,
                        begin, end, final_pair_separator,
                        prefix, suffix,
                        exclude_last_end);
            }

            // 處理普通集合類型
            if (!Judge.ValidateCodeParametersAuto(dslInstruction, enumerable.GetType(), out var invalidParamsForEnum))
                throw new ArgumentException($"Invalid parameters for enumerable processing: {string.Join(", ", invalidParamsForEnum)}");

            if (fe_opt)
                return FE_ProcessEnumerable_Optimized(enumerable,
                    format,
                    begin, end, final_pair_separator,
                    prefix, suffix,
                    exclude_last_end);
            else
                return FE_ProcessEnumerable(enumerable,
                    format,
                    begin, end, final_pair_separator,
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
        /// <param name="begin"> 每次跌代前加的字串</param>
        /// <param name="end"> 每次跌代後加的字串(如果是倒數第二個且 final_pair_separator 為空字串，或是最後一個且 exclude_last_end 為 true，則不加) </param>
        /// <param name="final_pair_separator"> 用於倒數第二個與最後一個項目之間的連接字串 </param>
        /// <param name="prefix"> 最前面的前綴，會加在整個字典處理結果的開頭(即使字典為空也會添加) </param>
        /// <param name="suffix"> 最後面的後綴，會加在整個字典處理結果的結尾(即使字典為空也會添加) </param>
        /// <param name="exclude_last_end"> 是否排除最後一個項目的 end 字串 </param>
        private static string FE_ProcessDictionary(IDictionary dictionary,
            string dictFormat, string keyFormat, string valueFormat,
            string begin, string end, string final_pair_separator,
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

                results.Append(begin);

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
        /// 處理普通集合
        /// </summary>
        /// <param name="enumerable"> 目標集合 </param>
        /// <param name="format"> 指定集合的格式化方式 </param>
        /// <param name="begin"> 每次跌代前加的字串</param>
        /// <param name="end"> 每次跌代後加的字串(如果是倒數第二個且 final_pair_separator 為空字串，或是最後一個且 exclude_last_end 為 true，則不加) </param>
        /// <param name="final_pair_separator"> 用於倒數第二個與最後一個項目之間的連接字串 </param>
        /// <param name="prefix"> 最前面的前綴，會加在整個集合處理結果的開頭(即使集合為空也會添加) </param>
        /// <param name="suffix"> 最後面的後綴，會加在整個集合處理結果的結尾(即使集合為空也會添加) </param>
        /// <param name="exclude_last_end"> 是否排除最後一個項目的 end 字串 </param>
        private static string FE_ProcessEnumerable(IEnumerable enumerable,
            string format,
            string begin, string end, string final_pair_separator,
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

                results.Append(begin);

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
    }
}
