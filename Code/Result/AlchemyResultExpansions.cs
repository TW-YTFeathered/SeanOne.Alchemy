// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SeanOne.Alchemy
{
    /// <summary>
    /// Provides extension methods for the <see cref="AlchemyResult"/> class.
    /// </summary>
    public static class AlchemyResultTaskExtensions
    {
        /// <summary>
        /// Asynchronously converts the specified object according to the provided DSL instruction.
        /// </summary>
        /// <param name="task">The task that contains the source object to convert.</param>
        /// <param name="dslInstruction">The DSL instruction string.</param>
        /// <returns>An <see cref="AlchemyResult"/> representing the converted object.</returns>
        public static async Task<AlchemyResult> ConvertAsync(this Task<AlchemyResult> task, string dslInstruction)
        {
            var result = await task;
            return await result.ConvertAsync(dslInstruction);
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
