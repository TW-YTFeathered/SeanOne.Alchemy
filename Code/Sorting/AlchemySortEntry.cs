// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections;

namespace SeanOne.Alchemy.Sorting
{
    internal partial class Sort
    {
        public static void Entry<T>(T obj, string ins)
        {
            if (string.IsNullOrEmpty(ins))
            {
                // 可以返回或拋出異常
                return;
            }

            if (obj is IList list)
            {
                ListSorter.Sort(list, ins);
            }
            else
            {
                // 預計之後會抱錯
            }
        }
    }
}
