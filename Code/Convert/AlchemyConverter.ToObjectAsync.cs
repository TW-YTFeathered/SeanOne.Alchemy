using System;
using System.Threading.Tasks;

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
