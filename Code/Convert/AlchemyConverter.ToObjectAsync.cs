// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using SeanOne.Alchemy.Sorting;
using SeanOne.Alchemy.Utility;
using System;
using System.Threading.Tasks;

namespace SeanOne.Alchemy
{
    partial class AlchemyConverter
    {
        private static async Task<AlchemyResult> Decoder_Async(object copyObj, string dslInstruction)
        {
            // 從 DSL 指令中提取函數名稱
            string directive = Get.ExtractDirective(dslInstruction);

            // 嘗試從字典中獲取對應的執行函數
            if (s_ActionsAsync.TryGetValue(directive, out var func))
            {
                // 找到並執行函數
                return await func(copyObj, dslInstruction);
            }
            else
            {
                // 如果指令以 DSL 定義的參數前綴符號（目前為 '/'）開頭，則執行 Basic 方法
                if (dslInstruction.StartsWith(DslSymbols.ParamPrefix))
                {
                    return AlchemyResult.Parse(AlchemyFormatter.Format(copyObj, dslInstruction));
                }
                else
                {
                    // 未知指令拋出異常
                    throw new MissingMethodException($"Unknown functions directive: {directive}");
                }
            }
        }

        private static async Task<AlchemyResult> CNV_Async(object copyObj, string dslInstruction)
        {
            string sortStr = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey("sort"), string.Empty);

            Sort.Entry(copyObj, sortStr);

            // short time testing method
            //Console.Write(Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey("print"), string.Empty));

            return AlchemyResult.Parse(copyObj);
        }
    }
}
