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
                ["cnv"] = CNV,
                ["convert"] = CNV,
                ["fe"] = (object copyObj, string dslInstruction) => AlchemyResult.Parse(Format(copyObj, dslInstruction)),
                ["foreach"] = (object copyObj, string dslInstruction) => AlchemyResult.Parse(Format(copyObj, dslInstruction)),
                ["basic"] = (object copyObj, string dslInstruction) => AlchemyResult.Parse(Format(copyObj, dslInstruction))
            };
    }
}
