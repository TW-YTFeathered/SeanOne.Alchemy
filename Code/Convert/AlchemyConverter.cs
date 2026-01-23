using System;
using System.Threading.Tasks;

namespace SeanOne.Alchemy
{
    public partial class AlchemyConverter
    {
        /// <summary>
        /// Converts the specified object according to the provided DSL instruction.
        /// </summary>
        /// <param name="obj">The source object to convert.</param>
        /// <param name="dslInstruction">The DSL instruction string.</param>
        /// <returns>An <see cref="AlchemyResult"/> representing the converted object.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="obj"/> is null or <paramref name="dslInstruction"/> is null or empty.
        /// </exception>
        public static AlchemyResult Convert(object obj, string dslInstruction)
        {
            // 檢查 物件 是否是 null
            if (obj == null)
                throw new ArgumentNullException("Input object must not be null.");

            // 檢查 DSL 指令是否為空或 null
            if (string.IsNullOrWhiteSpace(dslInstruction))
                throw new ArgumentNullException("Alchemy instruction cannot be null or empty");

            return Decoder(obj, dslInstruction);
        }

        /// <summary>
        /// Asynchronously converts the specified object according to the provided DSL instruction.
        /// </summary>
        /// <param name="obj">The source object to convert.</param>
        /// <param name="dslInstruction">The DSL instruction string.</param>
        /// <returns>An <see cref="AlchemyResult"/> representing the converted object.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="obj"/> is null or <paramref name="dslInstruction"/> is null or empty.
        /// </exception>
        public static async Task<AlchemyResult> ConvertAsync(object obj, string dslInstruction)
        {
            // 檢查 物件 是否是 null
            if (obj == null)
                throw new ArgumentNullException("Input object must not be null.");

            // 檢查 DSL 指令是否為空或 null
            if (string.IsNullOrWhiteSpace(dslInstruction))
                throw new ArgumentNullException("Alchemy instruction cannot be null or empty");

            return await Decoder_Async(obj, dslInstruction);
        }
    }
}
