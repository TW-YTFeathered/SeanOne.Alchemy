// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeanOne.Alchemy
{
    /// <summary>
    /// Provides extension methods for <see cref="Task{TResult}"/> where <c>TResult</c> is <see cref="AlchemyResult"/>.
    /// </summary>
    public static class AlchemyResultTaskExtensions
    {
        /// <summary>
        /// Asynchronously transforms the source object using the specified DSL instruction.
        /// </summary>
        /// <param name="task">The task that contains the source <see cref="AlchemyResult"/> to transform.</param>
        /// <param name="dslInstruction">The DSL instruction string.</param>
        /// <returns>A task representing the asynchronous operation, containing the transformed <see cref="AlchemyResult"/>.</returns>
        public static async Task<AlchemyResult> TransformAsync(this Task<AlchemyResult> task, string dslInstruction)
        {
            var result = await task;
            return await result.TransformAsync(dslInstruction);
        }

        // 轉換為 List<T> (非同步版本)
        public static async Task<List<T>> ToListAsync<T>(this Task<AlchemyResult> task)
        {
            var result = await task;
            return result.ToList<T>();
        }

        /// 轉換為對象 (非同步版本)
        public static async Task<T> ToObjectAsync<T>(this Task<AlchemyResult> task)
        {
            var result = await task;
            return result.ToObject<T>();
        }

        public static async Task<string> ToStringAsync(this Task<AlchemyResult> task)
        {
            var result = await task;
            return await result.ToStringAsync();
        }
    }
}
