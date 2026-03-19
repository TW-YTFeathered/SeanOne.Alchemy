// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using SeanOne.Alchemy.Definitions;
using SeanOne.Alchemy.Sorting;
using SeanOne.Alchemy.Utility;
using System;

namespace SeanOne.Alchemy
{
    partial class AlchemyConverter
    {
        private static AlchemyResult Decoder(object copyObj, string dslInstruction)
        {
            // 從 DSL 指令中提取函數名稱
            string directive = Get.ExtractDirective(dslInstruction);

            // 嘗試從字典中獲取對應的執行函數
            if (s_ActionsSync.TryGetValue(directive, out var func))
            {
                // 找到並執行函數
                return func(copyObj, dslInstruction);
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

        private static AlchemyResult CNV(object copyObj, string dslInstruction)
        {
            // 排序部分
            string sortStr = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey(CnvParams.Sort), string.Empty);
            Sort.Entry(copyObj, sortStr);

            // 溫度轉換部分
            string tempCnvIns = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey(CnvParams.Temp), string.Empty);
            copyObj = ConvertTemperature(copyObj, tempCnvIns);

            // short time testing method
            //Console.Write(Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey("print"), string.Empty));

            return AlchemyResult.Parse(copyObj);
        }
    }
}
