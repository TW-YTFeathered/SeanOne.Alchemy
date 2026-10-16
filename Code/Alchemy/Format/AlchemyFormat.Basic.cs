// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using SeanOne.Alchemy.Definitions;
using SeanOne.Alchemy.Utility;
using System;

namespace SeanOne.Alchemy;

partial class Alchemy
{
    /// <summary>
    /// 處理單個對象的格式化
    /// </summary>
    /// <param name="obj"> 目標物件 </param>
    /// <param name="dslInstruction"> Dsl 指令 </param>
    private static string Basic(object obj, string dslInstruction)
    {
        Judge.EnsureParameters(dslInstruction, "basic");

        string begin = dslInstruction.ParamField(CommonParams.Begin);
        string end = dslInstruction.ParamField(CommonParams.End);
        string prefix = dslInstruction.ParamField(CommonParams.Prefix);
        string suffix = dslInstruction.ParamField(CommonParams.Suffix);

        string format = dslInstruction.ParamField(CommonParams.Tostring);

        // 驗證 obj 是否實作 IFormattable
        if (!string.IsNullOrEmpty(format) && obj != null && !Judge.CanFormat(obj))
            throw new ArgumentException($"Collection elements must implement IFormattable for 'tostring'. Found: {obj.GetType().Name}");

        // 格式化對象
        string result = FormatObject(obj, format);

        return prefix + begin + result + end + suffix;
    }
}
