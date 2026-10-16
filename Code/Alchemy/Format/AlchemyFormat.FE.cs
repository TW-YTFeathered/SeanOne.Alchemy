// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

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
            if (obj is null)
                throw new ArgumentNullException($"Target object cannot be null for '{commandName}' directive");
            if (obj is string)
                throw new ArgumentException($"String is not supported for '{commandName}' directive");
            if (!(obj is IEnumerable enumerable))
                throw new ArgumentException($"Object must implement IEnumerable for '{commandName}' directive");

            // 提前提取所有參數
            var options = FeOptions.Parse(dslInstruction);

            // 驗證格式參數
            if (!string.IsNullOrEmpty(options.tostring))
                ValidateEnumerableFormattable(enumerable, options.tostring);

            // 處理字典類型
            if (obj is IDictionary dictionary)
            {
                Judge.EnsureParameters(dslInstruction, "dictionary", dictionary.GetType());

                if (options.fe_opt)
                    return FE_ProcessDictionary_Optimized(dictionary, options);
                else
                    return FE_ProcessDictionary(dictionary, options);
            }

            // 處理普通集合類型
            Judge.EnsureParameters(dslInstruction, "enumerable", enumerable.GetType());

            if (options.fe_opt)
                return FE_ProcessEnumerable_Optimized(enumerable, options);
            else
                return FE_ProcessEnumerable(enumerable, options);
        }

        /// <summary>
        /// 處理字典集合
        /// </summary>
        /// <param name="dictionary"> 目標字典 </param>
        /// <param name="options"> DSL 參數集合 </param>
        private static string FE_ProcessDictionary(IDictionary dictionary, in FeOptions options)
        {
            if (string.IsNullOrEmpty(options.dict_format))
                throw new ArgumentNullException("'dict-format' parameter is required when processing dictionaries.");

            StringBuilder results = new();

            // 獲取字典的鍵集合
            ICollection keys = dictionary.Keys;
            int count = keys.Count;

            var keyList = keys.Cast<object>().ToList();

            results.Append(options.prefix);

            for (int i = 0; i < count; i++)
            {
                var key = keyList[i];
                var value = dictionary[key];

                string keyStr = FormatObject(key, options.key_format);
                string valueStr = FormatObject(value, options.value_format);

                string formatted = Dict_Format(options.dict_format, keyStr, valueStr);

                results.Append(options.begin);

                // 如果是倒數第二個，且 final_pair_separator 不為 null 或空字串
                if (i == count - 2 && !string.IsNullOrEmpty(options.final_pair_separator))
                    results.Append(formatted).Append(options.final_pair_separator);
                // 如果是最後一個，且 exclude_last_end 為 true
                else if (i == count - 1 && options.exclude_last_end)
                    results.Append(formatted); // 不加 end
                else
                    results.Append(formatted).Append(options.end);
            }

            results.Append(options.suffix);

            return results.ToString();
        }

        /// <summary>
        /// 處理普通集合
        /// </summary>
        /// <param name="enumerable"> 目標集合 </param>
        /// <param name="options"> DSL 參數集合 </param>
        private static string FE_ProcessEnumerable(IEnumerable enumerable, in FeOptions options)
        {
            // 用來存儲格式化後的結果
            StringBuilder results = new();

            // 將 IEnumerable 轉成 IList 以支援索引存取
            var list = enumerable.Cast<object>().ToList();
            int count = list.Count;

            results.Append(options.prefix);

            for (int i = 0; i < count; i++)
            {
                string itemString = FormatObject(list[i], options.tostring);

                results.Append(options.begin);

                // 如果是倒數第二個，且 final_pair_separator 不為 null 或空字串
                if (i == count - 2 && !string.IsNullOrEmpty(options.final_pair_separator))
                    results.Append(itemString).Append(options.final_pair_separator);
                // 如果是最後一個，且 exclude_last_end 為 true
                else if (i == count - 1 && options.exclude_last_end)
                    results.Append(itemString); // 不加 end
                else
                    results.Append(itemString).Append(options.end);
            }

            results.Append(options.suffix);

            return results.ToString();
        }
    }
}
