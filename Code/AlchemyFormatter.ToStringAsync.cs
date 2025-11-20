using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeanOne.Alchemy
{
    partial class AlchemyFormatter
    {
        // 用來鎖定的物件
        private static readonly object _lock = new object();

        /// <summary>
        /// 解碼並選擇適當的方法 (非同步)
        /// </summary>
        /// <param name="obj"> 目標物件 </param>
        /// <param name="dslInstruction"> Dsl 指令 </param>
        private async static Task<string> Decoder_Async(object obj, string dslInstruction)
        {
            // 從 DSL 指令中提取函數名稱
            string directive = dslInstruction.Contains(DslSymbols.ParamPrefix) ? 
                dslInstruction.Substring(0, dslInstruction.IndexOf(DslSymbols.ParamPrefix)).Trim()
                : dslInstruction;

            // 創建函數名稱字典，映射到對應的執行函數
            var actions = new Dictionary<string, Func<Task<string>>>
            {
                ["fe"] = () => FE_Async(obj, dslInstruction, "fe"),
                ["foreach"] = () => FE_Async(obj, dslInstruction, "foreach"),
                ["basic"] = () => Basic_Async(obj, dslInstruction),
            };

            // 嘗試從字典中獲取對應的執行函數
            if (actions.TryGetValue(directive, out var func))
            {
                // 找到並執行函數
                return await func();
            }
            else
            {
                // 如果指令以 DSL 定義的參數前綴符號（目前為 '/'）開頭，則執行 Basic 方法
                if (dslInstruction.StartsWith(DslSymbols.ParamPrefix))
                {
                    return await Basic_Async(obj, dslInstruction);
                }
                else
                {
                    // 未知指令拋出異常
                    throw new MissingMethodException($"Unknown functions directive: {directive}");
                }
            }
        }

        #region FE Method Async
        /// <summary>
        /// 將 IEnumerable 轉換為字串 (非同步)
        /// </summary>
        /// <param name="obj"> 目標物件 </param>
        /// <param name="dslInstruction"> Dsl 指令 </param>
        /// <param name="commandName"> 呼叫時的指令名稱(僅用做 throw 時) </param>
        private async static Task<string> FE_Async(object obj, string dslInstruction, string commandName)
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

            // 提取並解析 /exclude-last-end: 參數
            bool exclude_last_end = false;
            string excludeLastEndValue = Get.ExtractParameterValue(dslInstruction, DslSyntaxBuilder.BuildParamKey("exclude-last-end"));
            if (!string.IsNullOrEmpty(excludeLastEndValue) &&
                bool.TryParse(excludeLastEndValue, out bool parsedEndPrint))
            {
                exclude_last_end = parsedEndPrint;
            }

            // 驗證格式參數
            if (!string.IsNullOrEmpty(format))
            {
                await ValidateEnumerableFormattable_Async(enumerable, format);
            }

            // 處理字典類型
            if (obj is IDictionary dictionary)
            {
                if (!Judge.ValidateCodeParametersAuto(dslInstruction, dictionary.GetType(), out var invalidParams))
                    throw new ArgumentException($"Invalid parameters for dictionary processing: {string.Join(", ", invalidParams)}");

                return await FE_ProcessDictionary_Async(dictionary, dictFormat, keyFormat, valueFormat, end, final_pair_separator, exclude_last_end);
            }

            // 處理普通集合類型
            if (!Judge.ValidateCodeParametersAuto(dslInstruction, enumerable.GetType(), out var invalidParamsForEnum))
                throw new ArgumentException($"Invalid parameters for enumerable processing: {string.Join(", ", invalidParamsForEnum)}");

            return await FE_ProcessEnumerable_Async(enumerable, format, end, final_pair_separator, exclude_last_end);
        }

        /// <summary>
        /// 處理字典集合 (非同步)
        /// </summary>
        /// <param name="dictionary"> 目標字典 </param>
        /// <param name="dictFormat"> 字典格式 </param>
        /// <param name="keyFormat"> 字典的鍵格式 </param>
        /// <param name="valueFormat"> 字典的值格式 </param>
        /// <param name="end"> 每次跌代後加的字串(如果是倒數第二個且 final_pair_separator 為空字串，或是最後一個且 exclude_last_end 為 true，則不加) </param>
        /// <param name="final_pair_separator"> 用於倒數第二個與最後一個項目之間的連接字串 </param>
        /// <param name="exclude_last_end"> 是否排除最後一個項目的 end 字串 </param>
        private static Task<string> FE_ProcessDictionary_Async(IDictionary dictionary, string dictFormat, string keyFormat, string valueFormat, string end, string final_pair_separator, bool exclude_last_end)
        {
            object syncRoot = _lock; // 複製鎖定物件，避免在多線程環境中出現問題

            return Task.Run(() =>
            {
                if (string.IsNullOrEmpty(dictFormat))
                    throw new ArgumentNullException("'dict-format' parameter is required when processing dictionaries");

                var results = new StringBuilder();

                // 獲取字典的鍵集合
                ICollection keys = dictionary.Keys;
                int count = keys.Count;

                var keyList = keys.Cast<object>().ToList();

                // 避免執行續搶，而導致格式化結果不符合預期
                lock (syncRoot)
                {
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
                }

                return results.ToString();
            });
        }

        /// <summary>
        /// 處理普通集合 (非同步)
        /// </summary>
        /// <param name="enumerable"> 目標集合 </param>
        /// <param name="format"> 指定集合的格式化方式 </param>
        /// <param name="end"> 每次跌代後加的字串(如果是倒數第二個且 final_pair_separator 為空字串，或是最後一個且 exclude_last_end 為 true，則不加) </param>
        /// <param name="final_pair_separator"> 用於倒數第二個與最後一個項目之間的連接字串 </param>
        /// <param name="exclude_last_end"> 是否排除最後一個項目的 end 字串 </param>
        private static Task<string> FE_ProcessEnumerable_Async(IEnumerable enumerable, string format, string end, string final_pair_separator, bool exclude_last_end)
        {
            object syncRoot = _lock; // 複製鎖定物件，避免在多線程環境中出現問題

            return Task.Run(() => {
                // 用來存儲格式化後的結果
                var results = new StringBuilder();

                // 將 IEnumerable 轉成 IList 以支援索引存取
                var list = enumerable.Cast<object>().ToList();
                int count = list.Count;

                // 避免執行續搶，而導致格式化結果不符合預期
                lock (syncRoot)
                {
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
                }

                return results.ToString();
            });
        }
        #endregion

        #region Basic Methode Async
        /// <summary>
        /// 處理單個對象的格式化 (非同步)
        /// </summary>
        /// <param name="obj"> 目標物件 </param>
        /// <param name="dslInstruction"> Dsl 指令 </param>
        private static Task<string> Basic_Async(object obj, string dslInstruction)
        {
            return Task.Run(() =>
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
                {
                    throw new ArgumentException($"Invalid parameters for basic processing: {string.Join(", ", invalidParams)}");
                }

                // 格式化對象
                string result = FormatObject(obj, format);

                return result + end;
            });
        }
        #endregion

        /// <summary>
        /// 驗證集合元素是否可格式化 (非同步)
        /// </summary>
        /// <param name="enumerable"> 要檢查的集合 </param>
        /// <param name="format"> 格式化字串(目前沒用) </param>
        private static Task ValidateEnumerableFormattable_Async(IEnumerable enumerable, string format)
        {
            var snapshot = enumerable.Cast<object>().ToList(); // 創建集合快照，避免多線程問題

            return Task.Run(() =>
            {
                foreach (var element in snapshot)
                {
                    if (element != null && !Judge.SafeToString(element))
                    {
                        var elementType = element.GetType();
                        throw new ArgumentException($"Collection elements must implement IFormattable for 'tostring'. Found: {elementType.Name}");
                    }
                }
            });
        }
    }
}
