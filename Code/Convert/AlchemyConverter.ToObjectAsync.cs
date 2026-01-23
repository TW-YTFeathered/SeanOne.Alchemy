using System.Threading.Tasks;

namespace SeanOne.Alchemy
{
    public partial class AlchemyConverter
    {
        private static async Task<AlchemyResult> Decoder_Async(object obj, string dslInstruction)
        {
            return await Task.Run(() =>
            {
                return AlchemyResult.Parse(obj);
            });
        }
    }
}
