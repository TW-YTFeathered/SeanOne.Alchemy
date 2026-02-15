// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System.Collections;
using System.Linq;

namespace SeanOne.Alchemy.Sorting
{
    partial class ListSorter
    {
        private static void LinqSortList(IList list, bool isDescending)
        {
            // 將 IList 轉換為數組進行排序
            var array = new object[list.Count];
            list.CopyTo(array, 0);

            var sorted = array.ToArray();
            if (isDescending)
            {
                // 排序
                sorted = array.OrderBy(x => x).ToArray();
            }
            else
            {
                // 排序
                sorted = array.OrderByDescending(x => x).ToArray();
            }

            // 將排序結果放回原列表
            for (int i = 0; i < sorted.Length; i++)
            {
                list[i] = sorted[i];
            }
        }
    }
}
