using System;
using System.Collections;
using System.Collections.Generic;

namespace SeanOne.Alchemy
{
    public partial class AlchemyConverter
    {
        private static AlchemyResult Decoder(object obj, string dslInstruction)
        {
            // 從 DSL 指令中提取函數名稱
            string directive = dslInstruction.Contains(DslSymbols.ParamPrefix) ?
                dslInstruction.Substring(0, dslInstruction.IndexOf(DslSymbols.ParamPrefix)).Trim()
                : dslInstruction;

            // 創建函數名稱字典，映射到對應的執行函數
            Dictionary<string, Func<AlchemyResult>> actions = new Dictionary<string, Func<AlchemyResult>>
            {
                ["cnv"] = () => CNV(obj, dslInstruction),
                ["convert"] = () => CNV(obj, dslInstruction),
                ["fe"] = () => AlchemyResult.Parse(AlchemyFormatter.Format(obj, dslInstruction)),
                ["foreach"] = () => AlchemyResult.Parse(AlchemyFormatter.Format(obj, dslInstruction)),
                ["basic"] = () => AlchemyResult.Parse(AlchemyFormatter.Format(obj, dslInstruction))
            };

            // 嘗試從字典中獲取對應的執行函數
            if (actions.TryGetValue(directive, out var func))
            {
                // 找到並執行函數
                return func();
            }
            else
            {
                // 如果指令以 DSL 定義的參數前綴符號（目前為 '/'）開頭，則執行 Basic 方法
                if (dslInstruction.StartsWith(DslSymbols.ParamPrefix))
                {
                    return AlchemyResult.Parse(AlchemyFormatter.Format(obj, dslInstruction));
                }
                else
                {
                    // 未知指令拋出異常
                    throw new MissingMethodException($"Unknown functions directive: {directive}");
                }
            }
        }

        private static AlchemyResult CNV(object obj, string dslInstruction)
        {
            string sort = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey("sort"), string.Empty);

            return AlchemyResult.Parse(obj);
        }
    }
}
