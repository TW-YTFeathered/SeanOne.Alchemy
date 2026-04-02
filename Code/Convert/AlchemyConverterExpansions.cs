// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using SeanOne.Alchemy.Utility;
using System;
using System.Collections;
using System.Collections.Generic;

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
        /// 統一泛型入口：轉換溫度數值或清單中的每個元素
        /// </summary>
        /// <typeparam name="T">輸入的型別，可以是數值型別或實作 IList 的集合型別</typeparam>
        /// <param name="input">要轉換的數值或集合</param>
        /// <param name="instruction">轉換指令 (例如 "CtoF" 或 "K->C")</param>
        /// <returns>
        /// 若輸入為數值，回傳轉換後的數值 (型別為 T)
        /// 若輸入為集合，回傳修改後的集合本身 (原地轉換)
        /// </returns>
        private static T ConvertTemperature<T>(T input, string instruction)
        {
            // 輸入值不能為 null
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            // 若指令為空或空白，直接回傳原值，不進行轉換
            if (string.IsNullOrWhiteSpace(instruction))
                return input;

            // 檢查是否為清單 (包括陣列和泛型集合)
            if (input is IList list)
            {
                ConvertList(list, instruction);
                return input; // 回傳原集合 (已修改)
            }
            else
            {
                // 單一數值處理
                double value = System.Convert.ToDouble(input);
                double result = ConvertSingleValue(value, instruction);
                return (T)System.Convert.ChangeType(result, typeof(T));
            }
        }

        // 私有輔助方法：轉換單一數值
        private static double ConvertSingleValue(double value, string instruction)
        {
            if (!s_ActionsTemperature.TryGetValue(instruction, out var action))
                throw new ArgumentException($"Unknown conversion instruction: {instruction}", nameof(instruction));

            return action(value);
        }

        // 私有輔助方法：轉換 IList 集合 (原地修改)
        private static void ConvertList(IList list, string instruction)
        {
            // 若清單為空，直接返回，無需轉換
            if (list.Count == 0) return;

            // 從靜態字典中取得對應的轉換函式，若找不到對應的轉換函式，則拋出異常
            if (!s_ActionsTemperature.TryGetValue(instruction, out var action))
                throw new ArgumentException($"Unknown conversion instruction: {instruction}", nameof(instruction));

            // 取得清單元素的實際型別：
            // - 若 list 是陣列，使用 GetElementType()
            // - 否則假設 list 是泛型集合 (例如 List<T>)，取第一個泛型引數
            //   注意：若傳入非泛型集合 (如 ArrayList)，將無法取得元素型別而拋出例外
            Type elementType;
            try
            {
                elementType = list.GetType().IsArray
                    ? list.GetType().GetElementType()
                    : list.GetType().GetGenericArguments()[0]; // 僅適用於泛型集合
            }
            catch
            {
                // 當無法取得元素型別時（例如陣列類型為 null，或非泛型集合），
                // 拋出明確的 ArgumentException 提示使用者
                throw new ArgumentException("Cannot determine element type of the list", nameof(list));
            }

            // 逐一轉換每個元素
            for (int i = 0; i < list.Count; i++)
            {
                try
                {
                    // 元素為 null 時無法轉換
                    if (list[i] == null)
                        throw new InvalidOperationException($"Element at index {i} is null, cannot convert");

                    // 將元素轉為 double 進行數值運算
                    double value = System.Convert.ToDouble(list[i]);
                    double result = action(value);

                    // 將運算結果轉回原本的元素型別，並更新回清單
                    list[i] = System.Convert.ChangeType(result, elementType);
                }
                catch (FormatException)
                {
                    // 當元素無法轉為 double 時拋出
                    throw new InvalidOperationException($"Element at index {i} ('{list[i]}') cannot be converted to a numeric value");
                }
                catch (InvalidCastException)
                {
                    // 當型別轉換失敗時拋出
                    throw new InvalidOperationException($"Element at index {i} (type: {list[i]?.GetType()}) cannot be converted to a numeric value");
                }
                catch (OverflowException)
                {
                    // 當運算結果超出目標型別的範圍時拋出
                    throw new InvalidOperationException($"Conversion result exceeds the range of {elementType.Name}");
                }
            }
        }
    }
}
