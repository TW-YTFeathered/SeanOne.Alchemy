// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SeanOne.Alchemy.Sorting
{
    /// <summary>
    /// 提供排序命令的靜態映射，將命令字串與具體的排序實作連結
    /// </summary>
    internal static class SortCommands
    {
        static SortCommands()
        {
            var comparer = StringComparer.InvariantCultureIgnoreCase;

            InitListSorterDict(comparer);
        }

        private static void InitListSorterDict(StringComparer comparer)
        {
            var searchTruncation = new Regex(@"^[A-Z][a-z]*");

            var dict = new Dictionary<string, Action<IList>>(comparer);

            foreach (var item in s_SortAlgorithmMap)
            {
                string key = item.Key;

                string abbreviation = new string(key.Where(char.IsUpper).ToArray());
                string abbreviationDesc = abbreviation + "d";

                string truncation = searchTruncation.Match(key).Value;
                string truncationDesc = truncation + "desc";

                string fullname = key;
                string fullnameDesc = fullname + "descending";

                var originalAction = item.Value;
                Action<IList> wrappedAscending = list => originalAction(list, false);
                Action<IList> wrappedDescending = list => originalAction(list, true);

                dict[abbreviation] = wrappedAscending;
                dict[truncation] = wrappedAscending;
                dict[fullname] = wrappedAscending;

                dict[abbreviationDesc] = wrappedDescending;
                dict[truncationDesc] = wrappedDescending;
                dict[fullnameDesc] = wrappedDescending;
            }
            ListSorterActions = dict;
        }

        private static readonly IReadOnlyDictionary<string, Action<IList, bool>> s_SortAlgorithmMap =
            new Dictionary<string, Action<IList, bool>>()
            {
                ["InsertionSort"] = ListSorter.InsertionSortList,
                ["ArraySort"] = ListSorter.ArraySortList,
                ["LinqSort"] = ListSorter.LinqSortList
            };

        /// <summary>
        /// 存放所有 IList 可以成功排序的參數
        /// 鍵 (Key) 為命令字串 (如 "bs", "bubble")
        /// 值 (Value) 為對應的排序動作委託，接受 IList 參數並直接修改該清單
        /// </summary>
        /// <remarks>
        /// 所有命令字串均不區分大小寫，建議使用小寫以保持一致
        /// </remarks>
        public static IReadOnlyDictionary<string, Action<IList>> ListSorterActions { get; private set; }
    }
}
