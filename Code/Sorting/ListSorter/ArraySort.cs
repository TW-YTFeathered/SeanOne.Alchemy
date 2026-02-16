// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SeanOne.Alchemy.Sorting
{
    partial class ListSorter
    {
        public static void ArraySortList(IList list, bool isDescending)
        {
            List<object> tempList = list.Cast<object>().ToList();
            if (isDescending)
                tempList.Sort((a, b) => Comparer.Default.Compare(b, a));
            else
                tempList.Sort();

            for (int i = 0; i < tempList.Count; i++)
                list[i] = tempList[i];
        }
    }
}
