// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using SeanOne.Alchemy.Definitions;
using SeanOne.Alchemy.Utility;
using System;

namespace SeanOne.Alchemy
{
    partial class Alchemy
    {
        /// <summary>
        /// 解碼並選擇適當的方法
        /// </summary>
        /// <param name="obj"> 目標物件 </param>
        /// <param name="dslInstruction"> Dsl 指令 </param>
        private static string FormatDecoder(object obj, string dslInstruction)
        {
            // 從 DSL 指令中提取函數名稱
            string directive = Get.ExtractDirective(dslInstruction);

            // 嘗試從字典中獲取對應的執行函數
            if (s_FormatActionsSync.TryGetValue(directive, out var func))
            {
                // 找到並執行函數
                return func(obj, dslInstruction);
            }
            else
            {
                // 如果指令以 DSL 定義的參數前綴符號 (目前為 '/')開頭，則執行 Basic 方法
                if (dslInstruction.StartsWith(DslSymbols.ParamPrefix))
                {
                    return Basic(obj, dslInstruction);
                }
                else
                {
                    // 未知指令拋出異常
                    throw new MissingMethodException($"Unknown functions directive: {directive}");
                }
            }
        }
    }
}
