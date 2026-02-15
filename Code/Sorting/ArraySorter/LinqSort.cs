// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Linq;

namespace SeanOne.Alchemy.Sorting
{
    partial class ArraySorter
    {
        private static void LinqSortList<T>(T[] array, bool isDescending)
        {
            if (array == null || array.Length <= 1)
                return;

            // 直接對原陣列進行 LINQ 排序，產生排序後的陣列
            T[] sorted;
            if (isDescending)
                sorted = array.OrderByDescending(x => x).ToArray();
            else
                sorted = array.OrderBy(x => x).ToArray();

            // 將排序結果複製回原陣列
            Array.Copy(sorted, array, array.Length);
        }
    }
}
