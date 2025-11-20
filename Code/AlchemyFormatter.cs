using System;
using System.Threading.Tasks;

namespace SeanOne.Alchemy
{
    /// <summary>
    /// Transforms objects into formatted strings using a simple DSL syntax.
    /// </summary>
    public partial class AlchemyFormatter
    {
        /// <summary>
        /// Formats the specified object according to the provided DSL instruction.
        /// </summary>
        /// <param name="obj">The object to format.</param>
        /// <param name="dslInstruction">The DSL instruction string.</param>
        /// <returns>The formatted string.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="obj"/> is null or <paramref name="dslInstruction"/> is null or empty.
        /// </exception>
        public static string Format(object obj, string dslInstruction)
        {
            // 檢查 物件 是否是 null
            if (obj == null)
                throw new ArgumentNullException("Input object must not be null.");

            // 檢查 DSL 指令是否為空或 null
            if (string.IsNullOrWhiteSpace(dslInstruction))
                throw new ArgumentNullException("Alchemy instruction cannot be null or empty");

            dslInstruction = dslInstruction.Trim(); // 去除前後空白
            return Decoder(obj, dslInstruction); // 呼叫 Decoder 方法，並回傳結果
        }

        /// <summary>
        /// Asynchronously formats the specified object according to the provided DSL instruction.
        /// </summary>
        /// <param name="obj">The object to format.</param>
        /// <param name="dslInstruction">The DSL instruction string.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="obj"/> is null or <paramref name="dslInstruction"/> is null or empty.
        /// </exception>
        public static async Task<string> FormatAsync(object obj, string dslInstruction)
        {
            // 檢查 物件 是否是 null
            if (obj == null)
                throw new ArgumentNullException("Input object must not be null.");

            // 檢查 DSL 指令是否為空或 null
            if (string.IsNullOrWhiteSpace(dslInstruction))
                throw new ArgumentNullException("Alchemy instruction cannot be null or empty");

            dslInstruction = dslInstruction.Trim(); // 去除前後空白

            return await Decoder_Async(obj, dslInstruction); // 呼叫 Decoder 方法，並回傳結果
        }
    }
}
