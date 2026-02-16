// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections;

namespace SeanOne.Alchemy.Sorting
{
    internal partial class ListSorter
    {
        private static readonly Comparer s_Comparer = Comparer.Default;

        public static void Sort(IList list, string algorithm)
        {
            // 嘗試取得函式，並執行
            if (SortCommands.s_ListSorterActions.TryGetValue(algorithm, out var action))
            {
                action(list);
            }
            else
            {
                // 預計之後抱錯
                throw new ArgumentException("Invalid sorting algorithm.", nameof(algorithm));
            }
        }
    }
}
