// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Threading.Tasks;

// Used to ensure it's UTF-8 🤗
namespace SeanOne.Alchemy
{
    public partial class AlchemyConverter
    {
        private static async Task<AlchemyResult> Decoder_Async(object copyObj, string dslInstruction)
        {
            return await Task.Run(() =>
            {
                // short time testing method
                //Console.Write(Get.ParameterValueOrDefault(dslInstruction, DslSyntaxBuilder.BuildParamKey("print"), string.Empty));

                return AlchemyResult.Parse(copyObj);
            });
        }
    }
}
