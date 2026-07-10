// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections.Generic;

namespace SeanOne.Alchemy
{
    partial class Alchemy
    {
        /// <summary>
        /// 取得 Transform 相關的同步執行函數字典
        /// </summary>
        /// <remarks>
        /// 字典鍵值對應: 
        /// - "cnv", "convert": 執行 CNV 轉換
        /// - "fe", "foreach", "basic": 執行 AlchemyFormatter 格式化
        /// </remarks>
        private static readonly Dictionary<string, Func<object, string, AlchemyResult>> s_TransformActionsSync =
            new Dictionary<string, Func<object, string, AlchemyResult>>
            {
                ["cnv"] = Cnv,
                ["convert"] = Cnv,
                ["arr"] = Arr,
                ["arrange"] = Arr,
                ["fe"] = (object copyObj, string dslInstruction) => AlchemyResult.Parse(Format(copyObj, dslInstruction)),
                ["foreach"] = (object copyObj, string dslInstruction) => AlchemyResult.Parse(Format(copyObj, dslInstruction)),
                ["basic"] = (object copyObj, string dslInstruction) => AlchemyResult.Parse(Format(copyObj, dslInstruction))
            };

        /// <summary>
        /// 取得 Format 相關的同步執行函數字典
        /// </summary>
        /// <remarks>
        /// 字典鍵值對應: 
        /// - "fe", "foreach": 執行 FE 函數 (帶對應的類型參數)
        /// - "basic": 執行 Basic 函數
        /// </remarks>
        private static readonly Dictionary<string, Func<object, string, string>> s_FormatActionsSync =
            new Dictionary<string, Func<object, string, string>>
            {
                ["fe"] = (obj, dslInstruction) => FE(obj, dslInstruction, "fe"),
                ["foreach"] = (obj, dslInstruction) => FE(obj, dslInstruction, "foreach"),
                ["basic"] = Basic,
            };
    }
}
