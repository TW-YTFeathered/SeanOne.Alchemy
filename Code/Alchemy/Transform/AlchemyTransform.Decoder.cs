// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using SeanOne.Alchemy.Definitions;
using SeanOne.Alchemy.Utility;
using System;

namespace SeanOne.Alchemy
{
    partial class Alchemy
    {
        private static AlchemyResult TransformDecoder(object copyObj, string dslInstruction)
        {
            // 從 DSL 指令中提取函數名稱
            string directive = Get.Directive(dslInstruction);

            // 嘗試執行對應的轉換函式
            if (s_TransformActionsSync.TryGetValue(directive, out var func))
                return func(copyObj, dslInstruction);

            // 如果指令以 DSL 定義的參數前綴符號 (目前為 '/')開頭，則執行 Basic 方法
            if (dslInstruction.StartsWith(DslSymbols.ParamPrefix))
                return AlchemyResult.Parse(Format(copyObj, dslInstruction));

            // 未知指令拋出異常
            else
                throw new MissingMethodException($"Unknown functions directive: {directive}");
        }
    }
}
