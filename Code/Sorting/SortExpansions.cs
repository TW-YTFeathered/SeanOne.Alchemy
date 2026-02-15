// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections;

namespace SeanOne.Alchemy.Sorting
{
    internal static class SortExpansions
    {
        /// <summary>
        /// 交換 <see cref="IList"/> 中指定索引與前一個索引的元素，並將 <paramref name="swapped"/> 設為 true
        /// </summary>
        /// <param name="list"> 要進行交換的 <see cref="IList"/> 集合 </param>
        /// <param name="index"> 要交換的後一個元素索引 (與索引 index-1 的元素交換) </param>
        /// <param name="swapped"> 參考的布林值，方法執行後會被設為 true，表示交換已發生  </param>
        /// <remarks>
        /// 此方法假設 <paramref name="index"/> 大於等於 1 且小於 <paramref name="list"/> 的 Count，呼叫者需確保索引有效
        /// 主要用於排序演算法 (如氣泡排序) 內部，以簡化交換邏輯
        /// </remarks>
        public static void Swap(this IList list, int index, ref bool swapped)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (index < 1 || index >= list.Count) throw new ArgumentOutOfRangeException(nameof(index));

            object temp = list[index - 1];
            list[index - 1] = list[index];
            list[index] = temp;
            swapped = true;
        }

        /// <summary>
        /// 交換泛型陣列中指定索引與前一個索引的元素，並將 <paramref name="swapped"/> 設為 true
        /// </summary>
        /// <typeparam name="T"> 陣列中元素的型別 </typeparam>
        /// <param name="array"> 要進行交換的陣列</param>
        /// <param name="index"> 要交換的後一個元素索引 (與索引 index-1 的元素交換) </param>
        /// <param name="swapped"> 參考的布林值，方法執行後會被設為 true，表示交換已發生 </param>
        /// <remarks>
        /// 此方法假設 <paramref name="index"/> 大於等於 1 且小於陣列長度，呼叫者需確保索引有效
        /// 與 <see cref="Swap(IList, int, ref bool)"/> 不同，此版本為強型別，避免了裝箱（boxing）開銷，適合直接操作陣列
        /// 主要用於排序演算法（如氣泡排序）內部
        /// </remarks>
        public static void Swap<T>(this T[] array, int index, ref bool swapped)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (index < 1 || index >= array.Length) throw new ArgumentOutOfRangeException(nameof(index));

            T temp = array[index - 1];
            array[index - 1] = array[index];
            array[index] = temp;
            swapped = true;
        }
    }
}
