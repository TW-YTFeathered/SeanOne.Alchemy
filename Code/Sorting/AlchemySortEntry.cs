// Copyright © TW-YTFeathered (https://github.com/TW-YTFeathered)
// SeanOne™ - A Professional Project and Brand.

using System;
using System.Collections;
using System.Collections.Generic;

namespace SeanOne.Alchemy
{
    internal partial class Sort
    {
        public static void Entry(object obj, string ins)
        {
            if (string.IsNullOrEmpty(ins))
            {
                // 可以返回或拋出異常
                return;
            }

            var ici = StringComparison.InvariantCultureIgnoreCase;

            if (ins.Equals("bs", ici) || ins.Equals("bubble", ici) || ins.Equals("bubblesort", ici))
            {
                if (obj is IList list)
                {
                    BubbleSortList(list);
                }
            }
            else
            {
                // 預計之後會抱錯
            }
        }

        private static void BubbleSortList(IList list)
        {
            if (list == null || list.Count <= 1)
                return;

            bool swapped;
            int n = list.Count;

            do
            {
                swapped = false;
                for (int i = 1; i < n; i++)
                {
                    // 使用 Comparer.Default 進行比較
                    if (Comparer.Default.Compare(list[i - 1], list[i]) > 0)
                    {
                        // 交換元素
                        object temp = list[i - 1];
                        list[i - 1] = list[i];
                        list[i] = temp;
                        swapped = true;
                    }
                }
                n--;
            } while (swapped);
        }
    }
}
