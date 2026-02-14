// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        /// 取得 Convert 相關的非同步執行函數字典
        /// </summary>
        /// <remarks>
        /// 字典鍵值對應：
        /// - "cnv", "convert": 執行 CNV_Async 非同步轉換
        /// - "fe", "foreach", "basic": 執行 AlchemyFormatter 格式化（包裝為 Task）
        /// </remarks>
        private static readonly Dictionary<string, Func<object, string, Task<AlchemyResult>>> s_ActionsAsync = 
            new Dictionary<string, Func<object, string, Task<AlchemyResult>>>
            {
                ["cnv"] = (object copyObj, string dslInstruction) => CNV_Async(copyObj, dslInstruction),
                ["convert"] = (object copyObj, string dslInstruction) => CNV_Async(copyObj, dslInstruction),
                ["fe"] = (object copyObj, string dslInstruction) => Task.FromResult(AlchemyResult.Parse(AlchemyFormatter.Format(copyObj, dslInstruction))),
                ["foreach"] = (object copyObj, string dslInstruction) => Task.FromResult(AlchemyResult.Parse(AlchemyFormatter.Format(copyObj, dslInstruction))),
                ["basic"] = (object copyObj, string dslInstruction) => Task.FromResult(AlchemyResult.Parse(AlchemyFormatter.Format(copyObj, dslInstruction)))
            };
    }
}
