// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using SeanOne.Alchemy.Utility;
using System;
using System.Threading.Tasks;

namespace SeanOne.Alchemy
{
    public partial class AlchemyConverter
    {
        /// <summary>
        /// Converts the specified object according to the provided DSL instruction.
        /// </summary>
        /// <param name="obj">The source object to convert.</param>
        /// <param name="dslInstruction">The DSL instruction string.</param>
        /// <returns>An <see cref="AlchemyResult"/> representing the converted object.</returns>
        public static AlchemyResult Convert(object obj, string dslInstruction)
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

        public static AlchemyResult Convert(object obj, params string[] dslInstructions)
        {
            // 檢查 物件 是否是 null
            if (obj == null)
                throw new ArgumentNullException("Input object must not be null.");

            // 檢查 DSL 指令是否為空或 null
            if (dslInstructions.Length == 0)
                throw new ArgumentNullException("Alchemy instruction cannot be null or empty");

            // 進行深層拷貝，避免對原始物件進行修改
            object current = ReflectionCloner.DeepClone(obj);

            string oldDirective = string.Empty;
            foreach (var ins in dslInstructions)
            {
                var temp = ins.Trim(); // 去除前後空白，但因為 ins 無法被赴值，只能再來一個變數

                // 從 DSL 指令中提取指令名稱
                string directive = Get.ExtractDirective(temp);

                // 為了簡化寫法，避免要在 DSL 指令中一直寫指令名稱
                if (string.IsNullOrWhiteSpace(directive) && !string.IsNullOrWhiteSpace(oldDirective))
                {
                    temp = oldDirective + temp;
                }
                // 更新 oldDirective，避免無法使用免寫指令名稱
                if (!string.IsNullOrEmpty(directive))
                {
                    oldDirective = directive;
                }


                // 為了支援多指令
                current = Decoder(current, temp).ToObject<object>();
            }

            return AlchemyResult.Parse(current);
        }

        /// <summary>
        /// Asynchronously converts the specified object according to the provided DSL instruction.
        /// </summary>
        /// <param name="obj">The source object to convert.</param>
        /// <param name="dslInstruction">The DSL instruction string.</param>
        /// <returns>An <see cref="AlchemyResult"/> representing the converted object.</returns>
        public static async Task<AlchemyResult> ConvertAsync(object obj, string dslInstruction)
        {
            return await Task.Run(() =>
            {
                return Convert(obj, dslInstruction);
            });
        }

        public static async Task<AlchemyResult> ConvertAsync(object obj, params string[] dslInstructions)
        {
            return await Task.Run(() =>
            {
                return Convert(obj, dslInstructions);
            });
        }
    }
}
