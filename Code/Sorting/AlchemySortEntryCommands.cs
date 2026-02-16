// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections;
using System.Collections.Generic;

namespace SeanOne.Alchemy.Sorting
{
    /// <summary>
    /// 
    /// </summary>
    internal static class SortCommands
    {
        public static readonly Dictionary<string, Action<IList>> s_ListSorterActions =
            new Dictionary<string, Action<IList>>(StringComparer.InvariantCultureIgnoreCase)
            {
                // 氣泡排序升序
                { "bs", list => ListSorter.BubbleSortList(list, false) },
                { "bubble", list => ListSorter.BubbleSortList(list, false) },
                { "bubblesort", list => ListSorter.BubbleSortList(list, false) },
                // 氣泡排序降序
                { "bsd", list => ListSorter.BubbleSortList(list, true) },
                { "bubbledesc", list => ListSorter.BubbleSortList(list, true) },
                { "bubbledescending", list => ListSorter.BubbleSortList(list, true) },
                { "bubblesortdescending", list => ListSorter.BubbleSortList(list, true) },

                // 插入排序升序
                { "is", list => ListSorter.InsertionSortList(list, false) },
                { "insertion", list => ListSorter.InsertionSortList(list, false) },
                { "insertionsort", list => ListSorter.InsertionSortList(list, false) },
                // 插入排序降序
                { "isd", list => ListSorter.InsertionSortList(list, true) },
                { "insertiondesc", list => ListSorter.InsertionSortList(list, true) },
                { "insertiondescending", list => ListSorter.InsertionSortList(list, true) },

                // Array.Sort 升序
                { "as", list => ListSorter.ArraySortList(list, false) },
                { "arraysort", list => ListSorter.ArraySortList(list, false) },
                // Array.Sort 降序
                { "asd", list => ListSorter.ArraySortList(list, true) },
                { "arraysortdesc", list => ListSorter.ArraySortList(list, true) },
                { "arraysortdescending", list => ListSorter.ArraySortList(list, true) },

                // Linq.OrderBy 升序
                { "ls", list => ListSorter.LinqSortList(list, false) },
                { "linq", list => ListSorter.LinqSortList(list, false) },
                { "linqsort", list => ListSorter.LinqSortList(list, false) },
                // Linq.OrderBy 降序
                { "lsd", list => ListSorter.LinqSortList(list, true) },
                { "linqsortdesc", list => ListSorter.LinqSortList(list, true) },
                { "linqsortdescending", list => ListSorter.LinqSortList(list, true) },
            };
    }
}
