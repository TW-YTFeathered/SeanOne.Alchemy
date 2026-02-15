// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections;

namespace SeanOne.Alchemy.Sorting
{
    internal partial class ArraySorter
    {
        /// <summary>
        /// 用於 objectA 和 objectB 比較的
        /// </summary>
        private static readonly Comparer s_Comparer = Comparer.Default;
        
        public static void Sort<T>(T[] array, string algorithm)
        {
            if (SortCommands.IsBubbleSortCommand(algorithm))
            {
                BubbleSortList(array, false);
            }
            else if (SortCommands.IsBubbleSortDescendingCommand(algorithm))
            {
                BubbleSortList(array, true);
            }
            else if (SortCommands.IsLinqSortCommand(algorithm))
            {
                LinqSortList(array, false);
            }
            else if (SortCommands.IsLinqSortDescendingCommand(algorithm))
            {
                LinqSortList(array, true);
            }
            else
            {
                // 預計之後抱錯
                throw new ArgumentException("Invalid sorting algorithm.", nameof(algorithm));
            }
        }
    }
}
