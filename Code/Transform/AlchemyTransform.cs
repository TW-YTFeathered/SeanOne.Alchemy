// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using SeanOne.Alchemy.Utility;
using System;
using System.Threading.Tasks;

namespace SeanOne.Alchemy
{
    /// <summary>
    /// Converts objects into new representations using a DSL instruction language.
    /// </summary>
    public partial class Alchemy
    {
        /// <summary>
        /// Transforms the specified object according to the provided DSL instruction.
        /// </summary>
        /// <param name="obj">The source object to transform.</param>
        /// <param name="dslInstruction">The DSL instruction string.</param>
        /// <returns>An <see cref="AlchemyResult"/> representing the transformed object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is <c>null</c> or <paramref name="dslInstruction"/> is empty.</exception>
        public static AlchemyResult Transform(object obj, string dslInstruction)
        {
            // 檢查 物件 是否是 null
            if (obj == null)
                throw new ArgumentNullException("Input object must not be null.");

            // 檢查 DSL 指令是否為空或 null
            if (string.IsNullOrWhiteSpace(dslInstruction))
                throw new ArgumentNullException("Alchemy instruction cannot be null or empty");

            dslInstruction = dslInstruction.Trim(); // 去除前後空白

            // 先進行深層拷貝，避免對原始物件進行修改
            object copy = ReflectionCloner.DeepClone(obj);

            return Decoder(copy, dslInstruction); // 呼叫 Decoder 方法，並回傳結果
        }

        /// <summary>
        /// Transforms the specified object by sequentially applying multiple DSL instructions.
        /// </summary>
        /// <param name="obj">The source object to transform.</param>
        /// <param name="dslInstructions">An array of DSL instruction strings to apply in order.</param>
        /// <returns>An <see cref="AlchemyResult"/> representing the final transformed object after all instructions.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is <c>null</c> or <paramref name="dslInstructions"/> is empty.</exception>
        public static AlchemyResult Transform(object obj, params string[] dslInstructions)
        {
            // 檢查 物件 是否是 null
            if (obj == null)
                throw new ArgumentNullException("Input object must not be null.");

            // 檢查 DSL 指令是否為空或 null
            if (dslInstructions.Length == 0)
                throw new ArgumentNullException("Alchemy instruction cannot be null or empty");

            // 進行深層拷貝，避免對原始物件進行修改
            object current = ReflectionCloner.DeepClone(obj);

            // 用於記錄上一個指令的名稱，以便後續指令省略指令名稱時自動補齊
            string oldDirective = string.Empty;
            foreach (var ins in dslInstructions)
            {
                var temp = ins.Trim(); // 去除前後空白，但因為 ins 無法被赴值，只能再來一個變數

                // 從 DSL 指令中提取指令名稱
                string directive = Get.ExtractDirective(temp);

                // 若當前指令沒有指定指令名稱，且存在上一個指令名稱，則自動補齊
                // 例如: ["cnv /sort:as", "/sort:asd"] => 第二個指令會自動變成 "cnv /sort:asd"
                if (string.IsNullOrWhiteSpace(directive) && !string.IsNullOrWhiteSpace(oldDirective))
                {
                    temp = oldDirective + temp;
                }

                // 更新 oldDirective 為當前有效的指令名稱
                if (!string.IsNullOrEmpty(directive))
                {
                    oldDirective = directive;
                }

                // Decoder 返回 AlchemyResult，透過 ToObject<object>() 取得轉換後的實際物件，
                // 以便作為下一個指令的輸入。
                current = Decoder(current, temp).ToObject<object>();
            }

            // 將最終轉換結果包裝成 AlchemyResult 回傳
            return AlchemyResult.Parse(current);
        }

        /// <summary>
        /// Asynchronously transforms the specified object according to the provided DSL instruction.
        /// </summary>
        /// <param name="obj">The source object to transform.</param>
        /// <param name="dslInstruction">The DSL instruction string.</param>
        /// <returns>
        /// A task that represents the asynchronous transformation operation.
        /// The task result contains an <see cref="AlchemyResult"/> representing the transformed object.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is <c>null</c> or <paramref name="dslInstruction"/> is <c>null</c> or empty.</exception>
        public static async Task<AlchemyResult> TransformAsync(object obj, string dslInstruction)
        {
            // 直接引用 Convert，避免程式碼過長或是重複性過高，並將其包在 Task.Run 中，以實現非同步執行
            return await Task.Run(() =>
            {
                return Transform(obj, dslInstruction);
            });
        }

        /// <summary>
        /// Asynchronously transforms the specified object by sequentially applying multiple DSL instructions.
        /// </summary>
        /// <param name="obj">The source object to transform.</param>
        /// <param name="dslInstructions">An array of DSL instruction strings to apply in order.</param>
        /// <returns>
        /// A task that represents the asynchronous transformation operation.
        /// The task result contains an <see cref="AlchemyResult"/> representing the final transformed object after all instructions.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is <c>null</c> or <paramref name="dslInstructions"/> is <c>null</c> or empty.</exception>
        public static async Task<AlchemyResult> TransformAsync(object obj, params string[] dslInstructions)
        {
            // 直接引用 Convert，避免程式碼過長或是重複性過高，並將其包在 Task.Run 中，以實現非同步執行
            return await Task.Run(() =>
            {
                return Transform(obj, dslInstructions);
            });
        }
    }
}
