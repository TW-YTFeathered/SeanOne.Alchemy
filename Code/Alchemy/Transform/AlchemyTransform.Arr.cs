// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using SeanOne.Alchemy.Definitions;
using SeanOne.Alchemy.Sorting;
using SeanOne.Alchemy.Utility;

namespace SeanOne.Alchemy
{
    partial class Alchemy
    {
        private static AlchemyResult Arr(object copyObj, string dslInstruction)
        {
            // 排序部分
            string sortStr = Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey(ArrParams.Sort), string.Empty);
            Sort.Entry(copyObj, sortStr);

            return AlchemyResult.Parse(copyObj);
        }
    }
}
