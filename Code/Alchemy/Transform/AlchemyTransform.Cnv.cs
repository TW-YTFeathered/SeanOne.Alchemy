// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using SeanOne.Alchemy.Definitions;
using SeanOne.Alchemy.Utility;
using System;

namespace SeanOne.Alchemy
{
    partial class Alchemy
    {
        private static AlchemyResult Cnv(object copyObj, string dslInstruction)
        {
            Judge.EnsureParameters(dslInstruction, "cnv");

            // 溫度轉換部分
            string tempCnvIns = dslInstruction.ParamField(CnvParams.Temp);
            copyObj = ConvertTemperature(copyObj, tempCnvIns);

            // 重量轉換部分
            string weightCnvIns = dslInstruction.ParamField(CnvParams.Weight);
            copyObj = ConvertWeight(copyObj, weightCnvIns);

            // 長度轉換部分
            string lengthCnvIns = dslInstruction.ParamField(CnvParams.Length);
            copyObj = ConvertLength(copyObj, lengthCnvIns);

            return AlchemyResult.Parse(copyObj);
        }
    }
}
