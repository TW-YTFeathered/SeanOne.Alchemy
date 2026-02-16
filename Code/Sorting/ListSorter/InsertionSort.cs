// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections;

namespace SeanOne.Alchemy.Sorting
{
    partial class ListSorter
    {
        public static void InsertionSortList(IList list, bool isDescending)
        {
            if (list == null || list.Count <= 1)
                return;

            int n = list.Count;

            for (int i = 1; i < n; i++)
            {
                object key = list[i];
                int j = i - 1;

                // 根據升降序決定移動條件
                while (j >= 0)
                {
                    int comparison = s_Comparer.Compare(list[j], key);
                    bool needMove = isDescending ? comparison < 0 : comparison > 0;
                    if (!needMove) break;

                    list[j + 1] = list[j];
                    j--;
                }
                list[j + 1] = key;
            }
        }
    }
}
