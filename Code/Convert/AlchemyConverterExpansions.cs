// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using SeanOne.Alchemy.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeanOne.Alchemy
{
    partial class AlchemyConverter
    {
        /// <summary>
        /// 取得 Convert 相關的同步執行函數字典
        /// </summary>
        /// <remarks>
        /// 字典鍵值對應：
        /// - "cnv", "convert": 執行 CNV 轉換
        /// - "fe", "foreach", "basic": 執行 AlchemyFormatter 格式化
        /// </remarks>
        private static readonly Dictionary<string, Func<object, string, AlchemyResult>> s_ActionsSync = 
            new Dictionary<string, Func<object, string, AlchemyResult>>
            {
                ["cnv"] = (object copyObj, string dslInstruction) => CNV(copyObj, dslInstruction),
                ["convert"] = (object copyObj, string dslInstruction) => CNV(copyObj, dslInstruction),
                ["fe"] = (object copyObj, string dslInstruction) => AlchemyResult.Parse(AlchemyFormatter.Format(copyObj, dslInstruction)),
                ["foreach"] = (object copyObj, string dslInstruction) => AlchemyResult.Parse(AlchemyFormatter.Format(copyObj, dslInstruction)),
                ["basic"] = (object copyObj, string dslInstruction) => AlchemyResult.Parse(AlchemyFormatter.Format(copyObj, dslInstruction))
            };

        /// <summary>
        /// 取得 Convert 相關的非同步執行函數字典
        /// </summary>
        /// <remarks>
        /// 字典鍵值對應：
        /// - "cnv", "convert": 執行 CNV_Async 非同步轉換
        /// - "fe", "foreach", "basic": 執行 AlchemyFormatter 格式化(包裝為 Task)
        /// </remarks>
        private static readonly Dictionary<string, Func<object, string, Task<AlchemyResult>>> s_ActionsAsync = 
            new Dictionary<string, Func<object, string, Task<AlchemyResult>>>
            {
                ["cnv"] = (object copyObj, string dslInstruction) => CNV_Async(copyObj, dslInstruction),
                ["convert"] = (object copyObj, string dslInstruction) => CNV_Async(copyObj, dslInstruction),
                ["fe"] = (object copyObj, string dslInstruction) => Task.FromResult(AlchemyResult.Parse(AlchemyFormatter.Format(copyObj, dslInstruction))),
                ["foreach"] = (object copyObj, string dslInstruction) => Task.FromResult(AlchemyResult.Parse(AlchemyFormatter.Format(copyObj, dslInstruction))),
                ["basic"] = (object copyObj, string dslInstruction) => Task.FromResult(AlchemyResult.Parse(AlchemyFormatter.Format(copyObj, dslInstruction)))
            };

        /// <summary>
        /// 溫度轉換指令與對應函數的映射字典
        /// 支援兩種指令格式: 
        /// <list type="bullet">
        /// <item><description>"XtoY" 格式: 例如 "KtoC" (凱氏轉攝氏)、"FtoC" (華氏轉攝氏)</description></item>
        /// <item><description>"X->Y" 格式: 例如 "K->C"、 "F->C"，使用箭頭符號增強可讀性</description></item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// 字典使用 <see cref="StringComparer.InvariantCultureIgnoreCase"/> 進行比較，
        /// 因此指令不區分大小寫 (例如 "ktoC"、"K->c" 皆可正確匹配)
        /// </remarks>
        private static readonly Dictionary<string, Func<double, double>> s_ActionsTemperature =
            new Dictionary<string, Func<double, double>>(StringComparer.InvariantCultureIgnoreCase)
            {
                ["KtoC"] = TemperatureConverter.KtoC,
                ["KtoF"] = TemperatureConverter.KtoF,
                ["FtoC"] = TemperatureConverter.FtoC,
                ["FtoK"] = TemperatureConverter.FtoK,
                ["CtoK"] = TemperatureConverter.CtoK,
                ["CtoF"] = TemperatureConverter.CtoF,

                ["K->C"] = TemperatureConverter.KtoC,
                ["K->F"] = TemperatureConverter.KtoF,
                ["F->C"] = TemperatureConverter.FtoC,
                ["F->K"] = TemperatureConverter.FtoK,
                ["C->K"] = TemperatureConverter.CtoK,
                ["C->F"] = TemperatureConverter.CtoF,
            };

        /// <summary>
        /// 轉換清單中的溫度數值
        /// </summary>
        /// <param name="obj">目標物件(必須是 IList)</param>
        /// <param name="ins">轉換指令(例如 "CtoF")</param>
        /// <exception cref="ArgumentException">參數無效時拋出</exception>
        /// <exception cref="InvalidOperationException">轉換失敗時拋出</exception>
        private static void TemperatureConversion(object obj, string ins)
        {
            // 檢查物件是否為 IList。
            if (!(obj is IList list))
            {
                throw new ArgumentException("Object must be of type IList", nameof(obj));
            }

            // 避免後續錯誤處理，先檢查清單是否為空
            if (list.Count == 0) return;
            // 避免後續錯誤處理，先檢查指令是否為空
            if (string.IsNullOrWhiteSpace(ins)) return;

            if (!s_ActionsTemperature.TryGetValue(ins, out var action))
            {
                throw new ArgumentException($"Unknown conversion instruction: {ins}", nameof(ins));
            }

            // 取得清單的實際元素類型 (只做一次)
            Type elementType;
            try
            {
                elementType = list.GetType().IsArray
                    ? list.GetType().GetElementType()
                    : list.GetType().GetGenericArguments()[0];
            }
            catch
            {
                throw new ArgumentException("Cannot determine element type of the list", nameof(obj));
            }

            // 逐個元素進行轉換
            for (int i = 0; i < list.Count; i++)
            {
                try
                {
                    // 處理空值
                    if (list[i] == null)
                    {
                        throw new InvalidOperationException($"Element at index {i} is null, cannot convert");
                    }

                    // 嘗試轉換為 double
                    double value = System.Convert.ToDouble(list[i]);
                    double result = action(value);

                    // 轉換回原始Type
                    list[i] = System.Convert.ChangeType(result, elementType);
                }
                catch (FormatException)
                {
                    throw new InvalidOperationException(
                        $"Element at index {i} ('{list[i]}') cannot be converted to a numeric value");
                }
                catch (InvalidCastException)
                {
                    throw new InvalidOperationException(
                        $"Element at index {i} (type: {list[i]?.GetType()}) cannot be converted to a numeric value");
                }
                catch (OverflowException)
                {
                    throw new InvalidOperationException(
                        $"Conversion result exceeds the range of {elementType.Name}");
                }
            }
        }
    }
}
