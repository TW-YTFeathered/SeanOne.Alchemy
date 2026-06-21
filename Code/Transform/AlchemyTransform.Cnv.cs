// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using SeanOne.Alchemy.Definitions;
using SeanOne.Alchemy.Utility;

namespace SeanOne.Alchemy
{
    partial class Alchemy
    {
        private static AlchemyResult Cnv(object copyObj, string dslInstruction)
        {
            // 溫度轉換部分
            string tempCnvIns = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey(CnvParams.Temp), string.Empty);
            copyObj = ConvertTemperature(copyObj, tempCnvIns);

            // 重量轉換部分
            string weightCnvIns = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey(CnvParams.Weight), string.Empty);
            copyObj = ConvertWeight(copyObj, weightCnvIns);

            // 長度轉換部分
            string lengthCnvIns = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey(CnvParams.Length), string.Empty);
            copyObj = ConvertLength(copyObj, lengthCnvIns);

            return AlchemyResult.Parse(copyObj);
        }
    }
}
