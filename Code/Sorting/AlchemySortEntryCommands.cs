// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections;
using System.Collections.Generic;

namespace SeanOne.Alchemy.Sorting
{
    /// <summary>
    /// 提供排序命令的靜態映射，將命令字串與具體的排序實作連結
    /// </summary>
    internal static class SortCommands
    {
        /// <summary>
        /// 存放所有 IList 可以成功排序的參數
        /// 鍵 (Key) 為命令字串 (如 "bs", "bubble")
        /// 值 (Value) 為對應的排序動作委託，接受 IList 參數並直接修改該清單
        /// </summary>
        /// <remarks>
        /// 所有命令字串均不區分大小寫，建議使用小寫以保持一致
        /// </remarks>
        public static readonly Dictionary<string, Action<IList>> s_ListSorterActions =
            new Dictionary<string, Action<IList>>(StringComparer.InvariantCultureIgnoreCase)
            {
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
