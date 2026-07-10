// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System.Collections;
using System.Linq;

namespace SeanOne.Alchemy.Sorting
{
    partial class ListSorter
    {
        public static void LinqSortList(IList list, bool isDescending)
        {
            var array = new object[list.Count];
            list.CopyTo(array, 0);

            object[] sorted;
            if (isDescending)
                sorted = array.OrderByDescending(x => x).ToArray();
            else
                sorted = array.OrderBy(x => x).ToArray();

            for (int i = 0; i < sorted.Length; i++)
                list[i] = sorted[i];
        }
    }
}
