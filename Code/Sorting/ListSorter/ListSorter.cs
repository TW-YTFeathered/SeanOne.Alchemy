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
            if (SortCommands.IsBubbleSortCommand(algorithm))
            {
                BubbleSortList(list, false);
            }
            else if (SortCommands.IsBubbleSortDescendingCommand(algorithm))
            {
                BubbleSortList(list, true);
            }
            else if (SortCommands.IsLinqSortCommand(algorithm))
            {
                LinqSortList(list, false);
            }
            else if (SortCommands.IsLinqSortDescendingCommand(algorithm))
            {
                LinqSortList(list, true);
            }
            else
            {
                // 預計之後抱錯
                throw new ArgumentException("Invalid sorting algorithm.", nameof(algorithm));
            }
        }
    }
}
