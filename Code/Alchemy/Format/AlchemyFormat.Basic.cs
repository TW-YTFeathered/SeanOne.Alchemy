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
        /// 處理單個對象的格式化 (非同步)
        /// </summary>
        /// <param name="obj"> 目標物件 </param>
        /// <param name="dslInstruction"> Dsl 指令 </param>
        private static string Basic(object obj, string dslInstruction)
        {
            string format = string.Empty;

            string begin = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey(CommonParams.Begin), string.Empty);
            string end = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey(CommonParams.End), string.Empty);
            string prefix = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey(CommonParams.Prefix), string.Empty);
            string suffix = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey(CommonParams.Suffix), string.Empty);

            // 提取並驗證 /tostring: 參數
            if (Judge.HasString(dslInstruction, DslSyntaxBuilder.BuildParamKey(CommonParams.Tostring)))
            {
                format = Get.ExtractParameterValue(dslInstruction, DslSyntaxBuilder.BuildParamKey(CommonParams.Tostring));

                // 驗證 obj 是否實作 IFormattable
                if (obj != null && !Judge.SafeToString(obj))
                    throw new ArgumentException($"Collection elements must implement IFormattable for 'tostring'. Found: {obj.GetType().Name}");
            }

            if (!Judge.ValidateCodeParameters(dslInstruction, "basic", out var invalidParams))
                throw new ArgumentException($"Invalid parameters for basic processing: {string.Join(", ", invalidParams)}");

            // 格式化對象
            string result = FormatObject(obj, format);

            return prefix + begin + result + end + suffix;
        }
    }
}
